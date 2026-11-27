using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/mahaiak")]
public class MahaiakController : BaseApiKontroladorea
{
    public MahaiakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Mahai guztien zerrenda lortzen du (zerbitzu eta erreserba aktiboen adierazleekin).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Mahaia>();
        if (!barneEzabatuak)
            query = query.Where(m => !m.Ezabatua);

        var mahaiak = await query
            .OrderBy(m => m.Zenbakia)
            .ToListAsync();

        var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
        var eskaeraAktiboMahaiak = (await session.Query<Eskaera>()
            .Where(e => !e.Ezabatua && !itxitakoak.Contains(e.Egoera))
            .Select(e => e.Mahaia.Id)
            .Distinct()
            .ToListAsync()).ToHashSet();

        DateTime gaur = DateTime.Today;
        var erreserbaEgoeraAktiboak = new[] { "Baieztatua", "Eserita" };
        var erreserbaAktiboMahaiak = (await session.Query<Erreserba>()
            .Where(r => !r.Ezabatua && r.Data >= gaur && erreserbaEgoeraAktiboak.Contains(r.Egoera))
            .Select(r => r.Mahaia.Id)
            .Distinct()
            .ToListAsync()).ToHashSet();

        var zerrenda = mahaiak.Select(m => new MahaiaDTO
        {
            Id = m.Id,
            Zenbakia = m.Zenbakia,
            Edukiera = m.Edukiera,
            Kokapena = m.Kokapena,
            Egoera = m.Egoera,
            Zerbitzuan = m.Egoera is "Okupatuta" or "Garbitzen" || eskaeraAktiboMahaiak.Contains(m.Id),
            ErreserbaAktiboa = erreserbaAktiboMahaiak.Contains(m.Id),
            Ezabatua = m.Ezabatua
        }).ToList();

