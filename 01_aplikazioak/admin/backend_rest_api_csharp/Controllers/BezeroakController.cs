using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/bezeroak")]
public class BezeroakController : BaseApiKontroladorea
{
    public BezeroakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Bezero guztien zerrenda lortzen du (NHibernate LINQ).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Bezeroa>();
        if (!barneEzabatuak)
            query = query.Where(b => !b.Ezabatua);

        var bezeroak = await query
            .OrderBy(b => b.Abizena)
            .ThenBy(b => b.Izena)
            .ToListAsync();

        var zerrenda = bezeroak.Select(b => new BezeroaDTO
        {
            Id = b.Id,
            Izena = b.Izena,
            Abizena = b.Abizena,
            Telefonoa = b.Telefonoa,
            Emaila = b.Emaila,
            Ezabatua = b.Ezabatua
        }).ToList();

        return SuccessList(zerrenda, 200, "Bezeroak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Bezero bakarra lortzen du ID bidez (session.GetAsync).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var b = await session.GetAsync<Bezeroa>(id);
        if (b == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen bezerorik.");

        var dto = new BezeroaDTO
        {
            Id = b.Id,
            Izena = b.Izena,
            Abizena = b.Abizena,
            Telefonoa = b.Telefonoa,
            Emaila = b.Emaila,
            Ezabatua = b.Ezabatua
        };

        return Success(dto, 200, "Bezeroa ondo aurkitu da.");
    }

    /// <summary>
    /// Bezero berria sortzen du NHibernate transakzioan.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] BezeroaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var bezeroa = new Bezeroa
            {
                Izena = dto.Izena.Trim(),
                Abizena = dto.Abizena.Trim(),
                Telefonoa = dto.Telefonoa.Trim(),
                Emaila = dto.Emaila.Trim().ToLower(),
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(bezeroa);
            await tx.CommitAsync();

            var emaitza = new BezeroaDTO
            {
                Id = bezeroa.Id,
                Izena = bezeroa.Izena,
                Abizena = bezeroa.Abizena,
                Telefonoa = bezeroa.Telefonoa,
                Emaila = bezeroa.Emaila,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Bezeroa arrakastaz sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da '{dto.Emaila}' emaila duen bezerorik.");
        }
    }

    /// <summary>
    /// Bezeroaren datuak eguneratzen ditu (NHibernate dirty checking).
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] BezeroaSortuDto dto)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var bezeroa = await session.GetAsync<Bezeroa>(id, LockMode.Upgrade);
            if (bezeroa == null || bezeroa.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen bezerorik.");
            }

            bezeroa.Izena = dto.Izena.Trim();
            bezeroa.Abizena = dto.Abizena.Trim();
            bezeroa.Telefonoa = dto.Telefonoa.Trim();
            bezeroa.Emaila = dto.Emaila.Trim().ToLower();

            await session.UpdateAsync(bezeroa);
            await tx.CommitAsync();

            return Success(new { id, izena = dto.Izena, emaila = dto.Emaila }, 200, "Bezeroaren datuak ondo eguneratu dira.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da beste bezero bat '{dto.Emaila}' emailarekin.");
        }
    }

    /// <summary>
    /// Bezeroa ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
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
                var bezeroa = await session.GetAsync<Bezeroa>(id, LockMode.Upgrade);
                if (bezeroa == null)
                {
                    await RollbackSeguruaAsync(tx);
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen bezerorik.");
                }

                await session.DeleteAsync(bezeroa);
                await tx.CommitAsync();

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Bezeroa behin betiko ezabatu da.");
            }
            catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1451)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Bezeroa ezin da behin betiko ezabatu, erlazionatutako erreserbak edo iritziak dituelako.");
            }
        }
        else
        {
            var bezeroa = await session.GetAsync<Bezeroa>(id, LockMode.Upgrade);
            if (bezeroa == null || bezeroa.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen bezero aktiborik.");
            }

            bezeroa.Ezabatua = true;
            bezeroa.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(bezeroa);
            await tx.CommitAsync();

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Bezeroa desgaitu da (soft delete).");
        }
    }
}
