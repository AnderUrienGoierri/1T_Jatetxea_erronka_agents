using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/fakturak")]
public class FakturakController : BaseApiKontroladorea
{
    public FakturakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Faktura guztien zerrenda lortzen du (salmenta eta erosketa fakturak).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak(
        [FromQuery] string? mota = null,
        [FromQuery] string? egoera = null,
        [FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        var query = session.Query<Faktura>();

        if (!barneEzabatuak) query = query.Where(f => !f.Ezabatua);
        if (!string.IsNullOrWhiteSpace(mota)) query = query.Where(f => f.Mota == mota);
        if (!string.IsNullOrWhiteSpace(egoera)) query = query.Where(f => f.Egoera == egoera);

        var fakturak = await query
            .OrderByDescending(f => f.Data)
            .ThenByDescending(f => f.Id)
            .ToListAsync();

        var zerrenda = fakturak.Select(MapFakturaDto).ToList();
        return SuccessList(zerrenda, 200, "Fakturak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Fakturen finantza-laburpena (Irabaziak, Gastuak, Mozkin garbia eta BEZa).
    /// </summary>
    [HttpGet("laburpena")]
    public async Task<IActionResult> LortuFinantzaLaburpena()
    {
        using var session = IrekiSaioa();
        var aktiboak = await session.Query<Faktura>()
            .Where(f => !f.Ezabatua && f.Egoera != "Ezeztatua")
            .ToListAsync();

        var salmentak = aktiboak.Where(f => f.Mota == "Salmenta").ToList();
        var erosketak = aktiboak.Where(f => f.Mota == "Erosketa").ToList();

        var laburpena = new FinantzaLaburpenaDto
        {
            SalmentakGuztira = salmentak.Sum(f => f.Guztira),
            GastuakGuztira = erosketak.Sum(f => f.Guztira),
            JasotakoBez = salmentak.Sum(f => f.BezZenbatekoa),
            OrdaindutakoBez = erosketak.Sum(f => f.BezZenbatekoa),
            SalmentaKopurua = salmentak.Count,
            GastuKopurua = erosketak.Count
        };

        return Success(laburpena, 200, "Finantza laburpena ondo kalkulatu da.");
    }

    /// <summary>
    /// Faktura bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();
        var faktura = await session.GetAsync<Faktura>(id);
        if (faktura == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen fakturarik.");

        return Success(MapFakturaDto(faktura), 200, "Faktura aurkitu da.");
    }

    /// <summary>
    /// Faktura berria erregistratzen du (Salmenta edo Hornitzailearen Erosketa/Gastua).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] FakturaSortuDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Kontzeptua))
            return BadRequestError("Kontzeptua derrigorrezkoa da.");

        decimal oinarria = Math.Max(0, dto.OinarriZergagarria);
        decimal bezEhunekoa = dto.BezEhunekoa > 0 ? dto.BezEhunekoa : 10.00m;
        decimal bezZenbatekoa = Math.Round(oinarria * (bezEhunekoa / 100m), 2);
        decimal guztira = dto.Guztira.HasValue && dto.Guztira.Value > 0
            ? dto.Guztira.Value
            : oinarria + bezZenbatekoa;

        string zenbakia = !string.IsNullOrWhiteSpace(dto.FakturaZenbakia)
            ? dto.FakturaZenbakia.Trim()
            : $"F{DateTime.Now.Year}-{(dto.Mota == "Erosketa" ? "G" : "S")}{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:D6}";

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            Eskaera? eskaera = dto.EskaeraId.HasValue
                ? await session.GetAsync<Eskaera>(dto.EskaeraId.Value)
                : null;

            var faktura = new Faktura
            {
                Eskaera = eskaera,
                FakturaZenbakia = zenbakia,
                Mota = dto.Mota,
                Azpimota = dto.Azpimota,
                Kontzeptua = dto.Kontzeptua.Trim(),
                Hornitzailea = dto.Hornitzailea,
                HornitzaileCif = dto.HornitzaileCif,
                BezeroNif = dto.BezeroNif,
                BezeroIzena = dto.BezeroIzena,
                BezeroHelbidea = dto.BezeroHelbidea,
                OinarriZergagarria = oinarria,
                BezEhunekoa = bezEhunekoa,
                BezZenbatekoa = bezZenbatekoa,
                Guztira = guztira,
                OrdainketaMetodoa = dto.OrdainketaMetodoa ?? "Txartela",
                Egoera = dto.Egoera ?? "Ordaindua",
                Data = dto.Data ?? DateTime.Now,
                Oharrak = dto.Oharrak,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(faktura);
            await tx.CommitAsync();

            return CreatedResult(MapFakturaDto(faktura), 201, "Faktura arrakastaz sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"Dagoeneko existitzen da '{zenbakia}' zenbakia duen faktura bat.");
        }
    }

    /// <summary>
    /// Fakturaren egoera aldatzen du ('Ordaindua', 'Zain', 'Ezeztatua').
    /// </summary>
    [HttpPut("{id:int}/egoera")]
    public async Task<IActionResult> EguneratuEgoera(int id, [FromBody] FakturaEgoeraEguneratuDto dto)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var faktura = await session.GetAsync<Faktura>(id, LockMode.Upgrade);
        if (faktura == null || faktura.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen faktura aktiborik.");
        }

        faktura.Egoera = dto.Egoera;
        await session.UpdateAsync(faktura);
        await tx.CommitAsync();

        return Success(new { id, egoera = dto.Egoera }, 200, "Fakturaren egoera eguneratu da.");
    }

    /// <summary>
    /// Faktura ezabatzen du (Soft delete lehenetsia, Hard delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var faktura = await session.GetAsync<Faktura>(id, LockMode.Upgrade);
        if (faktura == null)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen fakturarik.");
        }

        if (hard)
        {
            await session.DeleteAsync(faktura);
        }
        else
        {
            faktura.Ezabatua = true;
            faktura.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(faktura);
        }

        await tx.CommitAsync();
        return Success(new { id, ezabatua = true }, 200, "Faktura ezabatu da.");
    }

    private static FakturaDTO MapFakturaDto(Faktura f) => new()
    {
        Id = f.Id,
        EskaeraId = f.Eskaera?.Id,
        FakturaZenbakia = f.FakturaZenbakia,
        Mota = f.Mota,
        Azpimota = f.Azpimota,
        Kontzeptua = f.Kontzeptua,
        Hornitzailea = f.Hornitzailea,
        HornitzaileCif = f.HornitzaileCif,
        BezeroNif = f.BezeroNif,
        BezeroIzena = f.BezeroIzena,
        BezeroHelbidea = f.BezeroHelbidea,
        OinarriZergagarria = f.OinarriZergagarria,
        BezEhunekoa = f.BezEhunekoa,
        BezZenbatekoa = f.BezZenbatekoa,
        Guztira = f.Guztira,
        OrdainketaMetodoa = f.OrdainketaMetodoa,
        Egoera = f.Egoera,
        Data = f.Data,
        Oharrak = f.Oharrak,
        Ezabatua = f.Ezabatua
    };
}
