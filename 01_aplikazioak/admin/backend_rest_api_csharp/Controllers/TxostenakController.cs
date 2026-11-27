using System.Security.Claims;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

/// <summary>
/// Analitika konplexua, paginazio/bilaketa dinamikoa eta rol bidez babestutako auditoria (NHibernate LINQ + JWT RBAC).
/// </summary>
[ApiController]
[Route("api/v1/txostenak")]
public class TxostenakController : BaseApiKontroladorea
{
    public TxostenakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Analitika agregatu osoa: Top N plater salduenak, ordainketak motaka, langileen fakturazioa,
    /// eguneko bilakaera, batez besteko tiketa eta biltegiko balio ekonomikoa (NHibernate LINQ).
    /// </summary>
    [HttpGet("laburpena")]
    public async Task<IActionResult> LortuLaburpena(
        [FromQuery] string? hasiera = null,
        [FromQuery] string? amaiera = null,
        [FromQuery] int top = 5)
    {
        DateTime hasieraData = DateTime.TryParse(hasiera, out var h) ? h.Date : DateTime.Today.AddDays(-90);
        DateTime amaieraData = DateTime.TryParse(amaiera, out var a) ? a.Date.AddDays(1).AddTicks(-1) : DateTime.Today.AddDays(1).AddTicks(-1);
        int topMuga = Math.Clamp(top, 1, 20);

        using var session = IrekiSaioa();

        // 1. Eskaera baliozkoak data tartean (Fetch bidez langilea kargatuz)
        var eskaerak = await session.Query<Eskaera>()
            .Where(e => !e.Ezabatua && e.Egoera != "Ezeztatua"
                     && e.SortzeData >= hasieraData && e.SortzeData <= amaieraData)
            .Fetch(e => e.Langilea)
            .ToListAsync();

        var eskaeraIdak = eskaerak.Select(e => e.Id).ToHashSet();

        // 2. Top N plater salduenak (EskaeraLerroa + Platera)
        var lerroak = eskaeraIdak.Count > 0
            ? await session.Query<EskaeraLerroa>()
                .Where(el => eskaeraIdak.Contains(el.Eskaera.Id))
                .Fetch(el => el.Platera)
                .ToListAsync()
            : new List<EskaeraLerroa>();

        var topPlaterak = lerroak
            .GroupBy(el => new { el.Platera.Id, el.Platera.Izena, el.Platera.Kategoria })
            .Select(g => new TopPlateraDto
            {
                PlateraId = g.Key.Id,
                Izena = g.Key.Izena,
                Kategoria = g.Key.Kategoria,
                SaldutakoUnitateak = g.Sum(x => x.Kantitatea),
                DiruSarreraGuztira = Math.Round(g.Sum(x => x.Kantitatea * x.UnitatePrezioa), 2)
            })
            .OrderByDescending(x => x.SaldutakoUnitateak)
            .ThenByDescending(x => x.DiruSarreraGuztira)
            .Take(topMuga)
            .ToList();

        // 3. Ordainketak motaren arabera (Eskudirua, Txartela, Bizum...)
        var ordainketak = await session.Query<Ordainketa>()
            .Where(o => o.Egoera == "Arrakastatsua" && o.Data >= hasieraData && o.Data <= amaieraData)
            .ToListAsync();

        var ordainketakMotaka = ordainketak
            .GroupBy(o => o.Mota)
            .Select(g => new OrdainketaMotaLaburpenaDto
            {
                Mota = g.Key,
                EragiketaKopurua = g.Count(),
                ZenbatekoaGuztira = Math.Round(g.Sum(x => x.Zenbatekoa), 2)
            })
            .OrderByDescending(x => x.ZenbatekoaGuztira)
            .ToList();

        // 4. Langile bakoitzaren salmentak
        var langileSalmentak = eskaerak
            .GroupBy(e => new { e.Langilea.Id, e.Langilea.Izena, e.Langilea.Rola })
            .Select(g => new LangileSalmentaDto
            {
                LangileaId = g.Key.Id,
                Izena = g.Key.Izena,
                Rola = g.Key.Rola,
                EskaeraKopurua = g.Count(),
                SalmentaGuztira = Math.Round(g.Sum(x => x.Guztira), 2)
            })
            .OrderByDescending(x => x.SalmentaGuztira)
            .ToList();

        // 5. Eguneko fakturazio bilakaera
        var egunekoFakturazioa = eskaerak
            .GroupBy(e => e.SortzeData.Date)
            .Select(g => new EgunekoFakturazioaDto
            {
                Eguna = g.Key.ToString("yyyy-MM-dd"),
                EskaeraKopurua = g.Count(),
                Guztira = Math.Round(g.Sum(x => x.Guztira), 2)
            })
            .OrderByDescending(x => x.Eguna)
            .Take(30)
            .ToList();

        // 6. Biltegiaren balio ekonomiko osoa eta stock alertak (VIEW)
        var lehengaiAktiboak = await session.Query<Lehengaia>()
            .Where(l => !l.Ezabatua)
            .ToListAsync();
        decimal biltegiBalioa = Math.Round(lehengaiAktiboak.Sum(l => l.UnekoStocka * l.KostuUnitarioa), 2);
        int alertaKopurua = await session.Query<ErosiBeharrekoLehengaia>().CountAsync();

        decimal fakturazioGuztira = Math.Round(eskaerak.Sum(e => e.Guztira), 2);
        decimal batezBestekoTiketa = eskaerak.Count > 0 ? Math.Round(fakturazioGuztira / eskaerak.Count, 2) : 0m;

        var laburpena = new TxostenLaburpenaDto
        {
            HasieraData = hasieraData.ToString("yyyy-MM-dd"),
            AmaieraData = amaieraData.ToString("yyyy-MM-dd"),
            EskaeraKopurua = eskaerak.Count,
            FakturazioGuztira = fakturazioGuztira,
            BatezBestekoTiketa = batezBestekoTiketa,
            BiltegiBalioGuztira = biltegiBalioa,
            StockAlertaKopurua = alertaKopurua,
            TopPlaterak = topPlaterak,
            OrdainketakMotaka = ordainketakMotaka,
            LangileSalmentak = langileSalmentak,
            EgunekoFakturazioa = egunekoFakturazioa
        };

        return Success(laburpena, 200, "Analitika eta txosten agregatua ondo kalkulatu da.");
    }

