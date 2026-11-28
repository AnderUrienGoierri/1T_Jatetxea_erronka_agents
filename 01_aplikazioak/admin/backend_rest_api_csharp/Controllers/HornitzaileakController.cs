using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/hornitzaileak")]
public class HornitzaileakController : BaseApiKontroladorea
{
    public HornitzaileakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Hornitzaile aktiboen zerrenda lortzen du (produktu mota bakoitzeko hornitzailearekin).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Hornitzailea>();
        if (!barneEzabatuak)
            query = query.Where(h => !h.Ezabatua && h.Aktibo);

        var hornitzaileak = await query
            .OrderBy(h => h.ProduktuMota)
            .ThenBy(h => h.Izena)
            .ToListAsync();

        var kopuruak = (await session.Query<Lehengaia>()
            .Where(l => !l.Ezabatua && l.HornitzaileaEntitatea != null)
            .Select(l => l.HornitzaileaEntitatea!.Id)
            .ToListAsync())
            .GroupBy(id => id)
            .ToDictionary(g => g.Key, g => g.Count());

        var zerrenda = hornitzaileak.Select(h => new HornitzaileaDTO
        {
            Id = h.Id,
            Izena = h.Izena,
            Cif = h.Cif,
            ProduktuMota = h.ProduktuMota,
            BezEhunekoa = h.BezEhunekoa,
            Telefonoa = h.Telefonoa,
            Emaila = h.Emaila,
            Helbidea = h.Helbidea,
            Aktibo = h.Aktibo,
            LehengaiKopurua = kopuruak.TryGetValue(h.Id, out int kop) ? kop : 0
        }).ToList();

        return SuccessList(zerrenda, 200, "Hornitzaileak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Hornitzaile bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var h = await session.GetAsync<Hornitzailea>(id);
        if (h == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen hornitzailea.");

        int kop = await session.Query<Lehengaia>()
            .CountAsync(l => !l.Ezabatua && l.HornitzaileaEntitatea != null && l.HornitzaileaEntitatea.Id == id);

        var dto = new HornitzaileaDTO
        {
            Id = h.Id,
            Izena = h.Izena,
            Cif = h.Cif,
            ProduktuMota = h.ProduktuMota,
            BezEhunekoa = h.BezEhunekoa,
            Telefonoa = h.Telefonoa,
            Emaila = h.Emaila,
            Helbidea = h.Helbidea,
            Aktibo = h.Aktibo,
            LehengaiKopurua = kop
        };

        return Success(dto, 200, "Hornitzailea ondo aurkitu da.");
    }

    /// <summary>
    /// Hornitzaile jakin baten lehengai aktiboak lortzen ditu.
    /// </summary>
    [HttpGet("{id:int}/lehengaiak")]
    public async Task<IActionResult> LortuLehengaiak(int id)
    {
        using var session = IrekiSaioa();
        var hornitzailea = await session.GetAsync<Hornitzailea>(id);
        if (hornitzailea == null || hornitzailea.Ezabatua)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen hornitzailea.");

        var lehengaiak = await session.Query<Lehengaia>()
            .Where(l => !l.Ezabatua && l.HornitzaileaEntitatea != null && l.HornitzaileaEntitatea.Id == id)
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
            HornitzaileaId = hornitzailea.Id,
            Hornitzailea = hornitzailea.Izena,
            BezEhunekoa = hornitzailea.BezEhunekoa,
            Ezabatua = l.Ezabatua
        }).ToList();

        return SuccessList(zerrenda, 200, $"{hornitzailea.Izena} hornitzailearen lehengaiak berreskuratu dira.");
    }

    /// <summary>
    /// Hornitzaile berria sortzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] HornitzaileaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Hornitzailearen datu baliogabeak bidali dira.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var hornitzailea = new Hornitzailea
            {
                Izena = dto.Izena.Trim(),
                Cif = dto.Cif.Trim().ToUpperInvariant(),
                ProduktuMota = dto.ProduktuMota.Trim(),
                BezEhunekoa = dto.BezEhunekoa,
                Telefonoa = dto.Telefonoa?.Trim(),
                Emaila = dto.Emaila?.Trim(),
                Helbidea = dto.Helbidea?.Trim(),
                Aktibo = true,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(hornitzailea);
            await tx.CommitAsync();

            var emaitza = new HornitzaileaDTO
            {
                Id = hornitzailea.Id,
                Izena = hornitzailea.Izena,
                Cif = hornitzailea.Cif,
                ProduktuMota = hornitzailea.ProduktuMota,
                BezEhunekoa = hornitzailea.BezEhunekoa,
                Telefonoa = hornitzailea.Telefonoa,
                Emaila = hornitzailea.Emaila,
                Helbidea = hornitzailea.Helbidea,
                Aktibo = true,
                LehengaiKopurua = 0
            };

            return CreatedResult(emaitza, 201, "Hornitzailea arrakastaz sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da '{dto.Izena}' izena edo '{dto.Cif}' CIFa duen hornitzailea.");
        }
    }
}
