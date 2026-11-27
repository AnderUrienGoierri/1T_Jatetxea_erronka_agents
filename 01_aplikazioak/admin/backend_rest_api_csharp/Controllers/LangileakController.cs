using System.Data;
using System.Security.Claims;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Helpers;
using Jatetxea.Api.Models;
using Jatetxea.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/langileak")]
public class LangileakController : BaseApiKontroladorea
{
    private readonly IConfiguration _config;

    public LangileakController(ISessionFactory sessionFactory, IConfiguration config) : base(sessionFactory)
    {
        _config = config;
    }

    /// <summary>
    /// Langile guztien zerrenda lortzen du (pasahitzik gabe).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Langilea>();
        if (!barneEzabatuak)
            query = query.Where(l => !l.Ezabatua);

        var langileak = await query
            .OrderBy(l => l.Izena)
            .ToListAsync();

        var zerrenda = langileak.Select(l => new LangileaDTO
        {
            Id = l.Id,
            Erabiltzailea = l.Erabiltzailea,
            Izena = l.Izena,
            Abizena = l.Abizena,
            NanDni = l.NanDni,
            Emaila = l.Emaila,
            Rola = l.Rola,
            Aktibo = l.Aktibo,
            TxataAktibo = l.TxataAktibo,
            Ezabatua = l.Ezabatua
        }).ToList();

        return SuccessList(zerrenda, 200, "Langileak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Langile bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var l = await session.GetAsync<Langilea>(id);
        if (l == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen langilerik.");

        var dto = new LangileaDTO
        {
            Id = l.Id,
            Erabiltzailea = l.Erabiltzailea,
            Izena = l.Izena,
            Abizena = l.Abizena,
            NanDni = l.NanDni,
            Emaila = l.Emaila,
            Rola = l.Rola,
            Aktibo = l.Aktibo,
            TxataAktibo = l.TxataAktibo,
            Ezabatua = l.Ezabatua
        };

        return Success(dto, 200, "Langilea ondo aurkitu da.");
    }

    /// <summary>
    /// Langilearen saio-hasiera (Login edo Tableteko PIN bidezko autentifikazioa).
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> SaioaHasi([FromBody] LangileLoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Pasahitza) && !string.IsNullOrWhiteSpace(dto.Pin))
        {
            dto.Pasahitza = dto.Pin;
        }

        if (string.IsNullOrWhiteSpace(dto.Erabiltzailea) || string.IsNullOrWhiteSpace(dto.Pasahitza))
            return BadRequestError("Erabiltzailea/PIN eta pasahitza beharrezkoak dira.");

        using var session = IrekiSaioa();
        string erab = dto.Erabiltzailea.Trim();
        int posibleId = int.TryParse(erab, out int idBalioa) ? idBalioa : -1;

        var langilea = await session.Query<Langilea>()
            .FirstOrDefaultAsync(l => !l.Ezabatua && (l.Erabiltzailea == erab || l.Id == posibleId));

        if (langilea == null)
            return Unauthorized(new { status = "error", message = "Erabiltzailea edo PINa ez da zuzena." });

        if (!langilea.Aktibo)
            return Unauthorized(new { status = "error", message = "Langile hau desgaituta dago." });

        // Egiaztatu pasahitza / PIN propioa BCrypt bidez (edo testu laua migrazio leunean)
        bool pasahitzZuzena = SegurtasunHelper.EgiaztatuPin(dto.Pasahitza, langilea.PasahitzaHash, out bool berrituBeharDa);
        if (!pasahitzZuzena)
            return Unauthorized(new { status = "error", message = "PIN edo pasahitz okerra. Mesedez, idatzi zure 4 digituko PIN propioa." });

