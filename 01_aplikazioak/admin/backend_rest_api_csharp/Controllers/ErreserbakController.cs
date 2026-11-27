using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/erreserbak")]
public class ErreserbakController : BaseApiKontroladorea
{
    public ErreserbakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Erreserben zerrenda lortzen du (Fetch bidez Bezeroa eta Mahaia kargatuz N+1 saihesteko).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak(
        [FromQuery] string? data = null,
        [FromQuery] string? txanda = null,
        [FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        IQueryable<Erreserba> query = session.Query<Erreserba>();

        if (!barneEzabatuak)
            query = query.Where(e => !e.Ezabatua);
        if (!string.IsNullOrWhiteSpace(data) && DateTime.TryParse(data, out DateTime egunData))
            query = query.Where(e => e.Data == egunData.Date);
        if (!string.IsNullOrWhiteSpace(txanda))
            query = query.Where(e => e.Txanda == txanda);

        var erreserbak = await query
            .Fetch(e => e.Bezeroa)
            .Fetch(e => e.Mahaia)
            .OrderByDescending(e => e.Data)
            .ThenBy(e => e.Txanda)
            .ThenBy(e => e.Mahaia.Zenbakia)
            .ToListAsync();

        var zerrenda = erreserbak.Select(MapErreserbaDto).ToList();
        return SuccessList(zerrenda, 200, "Erreserbak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Erreserba bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var erreserba = await session.Query<Erreserba>()
            .Fetch(e => e.Bezeroa)
            .Fetch(e => e.Mahaia)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (erreserba == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen erreserbarik.");

        return Success(MapErreserbaDto(erreserba), 200, "Erreserba ondo aurkitu da.");
    }

    /// <summary>
    /// Erreserba berria egiten du transakzio pesimistan (NHibernate LockMode.Upgrade -> SELECT ... FOR UPDATE).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] ErreserbaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        if (!DateTime.TryParse(dto.Data, out DateTime dataParsed))
            return BadRequestError("Data formatua ez da baliozkoa (erabili yyyy-MM-dd).");

        if (dataParsed.DayOfWeek == DayOfWeek.Monday)
            return BadRequestError("Astelehenetan jatetxea itxita dago (atseden eguna). Mesedez, aukeratu beste egun bat.");

        DateTime erreserbaData = dataParsed.Date;

        using var session = IrekiSaioa();

        // 1. Mahaia zehaztu (ID bidez, zenbaki bidez edo automatikoki esleituta)
        int mahaiaId = dto.MahaiaId ?? 0;
        if (mahaiaId <= 0 && dto.MahaiZenbakia.HasValue && dto.MahaiZenbakia.Value > 0)
        {
            var mahaiBilatua = await session.Query<Mahaia>()
                .FirstOrDefaultAsync(m => m.Zenbakia == dto.MahaiZenbakia.Value && !m.Ezabatua);
            if (mahaiBilatua != null) mahaiaId = mahaiBilatua.Id;
        }

        if (mahaiaId <= 0)
        {
            var okupatutakoak = await session.Query<Erreserba>()
                .Where(r => r.Data == erreserbaData && r.Txanda == dto.Txanda && !r.Ezabatua)
                .Select(r => r.Mahaia.Id)
                .ToListAsync();

            var libreQuery = session.Query<Mahaia>()
                .Where(m => !m.Ezabatua && m.Edukiera >= dto.PertsonaKopurua);
            if (okupatutakoak.Count > 0)
                libreQuery = libreQuery.Where(m => !okupatutakoak.Contains(m.Id));

            var autoMahaia = await libreQuery
                .OrderBy(m => m.Edukiera)
                .ThenBy(m => m.Zenbakia)
                .FirstOrDefaultAsync();

            if (autoMahaia != null)
                mahaiaId = autoMahaia.Id;
            else
                return ConflictError("Ez dago mahai librerik aukeratutako data eta txandarako.");
        }

        if (mahaiaId <= 0)
            return BadRequestError("Mahaia hautatzea beharrezkoa da.");

        // 2. Transakzio pesimista hasi (IsolationLevel.ReadCommitted + LockMode.Upgrade)
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // Bezeroa zehaztu edo automatikoki erregistratu
            Bezeroa? bezeroa = null;
            if (dto.BezeroaId.HasValue && dto.BezeroaId.Value > 0)
            {
                bezeroa = await session.GetAsync<Bezeroa>(dto.BezeroaId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(dto.BezeroEmaila))
            {
                string emailNorm = dto.BezeroEmaila.Trim().ToLower();
                bezeroa = await session.Query<Bezeroa>()
                    .FirstOrDefaultAsync(b => b.Emaila == emailNorm);

                if (bezeroa == null)
                {
                    bezeroa = new Bezeroa
                    {
                        Izena = string.IsNullOrWhiteSpace(dto.BezeroIzena) ? "Bezeroa" : dto.BezeroIzena.Trim(),
                        Abizena = string.IsNullOrWhiteSpace(dto.BezeroAbizena) ? "-" : dto.BezeroAbizena.Trim(),
                        Telefonoa = string.IsNullOrWhiteSpace(dto.BezeroTelefonoa) ? "-" : dto.BezeroTelefonoa.Trim(),
                        Emaila = emailNorm,
                        Ezabatua = false,
                        SortzeData = DateTime.Now
                    };
                    await session.SaveAsync(bezeroa);
                }
            }

            if (bezeroa == null)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError("Bezeroa zehaztu behar da (BezeroaId edo BezeroEmaila bidaliz).");
            }

            // Mahaia blokeatu (SELECT ... FROM mahaiak WHERE id = ? FOR UPDATE)
            var mahaia = await session.GetAsync<Mahaia>(mahaiaId, LockMode.Upgrade);
            if (mahaia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {mahaiaId} ID-a duen mahaia.");
            }

            if (mahaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Mahaia desgaituta dago. Ezin da erreserbarik egin bertan.");
            }

            if (dto.PertsonaKopurua > mahaia.Edukiera)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError($"Mahaiaren aforoa gainditu da: {mahaia.Zenbakia}. mahaiak gehienez {mahaia.Edukiera} lagunentzako lekua du ({dto.PertsonaKopurua} pertsona eskatu dira).");
            }

            // Data eta txanda berean erreserbarik dagoen egiaztatu (WithLock(LockMode.Upgrade))
            var dagoenErreserba = await session.Query<Erreserba>()
                .Where(r => r.Mahaia.Id == mahaiaId && r.Data == erreserbaData && r.Txanda == dto.Txanda && !r.Ezabatua)
                .WithLock(LockMode.Upgrade)
                .FirstOrDefaultAsync();

            if (dagoenErreserba != null)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"{mahaia.Zenbakia}. mahaia jada erreserbatuta dago {dto.Data} datan eta {dto.Txanda} txandan.");
            }