    /// <summary>
    /// Plateren bilaketa aurreratua, iragazketa konbinatua, ordenazio dinamikoa eta SQL paginazio erreala (Skip/Take).
    /// </summary>
    [HttpGet("platerak-bilatu")]
    public async Task<IActionResult> BilatuPlaterakPaginatuta(
        [FromQuery] string? search = null,
        [FromQuery] string? kategoria = null,
        [FromQuery] decimal? minPrezioa = null,
        [FromQuery] decimal? maxPrezioa = null,
        [FromQuery] string sortBy = "izena",
        [FromQuery] string sortDir = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        int orria = Math.Max(1, page);
        int tamaina = Math.Clamp(pageSize, 1, 50);
        bool beherakorra = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase);

        using var session = IrekiSaioa();
        IQueryable<Platera> query = session.Query<Platera>().Where(p => !p.Ezabatua);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string testua = search.Trim().ToLower();
            query = query.Where(p => p.Izena.ToLower().Contains(testua)
                                  || (p.Deskribapena != null && p.Deskribapena.ToLower().Contains(testua)));
        }
        if (!string.IsNullOrWhiteSpace(kategoria))
            query = query.Where(p => p.Kategoria == kategoria.Trim());
        if (minPrezioa.HasValue)
            query = query.Where(p => p.Prezioa >= minPrezioa.Value);
        if (maxPrezioa.HasValue)
            query = query.Where(p => p.Prezioa <= maxPrezioa.Value);

        int guztira = await query.CountAsync();

        query = sortBy.ToLowerInvariant() switch
        {
            "prezioa" => beherakorra ? query.OrderByDescending(p => p.Prezioa) : query.OrderBy(p => p.Prezioa),
            "kategoria" => beherakorra ? query.OrderByDescending(p => p.Kategoria).ThenBy(p => p.Izena) : query.OrderBy(p => p.Kategoria).ThenBy(p => p.Izena),
            _ => beherakorra ? query.OrderByDescending(p => p.Izena) : query.OrderBy(p => p.Izena)
        };

        var platerak = await query
            .Skip((orria - 1) * tamaina)
            .Take(tamaina)
            .ToListAsync();

        // Batu stock ahalmena VIEW-tik (N+1 gabe, kontsulta bakarrean)
        var platerIdak = platerak.Select(p => p.Id).ToList();
        var stockMapa = platerIdak.Count > 0
            ? (await session.Query<PlaterStockAhalmena>()
                .Where(v => platerIdak.Contains(v.PlateraId))
                .ToListAsync())
                .ToDictionary(v => v.PlateraId, v => v.EginDaitezkeenPlaterak)
            : new Dictionary<int, int>();

        var emaitza = new PaginatutakoPlaterakDto
        {
            Page = orria,
            PageSize = tamaina,
            TotalItems = guztira,
            TotalPages = (int)Math.Ceiling(guztira / (double)tamaina),
            SortBy = sortBy.ToLowerInvariant(),
            SortDir = beherakorra ? "desc" : "asc",
            Items = platerak.Select(p => new PlaterBilaketaItemDto
            {
                Id = p.Id,
                Izena = p.Izena,
                Deskribapena = p.Deskribapena,
                Kategoria = p.Kategoria,
                Prezioa = p.Prezioa,
                Alergenoak = p.Alergenoak,
                Aktibo = p.Aktibo,
                EginDaitezkeenAnoak = stockMapa.TryGetValue(p.Id, out int anoak) ? anoak : null
            }).ToList()
        };

        return Success(emaitza, 200, "Plateren bilaketa paginatua ondo burutu da.");
    }

    /// <summary>
    /// JWT Bearer + Rol bidez babestutako auditoria-txostena (Admin, AdminIT edo Kudeatzailea soilik).
    /// Tokenik gabe -> 401 Unauthorized; beste rol batekin (adib. Zerbitzaria) -> 403 Forbidden.
    /// </summary>
    [Authorize(Roles = "Admin,AdminIT,Kudeatzailea")]
    [HttpGet("auditoria-segurua")]
    public async Task<IActionResult> LortuAuditoriaSegurua()
    {
        using var session = IrekiSaioa();

        int mugimenduKop = await session.Query<StockMugimendua>().CountAsync();
        int ordainketaKop = await session.Query<Ordainketa>().CountAsync();

        var azkenak = await session.Query<StockMugimendua>()
            .OrderByDescending(m => m.Id)
            .Fetch(m => m.Lehengaia)
            .Take(15)
            .ToListAsync();

        var dto = new AuditoriaSeguruaDto
        {
            Erabiltzailea = User.FindFirstValue("erabiltzailea") ?? "",
            Izena = User.FindFirstValue(ClaimTypes.Name) ?? "",
            Rola = User.FindFirstValue(ClaimTypes.Role) ?? "",
            KontsultaOrdua = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            StockMugimenduakGuztira = mugimenduKop,
            OrdainketakGuztira = ordainketaKop,
            AzkenStockMugimenduak = azkenak.Select(m => (object)new
            {
                id = m.Id,
                lehengaia = m.Lehengaia.Izena,
                mota = m.Mota,
                kantitatea = m.Kantitatea,
                unitatea = m.Lehengaia.Unitatea,
                eskaeraId = m.Eskaera?.Id,
                oharrak = m.Oharrak,
                data = m.Data.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList()
        };

        return Success(dto, 200, "Auditoria segurua ondo berreskuratu da (JWT + RBAC balidatuta).");
    }
}
