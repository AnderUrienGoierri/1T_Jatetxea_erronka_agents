using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/platerak")]
public class PlaterakController : BaseApiKontroladorea
{
    public PlaterakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Plater guztien zerrenda lortzen du (aukeran kategoriaren eta ezabatuen arabera iragazita).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] string? kategoria = null, [FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Platera>();

        if (!barneEzabatuak) query = query.Where(p => !p.Ezabatua);
        if (!string.IsNullOrWhiteSpace(kategoria)) query = query.Where(p => p.Kategoria == kategoria);

        var platerak = await query
            .OrderBy(p => p.Kategoria)
            .ThenBy(p => p.Izena)
            .ToListAsync();

        var zerrenda = platerak.Select(p => new PlateraDTO
        {
            Id = p.Id,
            Izena = p.Izena,
            Deskribapena = p.Deskribapena,
            Kategoria = p.Kategoria,
            Prezioa = p.Prezioa,
            Alergenoak = p.Alergenoak,
            IrudiErreferentzia = p.IrudiErreferentzia,
            Aktibo = p.Aktibo,
            Ezabatua = p.Ezabatua
        }).ToList();

        return SuccessList(zerrenda, 200, "Platerak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Plateren erabilgarritasuna lortzen du (v_plateren_stock_ahalmena ikuspegitik NHibernate bidez).
    /// </summary>
    [HttpGet("erabilgarritasuna")]
    public async Task<IActionResult> LortuErabilgarritasuna()
    {
        using var session = IrekiSaioa();
        var ahalmenak = await session.Query<PlaterStockAhalmena>()
            .OrderByDescending(x => x.EginDaitezkeenPlaterak)
            .ToListAsync();

        var zerrenda = ahalmenak.Select(x => new PlaterErabilgarritasunaDTO
        {
            PlateraId = x.PlateraId,
            PlaterIzena = x.PlaterIzena,
            EginDaitezkeenPlaterak = x.EginDaitezkeenPlaterak
        }).ToList();

        return SuccessList(zerrenda, 200, "Plateren erabilgarritasuna kalkulatu da.");
    }

    /// <summary>
    /// Plater bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var p = await session.GetAsync<Platera>(id);
        if (p == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen platerik.");

        var dto = new PlateraDTO
        {
            Id = p.Id,
            Izena = p.Izena,
            Deskribapena = p.Deskribapena,
            Kategoria = p.Kategoria,
            Prezioa = p.Prezioa,
            Alergenoak = p.Alergenoak,
            IrudiErreferentzia = p.IrudiErreferentzia,
            Aktibo = p.Aktibo,
            Ezabatua = p.Ezabatua
        };

        return Success(dto, 200, "Platera ondo aurkitu da.");
    }

    /// <summary>
    /// Plater baten errezeta eta osagaien proportzioak (eskandalloa) lortzen ditu (Fetch bidez).
    /// </summary>
    [HttpGet("{id:int}/eskandalloa")]
    public async Task<IActionResult> LortuEskandalloa(int id)
    {
        using var session = IrekiSaioa();
        var platera = await session.GetAsync<Platera>(id);
        if (platera == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen platerik.");

        var osagaiLerroak = await session.Query<PlaterOsagaia>()
            .Fetch(po => po.Lehengaia)
            .Where(po => po.PlateraId == id && !po.Lehengaia!.Ezabatua)
            .ToListAsync();

        var osagaiak = osagaiLerroak.Select(po => new PlaterOsagaiaDto
        {
            LehengaiaId = po.LehengaiaId,
            LehengaiaIzena = po.Lehengaia!.Izena,
            Kantitatea = po.Kantitatea,
            Unitatea = po.Lehengaia.Unitatea
        }).ToList();

        var emaitza = new PlaterEskandalloaDTO
        {
            PlateraId = platera.Id,
            PlaterIzena = platera.Izena,
            Prezioa = platera.Prezioa,
            Osagaiak = osagaiak
        };

        return Success(emaitza, 200, "Plateraren eskandalloa ondo berreskuratu da.");
    }

    /// <summary>
    /// Plater berria sortzen du eta errezetako osagaiak transakzio bakarrean erlazionatzen ditu.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] PlateraSortuDto dto)
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
            var platera = new Platera
            {
                Izena = dto.Izena.Trim(),
                Deskribapena = dto.Deskribapena,
                Kategoria = dto.Kategoria,
                Prezioa = dto.Prezioa,
                Alergenoak = dto.Alergenoak,
                Aktibo = true,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(platera);

            if (dto.Osagaiak != null && dto.Osagaiak.Count > 0)
            {
                foreach (var osagai in dto.Osagaiak)
                {
                    var po = new PlaterOsagaia
                    {
                        PlateraId = platera.Id,
                        LehengaiaId = osagai.LehengaiaId,
                        Kantitatea = osagai.Kantitatea
                    };
                    await session.SaveAsync(po);
                }
            }

            await tx.CommitAsync();

            var emaitza = new PlateraDTO
            {
                Id = platera.Id,
                Izena = platera.Izena,
                Deskribapena = platera.Deskribapena,
                Kategoria = platera.Kategoria,
                Prezioa = platera.Prezioa,
                Alergenoak = platera.Alergenoak,
                Aktibo = true,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Platera arrakastaz sortu da.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea platera sortzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Plateraren datuak eta errezeta transakzio pesimistan (LockMode.Upgrade) eguneratzen ditu.
    /// Mahai aktiboak, platera eta osagaiak (ID gorakorrean) blokeatzen ditu deadlock-ak saihesteko.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] PlateraEguneratuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Plateraren datuak ez dira baliozkoak.");
        if (dto.Osagaiak?.GroupBy(osagai => osagai.LehengaiaId).Any(taldea => taldea.Count() > 1) == true)
            return BadRequestError("Errezetak ezin du lehengai bera behin baino gehiagotan izan.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Blokeatu mahai aktiboak zerbitzua ireki ez dadin edizioa amaitu arte
            await session.Query<Mahaia>()
                .Where(m => !m.Ezabatua)
                .OrderBy(m => m.Id)
                .WithLock(LockMode.Upgrade)
                .ToListAsync();

            ServiceStatusDto serviceStatus = await LortuZerbitzuEgoeraAsync(session);
            if (serviceStatus.ServiceActive)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError(
                    $"Zerbitzua aktibo dago ({serviceStatus.OpenTables} mahai). Menuaren konfigurazioa zerbitzua itxita dagoenean bakarrik alda daiteke.");
            }

            // 2. Blokeatu platera
            var platera = await session.GetAsync<Platera>(id, LockMode.Upgrade);
            if (platera == null || platera.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen plater aktiborik.");
            }

            // 3. Blokeatu errezetako lehengaiak ID gorakorrean (deadlock prebentzioa)
            var osagaiak = (dto.Osagaiak ?? new List<PlaterOsagaiaDto>())
                .OrderBy(o => o.LehengaiaId)
                .ToList();

            foreach (var osagai in osagaiak)
            {
                var lehengaia = await session.GetAsync<Lehengaia>(osagai.LehengaiaId, LockMode.Upgrade);
                if (lehengaia == null || lehengaia.Ezabatua)
                {
                    await RollbackSeguruaAsync(tx);
                    return ConflictError($"Ezin da errezeta eguneratu: {osagai.LehengaiaId} ID-a duen lehengaia ez dago aktibo.");
                }
            }

            // 4. Eguneratu plateraren datuak
            platera.Izena = dto.Izena.Trim();
            platera.Deskribapena = dto.Deskribapena;
            platera.Kategoria = dto.Kategoria;
            platera.Prezioa = dto.Prezioa;
            platera.Alergenoak = dto.Alergenoak;
            await session.UpdateAsync(platera);

            // 5. Ordeztu errezetako osagaiak (ezabatu zaharrak, flush, eta gorde berriak)
            var zaharrak = await session.Query<PlaterOsagaia>()
                .Where(po => po.PlateraId == id)
                .ToListAsync();

            foreach (var zaharra in zaharrak)
            {
                await session.DeleteAsync(zaharra);
            }
            await session.FlushAsync();

            foreach (var osagai in osagaiak)
            {
                await session.SaveAsync(new PlaterOsagaia
                {
                    PlateraId = id,
                    LehengaiaId = osagai.LehengaiaId,
                    Kantitatea = osagai.Kantitatea
                });
            }

            await tx.CommitAsync();

            return Success(new PlateraDTO
            {
                Id = platera.Id,
                Izena = platera.Izena,
                Deskribapena = platera.Deskribapena,
                Kategoria = platera.Kategoria,
                Prezioa = platera.Prezioa,
                Alergenoak = platera.Alergenoak,
                Aktibo = platera.Aktibo,
                Ezabatua = false
            }, 200, "Plateraren datuak eta errezeta eguneratu dira.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number is 1062 or 1452)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Ezin izan da platera eguneratu datu-baseko erlazio edo balio errepikatuengatik.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da platera eguneratzean. Mesedez, saiatu berriro.");
        }
    }

    /// <summary>
    /// Platera ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
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
                var platera = await session.GetAsync<Platera>(id, LockMode.Upgrade);
                if (platera == null)
                {
                    await RollbackSeguruaAsync(tx);
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen platerik.");
                }

                await session.DeleteAsync(platera);
                await tx.CommitAsync();

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Platera behin betiko ezabatu da.");
            }
            catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1451)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Platera ezin da behin betiko ezabatu, eskaera lerroetan edo errezetetan erabilia dagoelako.");
            }
        }
        else
        {
            var platera = await session.GetAsync<Platera>(id, LockMode.Upgrade);
            if (platera == null || platera.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen plater aktiborik.");
            }

            platera.Ezabatua = true;
            platera.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(platera);
            await tx.CommitAsync();

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Platera desgaitu da (soft delete).");
        }
    }

    /// <summary>
    /// Desgaitutako platera berreskuratzen du (Restore).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        using var session = IrekiSaioa();
        IActionResult? lockError = await EgiaztatuZerbitzuaItxitaDagoelaAsync(session);
        if (lockError != null)
            return lockError;

        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var platera = await session.GetAsync<Platera>(id, LockMode.Upgrade);
        if (platera == null || !platera.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen platerik.");
        }

        platera.Ezabatua = false;
        platera.EzabatzeData = null;
        await session.UpdateAsync(platera);
        await tx.CommitAsync();

        return Success(new { id, berreskuratua = true }, 200, "Platera berreskuratu da.");
    }
}