            // Simulazio-itxaronaldia (konkurrentzia probetarako)
            if (dto.Itxaron > 0)
            {
                await Task.Delay(dto.Itxaron);
            }

            // Erreserba txertatu
            var erreserba = new Erreserba
            {
                Bezeroa = bezeroa,
                Mahaia = mahaia,
                Data = erreserbaData,
                Txanda = dto.Txanda,
                PertsonaKopurua = dto.PertsonaKopurua,
                Egoera = "Baieztatua",
                Oharrak = dto.Oharrak,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(erreserba);
            await tx.CommitAsync();

            return CreatedResult(MapErreserbaDto(erreserba), 201, "Erreserba arrakastaz baieztatu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Erreserba gatazka: mahaia jada erreserbatuta dago data eta txanda horretan.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da aldi bereko erreserbak kudeatzean. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea erreserba egitean: " + ex.Message);
        }
    }

    /// <summary>
    /// Erreserba ezeztatzen edo ezabatzen du (Soft Delete lehenetsia).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var erreserba = await session.GetAsync<Erreserba>(id, LockMode.Upgrade);
        if (hard)
        {
            if (erreserba == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen erreserbarik.");
            }

            await session.DeleteAsync(erreserba);
            await tx.CommitAsync();
            return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Erreserba behin betiko ezabatu da.");
        }
        else
        {
            if (erreserba == null || erreserba.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen erreserba aktiborik.");
            }

            erreserba.Ezabatua = true;
            erreserba.Egoera = "Ezeztatua";
            erreserba.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(erreserba);
            await tx.CommitAsync();
            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Erreserba ezeztatu da (soft delete).");
        }
    }

    /// <summary>
    /// Ezeztatutako erreserba berreskuratzen du transakzio pesimistan (mahai librearen egiaztapenarekin).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var erreserba = await session.GetAsync<Erreserba>(id, LockMode.Upgrade);
            if (erreserba == null || !erreserba.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen erreserbarik.");
            }

            // Blokeatu mahaia aldi bereko erreserba berriekin gatazkarik ez izateko
            await session.GetAsync<Mahaia>(erreserba.Mahaia.Id, LockMode.Upgrade);

            bool okupatuta = await session.Query<Erreserba>()
                .AnyAsync(r => r.Mahaia.Id == erreserba.Mahaia.Id
                            && r.Data == erreserba.Data
                            && r.Txanda == erreserba.Txanda
                            && !r.Ezabatua);

            if (okupatuta)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Ezin da berreskuratu: mahaia jada beste erreserba aktibo batek okupatu du txanda horretan.");
            }

            erreserba.Ezabatua = false;
            erreserba.Egoera = "Baieztatua";
            erreserba.EzabatzeData = null;
            await session.UpdateAsync(erreserba);

            await tx.CommitAsync();
            return Success(new { id, berreskuratua = true }, 200, "Erreserba arrakastaz berreskuratu da.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea erreserba berreskuratzean: " + ex.Message);
        }
    }

    private static ErreserbaDTO MapErreserbaDto(Erreserba e) => new()
    {
        Id = e.Id,
        BezeroaId = e.Bezeroa.Id,
        BezeroIzena = e.Bezeroa.Izena,
        BezeroAbizena = e.Bezeroa.Abizena,
        BezeroEmaila = e.Bezeroa.Emaila,
        BezeroTelefonoa = e.Bezeroa.Telefonoa,
        MahaiaId = e.Mahaia.Id,
        MahaiZenbakia = e.Mahaia.Zenbakia,
        Kokapena = e.Mahaia.Kokapena,
        Data = e.Data.ToString("yyyy-MM-dd"),
        Txanda = e.Txanda,
        PertsonaKopurua = e.PertsonaKopurua,
        Egoera = e.Egoera,
        Oharrak = e.Oharrak,
        Ezabatua = e.Ezabatua
    };
}
