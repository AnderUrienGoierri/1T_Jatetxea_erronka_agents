using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/lehengaiak")]
public class LehengaiakController : BaseApiKontroladorea
{
    public LehengaiakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Lehengai guztien zerrenda lortzen du uneko stockarekin.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Lehengaia>();
        if (!barneEzabatuak)
            query = query.Where(l => !l.Ezabatua);

        var lehengaiak = await query
            .OrderBy(l => l.Izena)
            .ToListAsync();

        var zerrenda = lehengaiak.Select(l => new LehengaiaDTO
        {
            Id = l.Id,
            Izena = l.Izena,
            Unitatea = l.Unitatea,
            UnekoStocka = l.UnekoStocka,
            GutxienekoStocka = l.GutxienekoStocka,
            KostuUnitarioa = l.KostuUnitarioa,
            Hornitzailea = l.Hornitzailea,
            Ezabatua = l.Ezabatua
        }).ToList();

        return SuccessList(zerrenda, 200, "Lehengaiak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Lehengai bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var l = await session.GetAsync<Lehengaia>(id);
        if (l == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengaia.");

        var dto = new LehengaiaDTO
        {
            Id = l.Id,
            Izena = l.Izena,
            Unitatea = l.Unitatea,
            UnekoStocka = l.UnekoStocka,
            GutxienekoStocka = l.GutxienekoStocka,
            KostuUnitarioa = l.KostuUnitarioa,
            Hornitzailea = l.Hornitzailea,
            Ezabatua = l.Ezabatua
        };

        return Success(dto, 200, "Lehengaia ondo aurkitu da.");
    }

    /// <summary>
    /// Erosketa-zerrenda: gutxieneko stocketik behera dauden lehengaiak (v_erosi_beharreko_lehengaiak ikuspegitik).
    /// </summary>
    [HttpGet("alertak")]
    public async Task<IActionResult> LortuAlertak()
    {
        using var session = IrekiSaioa();
        var alertak = await session.Query<ErosiBeharrekoLehengaia>()
            .OrderBy(a => a.UnekoStocka)
            .ToListAsync();

        var emaitza = alertak.Select(a => new LehengaiaAlertaDTO
        {
            LehengaiaId = a.LehengaiaId,
            Izena = a.Izena,
            Unitatea = a.Unitatea,
            UnekoStocka = a.UnekoStocka,
            GutxienekoStocka = a.GutxienekoStocka,
            ProposatutakoErosketa = a.ProposatutakoErosketa,
            Hornitzailea = a.Hornitzailea
        }).ToList();

        return SuccessList(emaitza, 200, "Stock alertak (erosketa-zerrenda) berreskuratu dira.");
    }

    /// <summary>
    /// Lehengai berria sortzen du transakzio atomikoan (hasierako stocka > 0 bada, auditoria ere sortuz).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] LehengaiaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");
        if (!UnitateBaliozkoa(dto.Unitatea))
            return BadRequestError("Unitatea ez da baliozkoa. Erabili kg, g, l, ml edo unitatea.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var lehengaia = new Lehengaia
            {
                Izena = dto.Izena.Trim(),
                Unitatea = dto.Unitatea,
                UnekoStocka = dto.UnekoStocka,
                GutxienekoStocka = dto.GutxienekoStocka,
                KostuUnitarioa = dto.KostuUnitarioa,
                Hornitzailea = dto.Hornitzailea?.Trim(),
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(lehengaia);

            if (dto.UnekoStocka > 0)
            {
                var mugimendua = new StockMugimendua
                {
                    Lehengaia = lehengaia,
                    Mota = "Egokitzapena_Inbentarioa",
                    Kantitatea = dto.UnekoStocka,
                    Oharrak = "Hasierako inbentarioa",
                    Data = DateTime.Now
                };
                await session.SaveAsync(mugimendua);
            }

            await tx.CommitAsync();

            var emaitza = new LehengaiaDTO
            {
                Id = lehengaia.Id,
                Izena = lehengaia.Izena,
                Unitatea = lehengaia.Unitatea,
                UnekoStocka = lehengaia.UnekoStocka,
                GutxienekoStocka = lehengaia.GutxienekoStocka,
                KostuUnitarioa = lehengaia.KostuUnitarioa,
                Hornitzailea = lehengaia.Hornitzailea,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Lehengaia arrakastaz sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da '{dto.Izena}' izena duen lehengaia.");
        }
    }

    /// <summary>
    /// Lehengaiaren datu nagusiak eguneratzen ditu blokeo pesimistan (LockMode.Upgrade), stocka aldatu gabe.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] LehengaiaEguneratuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");
        if (!UnitateBaliozkoa(dto.Unitatea))
            return BadRequestError("Unitatea ez da baliozkoa. Erabili kg, g, l, ml edo unitatea.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var lehengaia = await session.GetAsync<Lehengaia>(id, LockMode.Upgrade);
            if (lehengaia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengaia.");
            }
            if (lehengaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Desgaitutako lehengai bat ezin da editatu.");
            }

            if (!string.Equals(lehengaia.Unitatea, dto.Unitatea, StringComparison.Ordinal))
            {
                bool baduHistoria = await session.Query<StockMugimendua>().AnyAsync(sm => sm.Lehengaia.Id == id)
                                 || await session.Query<PlaterOsagaia>().AnyAsync(po => po.LehengaiaId == id);
                if (baduHistoria)
                {
                    await RollbackSeguruaAsync(tx);
                    return ConflictError("Unitatea ezin da aldatu stock-historia edo errezeta bati lotuta dagoenean.");
                }
            }

            lehengaia.Izena = dto.Izena.Trim();
            lehengaia.Unitatea = dto.Unitatea;
            lehengaia.GutxienekoStocka = dto.GutxienekoStocka;
            lehengaia.KostuUnitarioa = dto.KostuUnitarioa;
            lehengaia.Hornitzailea = dto.Hornitzailea?.Trim();

            await session.UpdateAsync(lehengaia);
            await tx.CommitAsync();

            return Success(new
            {
                id,
                izena = lehengaia.Izena,
                unitatea = lehengaia.Unitatea,
                gutxienekoStocka = lehengaia.GutxienekoStocka,
                kostuUnitarioa = lehengaia.KostuUnitarioa,
                hornitzailea = lehengaia.Hornitzailea
            }, 200, "Lehengaiaren datuak eguneratu dira; stocka aldatu gabe mantendu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da '{dto.Izena}' izena duen lehengaia.");
        }
    }

    /// <summary>
    /// Hornitzailearen albarana erregistratu eta lehengaiaren stocka gehitu transakzio pesimistan (LockMode.Upgrade).
    /// </summary>
    [HttpPost("sarrera")]
    public async Task<IActionResult> ErregistratuSarrera([FromBody] AlbaranaSarreraDto dto)
    {
        if (dto.Kantitatea <= 0)
            return BadRequestError("Sarrerako kantitateak 0 baino handiagoa izan behar du.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Lehengaia blokeatu (SELECT ... FOR UPDATE) eta egiaztatu
            var lehengaia = await session.GetAsync<Lehengaia>(dto.LehengaiaId, LockMode.Upgrade);
            if (lehengaia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {dto.LehengaiaId} ID-a duen lehengaia.");
            }

            if (lehengaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Lehengaia desgaituta dago. Berreskuratu lehenago.");
            }

            decimal aurrekoStocka = lehengaia.UnekoStocka;
            decimal stockBerria = aurrekoStocka + dto.Kantitatea;

            // 2. Stocka eguneratu
            lehengaia.UnekoStocka = stockBerria;
            await session.UpdateAsync(lehengaia);

            // 3. Auditoria erregistratu stock_mugimenduak taulan
            string oharra = dto.Oharrak ?? (dto.Hornitzailea != null ? $"Hornitzailea: {dto.Hornitzailea}" : "Albaran sarrera");
            var mugimendua = new StockMugimendua
            {
                Lehengaia = lehengaia,
                Mota = "Sarrera_Albarana",
                Kantitatea = dto.Kantitatea,
                Oharrak = oharra,
                Data = DateTime.Now
            };
            await session.SaveAsync(mugimendua);

            await tx.CommitAsync();

            return Success(new
            {
                lehengaiaId = lehengaia.Id,
                izena = lehengaia.Izena,
                aurrekoStocka,
                gehitutakoKantitatea = dto.Kantitatea,
                unekoStockBerria = stockBerria,
                unitatea = lehengaia.Unitatea
            }, 200, "Albarana ondo erregistratu da eta stocka eguneratu da.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea albarana erregistratzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Lehengaia ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        if (hard)
        {
            try
            {
                var lehengaia = await session.GetAsync<Lehengaia>(id, LockMode.Upgrade);
                if (lehengaia == null)
                {
                    await RollbackSeguruaAsync(tx);
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengaia.");
                }

                await session.DeleteAsync(lehengaia);
                await tx.CommitAsync();

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Lehengaia behin betiko ezabatu da.");
            }
            catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1451)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Lehengaia ezin da behin betiko ezabatu, plateren errezetetan edo stock mugimenduetan erabilia dagoelako.");
            }
        }
        else
        {
            var lehengaia = await session.GetAsync<Lehengaia>(id, LockMode.Upgrade);
            if (lehengaia == null || lehengaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengai aktiborik.");
            }

            bool errezetanErabilia = await session.Query<PlaterOsagaia>()
                .AnyAsync(po => po.LehengaiaId == id && !po.Platera!.Ezabatua);
            if (errezetanErabilia)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Lehengaia ezin da desgaitu plater aktibo baten errezetan erabiltzen den bitartean.");
            }

            lehengaia.Ezabatua = true;
            lehengaia.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(lehengaia);
            await tx.CommitAsync();

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Lehengaia desgaitu da (soft delete).");
        }
    }

    /// <summary>
    /// Desgaitutako lehengaia berreskuratzen du (Restore).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var lehengaia = await session.GetAsync<Lehengaia>(id, LockMode.Upgrade);
        if (lehengaia == null || !lehengaia.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen lehengairik.");
        }

        lehengaia.Ezabatua = false;
        lehengaia.EzabatzeData = null;
        await session.UpdateAsync(lehengaia);
        await tx.CommitAsync();

        return Success(new { id, berreskuratua = true }, 200, "Lehengaia berreskuratu da.");
    }

    private static bool UnitateBaliozkoa(string unitatea)
        => new[] { "kg", "g", "l", "ml", "unitatea" }.Contains(unitatea, StringComparer.Ordinal);
}