        // Testu laua bazen, oraintxe bertan automatikoki BCrypt hash-era berritu datu-basean
        if (berrituBeharDa && !string.IsNullOrWhiteSpace(dto.Pasahitza))
        {
            try
            {
                using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);
                langilea.PasahitzaHash = SegurtasunHelper.HashPin(dto.Pasahitza);
                await session.UpdateAsync(langilea);
                await tx.CommitAsync();
            }
            catch
            {
                // Isilpeko erregistroa: login-a arrakastatsua da hala ere
            }
        }

        var langileaDto = new LangileaDTO
        {
            Id = langilea.Id,
            Erabiltzailea = langilea.Erabiltzailea,
            Izena = langilea.Izena,
            Abizena = langilea.Abizena,
            NanDni = langilea.NanDni,
            Emaila = langilea.Emaila,
            Rola = langilea.Rola,
            Aktibo = langilea.Aktibo,
            TxataAktibo = langilea.TxataAktibo,
            Ezabatua = false
        };

        var (jwtToken, iraungitzea) = JwtHelper.SortuTokena(langilea, _config);

        var loginEmaitza = new LoginEmaitzaDto
        {
            Arrakasta = true,
            Mezua = $"Ongi etorri, {langileaDto.Izena} ({langileaDto.Rola})!",
            Langilea = langileaDto,
            Token = jwtToken,
            TokenMota = "Bearer",
            IraungitzeData = iraungitzea.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        // Monitore zentralean konektatutako erabiltzailea erregistratu
        string appId = AplikazioMonitoreZerbitzua.IdentifikatuAplikazioa(HttpContext);
        AplikazioMonitoreZerbitzua.ErregistratuErabiltzailea(appId, langileaDto.Izena);

        return Success(loginEmaitza, 200, "Saioa arrakastaz hasi da.");
    }

    /// <summary>
    /// JWT Bearer tokena egiaztatzen du eta autentifikatutako langilearen profila itzultzen du.
    /// </summary>
    [Authorize]
    [HttpGet("nire-profila")]
    public IActionResult LortuNireProfila()
    {
        var profila = new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            izena = User.FindFirstValue(ClaimTypes.Name),
            erabiltzailea = User.FindFirstValue("erabiltzailea"),
            rola = User.FindFirstValue(ClaimTypes.Role),
            saila = User.FindFirstValue("saila")
        };

        return Success(profila, 200, "JWT tokena baliozkoa da eta profila berreskuratu da.");
    }

    /// <summary>
    /// Langile berria erregistratzen du (PINa automatikoki BCrypt hash bidez zifratuz).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] LangileaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");
        if (!RolaBaliozkoa(dto.Rola))
            return BadRequestError("Langilearen rola ez da baliozkoa.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var langilea = new Langilea
            {
                Erabiltzailea = dto.Erabiltzailea.Trim(),
                PasahitzaHash = SegurtasunHelper.HashPin(dto.Pasahitza),
                Izena = dto.Izena.Trim(),
                Abizena = dto.Abizena.Trim(),
                NanDni = dto.NanDni.Trim(),
                Emaila = dto.Emaila.Trim(),
                Rola = dto.Rola,
                Saila = "Jantokia",
                Kargua = "Langilea",
                Aktibo = true,
                TxataAktibo = dto.TxataAktibo,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(langilea);
            await tx.CommitAsync();

            var emaitza = new LangileaDTO
            {
                Id = langilea.Id,
                Erabiltzailea = langilea.Erabiltzailea,
                Izena = langilea.Izena,
                Abizena = langilea.Abizena,
                NanDni = langilea.NanDni,
                Emaila = langilea.Emaila,
                Rola = langilea.Rola,
                Aktibo = true,
                TxataAktibo = langilea.TxataAktibo,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Langilea arrakastaz sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Erabiltzaile-izena, NANa edo emaila jada erregistratuta dago.");
        }
    }

    /// <summary>
    /// Langilearen datuak eguneratzen ditu blokeo pesimistan (LockMode.Upgrade); PIN berria aukerakoa da.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] LangileaEguneratuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Langilearen datuak ez dira baliozkoak.");
        if (!RolaBaliozkoa(dto.Rola))
            return BadRequestError("Langilearen rola ez da baliozkoa.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var langilea = await session.GetAsync<Langilea>(id, LockMode.Upgrade);
            if (langilea == null || langilea.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen langile aktiborik.");
            }

            string? hashedPin = string.IsNullOrWhiteSpace(dto.Pasahitza)
                ? null
                : SegurtasunHelper.HashPin(dto.Pasahitza);

            langilea.Erabiltzailea = dto.Erabiltzailea.Trim();
            langilea.Izena = dto.Izena.Trim();
            langilea.Abizena = dto.Abizena.Trim();
            langilea.NanDni = dto.NanDni.Trim();
            langilea.Emaila = dto.Emaila.Trim();
            langilea.Rola = dto.Rola;
            if (dto.TxataAktibo.HasValue)
            {
                langilea.TxataAktibo = dto.TxataAktibo.Value;
            }
            if (dto.Aktibo.HasValue)
            {
                langilea.Aktibo = dto.Aktibo.Value;
            }
            if (hashedPin != null)
            {
                langilea.PasahitzaHash = hashedPin;
            }

            await session.UpdateAsync(langilea);
            await tx.CommitAsync();

            return Success(new
            {
                id,
                erabiltzailea = langilea.Erabiltzailea,
                izena = langilea.Izena,
                abizena = langilea.Abizena,
                nanDni = langilea.NanDni,
                emaila = langilea.Emaila,
                rola = langilea.Rola,
                aktibo = langilea.Aktibo,
                txataAktibo = langilea.TxataAktibo,
                pinEguneratua = hashedPin != null
            }, 200, "Langilearen datuak eguneratu dira.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Erabiltzaile-izena, NANa edo emaila jada erregistratuta dago.");
        }
    }

    /// <summary>
    /// Langile baten barne-txat baimena aktibatzen edo desaktibatzen du (Gerente aplikaziotik).
    /// </summary>
    [HttpPut("{id:int}/txata")]
    public async Task<IActionResult> EguneratuTxata(int id, [FromBody] LangileTxataDto dto)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var langilea = await session.GetAsync<Langilea>(id, LockMode.Upgrade);
        if (langilea == null || langilea.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen langilerik.");
        }

        langilea.TxataAktibo = dto.TxataAktibo;
        await session.UpdateAsync(langilea);
        await tx.CommitAsync();

        string egoeraTestua = dto.TxataAktibo ? "aktibatu" : "desaktibatu";
        return Success(new
        {
            id = langilea.Id,
            erabiltzailea = langilea.Erabiltzailea,
            txataAktibo = langilea.TxataAktibo
        }, 200, $"{langilea.Izena} langilearen txata {egoeraTestua} da.");
    }

    /// <summary>
    /// Langile bat aktibatzen edo desaktibatzen du TPV/sistemarako sarbidea kudeatzeko.
    /// </summary>
    [HttpPut("{id:int}/aktibo")]
    public async Task<IActionResult> EguneratuAktibo(int id, [FromBody] LangileAktiboDto dto)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var langilea = await session.GetAsync<Langilea>(id, LockMode.Upgrade);
        if (langilea == null)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen langilerik.");
        }

        langilea.Aktibo = dto.Aktibo;
        if (dto.Aktibo)
        {
            langilea.Ezabatua = false;
            langilea.EzabatzeData = null;
        }
        await session.UpdateAsync(langilea);
        await tx.CommitAsync();

        string egoeraTestua = dto.Aktibo ? "aktibatu" : "desaktibatu";
        return Success(new
        {
            id = langilea.Id,
            erabiltzailea = langilea.Erabiltzailea,
            aktibo = langilea.Aktibo,
            txataAktibo = langilea.TxataAktibo
        }, 200, $"{langilea.Izena} langilea {egoeraTestua} da.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desaktibatu(int id)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var langilea = await session.GetAsync<Langilea>(id, LockMode.Upgrade);
        if (langilea == null || langilea.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen langile aktiborik.");
        }

        langilea.Aktibo = false;
        langilea.Ezabatua = true;
        langilea.EzabatzeData = DateTime.Now;
        await session.UpdateAsync(langilea);
        await tx.CommitAsync();

        return Success(new { id, aktibo = false, ezabatua = true }, 200, "Langilea desgaitu da.");
    }

    /// <summary>
    /// Datu-basean dauden testu lau moduko PIN guztiak BCrypt hash-etara migratzen ditu transakzio bakarrean.
    /// </summary>
    [HttpPost("migratu-hashak")]
    public async Task<IActionResult> MigratuHashak()
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var langileak = await session.Query<Langilea>().ToListAsync();
        int eguneratuak = 0;

        foreach (var l in langileak)
        {
            if (!SegurtasunHelper.IsBcryptHash(l.PasahitzaHash))
            {
                l.PasahitzaHash = SegurtasunHelper.HashPin(l.PasahitzaHash);
                await session.UpdateAsync(l);
                eguneratuak++;
            }
        }

        await tx.CommitAsync();

        return Success(new { mezua = $"{eguneratuak} langileren PINak BCrypt hash-era bihurtu dira.", eguneratuak }, 200, "Migrazioa ondo burutu da.");
    }

    private static bool RolaBaliozkoa(string rola)
        => new[] { "Zerbitzaria", "Sukaldaria", "Barrakoa", "Biltegikoa", "Kudeatzailea", "AdminIT", "Admin" }
            .Contains(rola, StringComparer.Ordinal);
}