        return SuccessList(zerrenda, 200, "Mahaiak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Data eta txanda jakin baterako libre dauden mahaiak lortzen ditu.
    /// </summary>
    [HttpGet("disponibleak")]
    public async Task<IActionResult> LortuDisponibleak(
        [FromQuery] string data,
        [FromQuery] string txanda,
        [FromQuery] int? pertsonaKopurua = null,
        [FromQuery] string? kokapena = null)
    {
        if (!DateTime.TryParse(data, out DateTime dataParsed))
            return BadRequestError("Data formatua ez da baliozkoa (erabili yyyy-MM-dd).");

        using var session = IrekiSaioa();
        DateTime egunData = dataParsed.Date;

        var okupatutakoMahaiIdak = await session.Query<Erreserba>()
            .Where(r => r.Data == egunData && r.Txanda == txanda && !r.Ezabatua)
            .Select(r => r.Mahaia.Id)
            .Distinct()
            .ToListAsync();

        var query = session.Query<Mahaia>()
            .Where(m => !m.Ezabatua);

        if (okupatutakoMahaiIdak.Count > 0)
            query = query.Where(m => !okupatutakoMahaiIdak.Contains(m.Id));
        if (pertsonaKopurua.HasValue && pertsonaKopurua.Value > 0)
            query = query.Where(m => m.Edukiera >= pertsonaKopurua.Value);
        if (!string.IsNullOrWhiteSpace(kokapena) && kokapena != "Guztiak")
            query = query.Where(m => m.Kokapena == kokapena);

        var libreak = await query
            .OrderBy(m => m.Kokapena)
            .ThenBy(m => m.Zenbakia)
            .ToListAsync();

        var zerrenda = libreak.Select(m => new MahaiaDTO
        {
            Id = m.Id,
            Zenbakia = m.Zenbakia,
            Edukiera = m.Edukiera,
            Kokapena = m.Kokapena,
            Egoera = "Libre",
            Ezabatua = false
        }).ToList();

        return SuccessList(zerrenda, 200, "Mahai libreak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Mahai bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var m = await session.GetAsync<Mahaia>(id);
        if (m == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen mahaia.");

        var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
        bool eskaeraAktiboa = await session.Query<Eskaera>()
            .AnyAsync(e => e.Mahaia.Id == m.Id && !e.Ezabatua && !itxitakoak.Contains(e.Egoera));

        DateTime gaur = DateTime.Today;
        var erreserbaAktiboak = new[] { "Baieztatua", "Eserita" };
        bool erreserbaAktiboa = await session.Query<Erreserba>()
            .AnyAsync(r => r.Mahaia.Id == m.Id && !r.Ezabatua && r.Data >= gaur && erreserbaAktiboak.Contains(r.Egoera));

        var dto = new MahaiaDTO
        {
            Id = m.Id,
            Zenbakia = m.Zenbakia,
            Edukiera = m.Edukiera,
            Kokapena = m.Kokapena,
            Egoera = m.Egoera,
            Zerbitzuan = m.Egoera is "Okupatuta" or "Garbitzen" || eskaeraAktiboa,
            ErreserbaAktiboa = erreserbaAktiboa,
            Ezabatua = m.Ezabatua
        };

        return Success(dto, 200, "Mahaia ondo aurkitu da.");
    }

    /// <summary>
    /// Mahai berria sortzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] MahaiaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        using var session = IrekiSaioa();
        IActionResult? lockError = await EgiaztatuZerbitzuaItxitaDagoelaAsync(session);
        if (lockError != null)
            return lockError;

        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
            var mahaia = new Mahaia
            {
                Zenbakia = dto.Zenbakia,
                Edukiera = dto.Edukiera,
                Kokapena = dto.Kokapena,
                Egoera = "Libre",
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(mahaia);
            await tx.CommitAsync();

            var emaitza = new MahaiaDTO
            {
                Id = mahaia.Id,
                Zenbakia = mahaia.Zenbakia,
                Edukiera = mahaia.Edukiera,
                Kokapena = mahaia.Kokapena,
                Egoera = "Libre",
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Mahaia arrakastaz sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da {dto.Zenbakia} zenbakia duen mahaia.");
        }
    }

    /// <summary>
    /// Mahaiaren xehetasunak aldatzen ditu blokeo pesimistan (LockMode.Upgrade), mahaia libre eta erreserbarik gabe dagoenean.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] MahaiaEguneratuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Mahaiaren datuak ez dira baliozkoak.");

        var kokapenBaliozkoak = new[] { "Barran", "Barra", "Jantoki Nagusia", "Terraza", "VIP" };
        if (!kokapenBaliozkoak.Contains(dto.Kokapena, StringComparer.Ordinal))
            return BadRequestError($"Kokapena ez da baliozkoa. Balio onargarriak: {string.Join(", ", kokapenBaliozkoak)}");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var mahaia = await session.GetAsync<Mahaia>(id, LockMode.Upgrade);
            if (mahaia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen mahaia.");
            }
            if (mahaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Desgaitutako mahaia ezin da editatu.");
            }

            var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
            bool baduEskaera = await session.Query<Eskaera>()
                .AnyAsync(e => e.Mahaia.Id == id && !e.Ezabatua && !itxitakoak.Contains(e.Egoera));
            if (baduEskaera)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Mahaia ezin da editatu eskaera aktibo bat duen bitartean.");
            }

            DateTime gaur = DateTime.Today;
            var erreserbaAktiboak = new[] { "Baieztatua", "Eserita" };
            bool baduErreserba = await session.Query<Erreserba>()
                .AnyAsync(r => r.Mahaia.Id == id && !r.Ezabatua && r.Data >= gaur && erreserbaAktiboak.Contains(r.Egoera));
            if (baduErreserba)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Mahaia ezin da editatu gaurko edo etorkizuneko erreserba aktiboak dituen bitartean.");
            }

            if (!string.Equals(mahaia.Egoera, "Libre", StringComparison.Ordinal))
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"Mahaia '{mahaia.Egoera}' egoeran dago. Libre dagoenean bakarrik editatu daiteke.");
            }

            mahaia.Zenbakia = dto.Zenbakia;
            mahaia.Edukiera = dto.Edukiera;
            mahaia.Kokapena = dto.Kokapena;
            await session.UpdateAsync(mahaia);
            await tx.CommitAsync();

            return Success(new
            {
                id,
                zenbakia = mahaia.Zenbakia,
                edukiera = mahaia.Edukiera,
                kokapena = mahaia.Kokapena,
                egoera = mahaia.Egoera
            }, 200, "Mahaiaren datuak eguneratu dira.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da {dto.Zenbakia} zenbakia duen mahaia.");
        }
    }

    /// <summary>
    /// Mahaiaren egoera eguneratzen du (Libre, Okupatuta, Erreserbatuta, Garbitzen).
    /// </summary>
    [HttpPut("{id:int}/egoera")]
    public async Task<IActionResult> EguneratuEgoera(int id, [FromBody] MahaiaEgoeraEguneratuDto dto)
    {
        var baliozkoak = new[] { "Libre", "Okupatuta", "Erreserbatuta", "Garbitzen" };
        if (!baliozkoak.Contains(dto.Egoera))
            return BadRequestError($"Egoera ez da baliozkoa. Balio onargarriak: {string.Join(", ", baliozkoak)}");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var mahaia = await session.GetAsync<Mahaia>(id, LockMode.Upgrade);
        if (mahaia == null || mahaia.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen mahai aktiborik.");
        }

        if (dto.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase) ||
            dto.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase))
        {
            var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
            long platerKop = await session.Query<EskaeraLerroa>()
                .LongCountAsync(el =>
                    el.Eskaera.Mahaia.Id == id &&
                    !el.Eskaera.Ezabatua &&
                    !itxitakoak.Contains(el.Eskaera.Egoera));

            if (platerKop > 0)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"Mahaia ezin da '{dto.Egoera}' jarri {platerKop} produktu daudelako kobratzeko eskari aktiboan. Kontua kobratu edo produktuak ezabatu behar dira lehenik.");
            }
        }

        mahaia.Egoera = dto.Egoera;
        await session.UpdateAsync(mahaia);
        await tx.CommitAsync();

        return Success(new { id, egoera = dto.Egoera }, 200, "Mahaiaren egoera eguneratu da.");
    }

    /// <summary>
    /// Mahaia ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        using var session = IrekiSaioa();
        IActionResult? lockError = await EgiaztatuZerbitzuaItxitaDagoelaAsync(session);
        if (lockError != null)
            return lockError;

        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        if (hard)
        {
            try
            {
                var mahaia = await session.GetAsync<Mahaia>(id, LockMode.Upgrade);
                if (mahaia == null)
                {
                    await RollbackSeguruaAsync(tx);
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen mahaia.");
                }

                await session.DeleteAsync(mahaia);
                await tx.CommitAsync();

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Mahaia behin betiko ezabatu da.");
            }
            catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1451)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Mahaia ezin da behin betiko ezabatu, erlazionatutako erreserbak edo eskaerak dituelako.");
            }
        }
        else
        {
            var mahaia = await session.GetAsync<Mahaia>(id, LockMode.Upgrade);
            if (mahaia == null || mahaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen mahai aktiborik.");
            }

            mahaia.Ezabatua = true;
            mahaia.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(mahaia);
            await tx.CommitAsync();

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Mahaia desgaitu da (soft delete).");
        }
    }

    /// <summary>
    /// Desgaitutako mahaia berreskuratzen du (Restore).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        using var session = IrekiSaioa();
        IActionResult? lockError = await EgiaztatuZerbitzuaItxitaDagoelaAsync(session);
        if (lockError != null)
            return lockError;

        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var mahaia = await session.GetAsync<Mahaia>(id, LockMode.Upgrade);
        if (mahaia == null || !mahaia.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen mahairik.");
        }

        mahaia.Ezabatua = false;
        mahaia.EzabatzeData = null;
        await session.UpdateAsync(mahaia);
        await tx.CommitAsync();

        return Success(new { id, berreskuratua = true }, 200, "Mahaia berreskuratu da.");
    }
}
