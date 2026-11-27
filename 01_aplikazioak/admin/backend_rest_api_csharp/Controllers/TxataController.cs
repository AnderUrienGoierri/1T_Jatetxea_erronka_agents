using System.Data;
using System.Net.Http.Json;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

/// <summary>
/// Barne-txat zerbitzuaren REST kontroladorea (Gerente JavaFX eta TPV C# arteko komunikazioa).
/// Langile bakoitzaren txata_aktibo baimena zerbitzari mailan bermatzen du eta
/// jatetxea-chat (Socket.IO :3001) zerbitzarira jakinarazpenak zubitzen ditu.
/// </summary>
[ApiController]
[Route("api/v1/txata")]
public class TxataController : BaseApiKontroladorea
{
    private static readonly HttpClient ChatBridgeClient = new()
    {
        Timeout = TimeSpan.FromMilliseconds(800)
    };

    private static readonly HashSet<string> GelaBaliozkoak = new(StringComparer.OrdinalIgnoreCase)
    {
        "Orokorra", "Jangela", "Sukaldea", "Barra"
    };

    public TxataController(ISessionFactory sessionFactory) : base(sessionFactory)
    {
    }

    /// <summary>
    /// Langile batek txata aktibatuta daukan egiaztatzen du.
    /// </summary>
    [HttpGet("egoera/{langileaId:int}")]
    public async Task<IActionResult> LortuEgoera(int langileaId)
    {
        using var session = IrekiSaioa();
        var langilea = await session.GetAsync<Langilea>(langileaId);
        if (langilea == null || langilea.Ezabatua)
            return NotFoundError($"Ez da aurkitu {langileaId} ID-a duen langilerik.");

        bool baimenduta = langilea.Aktibo && langilea.TxataAktibo;
        return Success(new
        {
            langileaId = langilea.Id,
            izena = langilea.Izena,
            aktibo = langilea.Aktibo,
            txataAktibo = langilea.TxataAktibo,
            baimenduta
        }, 200, baimenduta ? "Txata aktibatuta dago." : "Txata desaktibatuta dago langile honentzat.");
    }

    /// <summary>
    /// Gela bateko azken txat mezuak itzultzen ditu (langilearen txat-baimena egiaztatuz).
    /// </summary>
    [HttpGet("mezuak")]
    public async Task<IActionResult> LortuMezuak(
        [FromQuery] string gela = "Orokorra",
        [FromQuery] int limit = 60,
        [FromQuery] int? langileaId = null)
    {
        string gelaGarbia = NormalizatuGela(gela);
        int muga = Math.Clamp(limit, 1, 200);

        using var session = IrekiSaioa();

        if (langileaId.HasValue)
        {
            var eskatzailea = await session.GetAsync<Langilea>(langileaId.Value);
            if (eskatzailea == null || eskatzailea.Ezabatua || !eskatzailea.Aktibo)
                return StatusCode(403, new { status = "error", code = 403, message = "Langile hau desgaituta dago." });
            if (!eskatzailea.TxataAktibo)
                return StatusCode(403, new { status = "error", code = 403, message = "Txata desaktibatuta daukazu. Jarri harremanetan kudeatzailearekin." });
        }

        var mezuak = await session.Query<TxatMezua>()
            .Fetch(m => m.Langilea)
            .Where(m => m.Gela == gelaGarbia)
            .OrderByDescending(m => m.Id)
            .Take(muga)
            .ToListAsync();

        mezuak.Reverse();

        var zerrenda = mezuak.Select(MapToDto).ToList();
        return SuccessList(zerrenda, 200, $"{zerrenda.Count} txat mezu berreskuratu dira.");
    }

    /// <summary>
    /// Txat mezu berria gordetzen du (soilik langilea aktibo eta txataAktibo == true bada).
    /// </summary>
    [HttpPost("mezuak")]
    public async Task<IActionResult> BidaliMezua([FromBody] TxatMezuaSortuDto dto)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Testua))
            return BadRequestError("Mezuaren testua eta langilea beharrezkoak dira.");

        string gelaGarbia = NormalizatuGela(dto.Gela);
        string testuGarbia = dto.Testua.Trim();

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var langilea = await session.GetAsync<Langilea>(dto.LangileaId);
        if (langilea == null || langilea.Ezabatua || !langilea.Aktibo)
        {
            await RollbackSeguruaAsync(tx);
            return StatusCode(403, new { status = "error", code = 403, message = "Langile hau desgaituta dago eta ezin du mezurik bidali." });
        }

        if (!langilea.TxataAktibo)
        {
            await RollbackSeguruaAsync(tx);
            return StatusCode(403, new { status = "error", code = 403, message = "Txata desaktibatuta daukazu. Kudeatzaileak aktibatu behar dizu." });
        }

        var mezua = new TxatMezua
        {
            Langilea = langilea,
            Gela = gelaGarbia,
            Testua = testuGarbia,
            SortzeData = DateTime.Now
        };

        await session.SaveAsync(mezua);
        await tx.CommitAsync();

        var emaitza = MapToDto(mezua);

        // Best-effort zubia jatetxea-chat (Socket.IO :3001) zerbitzarira
        _ = JakinaraziSocketIoZerbitzariariAsync(emaitza);

        return CreatedResult(emaitza, 201, "Mezua arrakastaz bidali da.");
    }

    private static TxatMezuaDto MapToDto(TxatMezua m) => new()
    {
        Id = m.Id,
        LangileaId = m.Langilea.Id,
        LangileIzena = string.IsNullOrWhiteSpace(m.Langilea.Abizena)
            ? m.Langilea.Izena
            : $"{m.Langilea.Izena} {m.Langilea.Abizena}".Trim(),
        Erabiltzailea = m.Langilea.Erabiltzailea,
        Rola = m.Langilea.Rola,
        Gela = m.Gela,
        Testua = m.Testua,
        SortzeData = m.SortzeData.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static string NormalizatuGela(string? gela)
    {
        if (string.IsNullOrWhiteSpace(gela)) return "Orokorra";
        string trimmed = gela.Trim();
        foreach (var balioa in GelaBaliozkoak)
        {
            if (balioa.Equals(trimmed, StringComparison.OrdinalIgnoreCase))
                return balioa;
        }
        return "Orokorra";
    }

    private static async Task JakinaraziSocketIoZerbitzariariAsync(TxatMezuaDto mezua)
    {
        try
        {
            string chatUrl = Environment.GetEnvironmentVariable("CHAT_BRIDGE_URL")
                             ?? "http://jatetxea-chat:3001/api/notify";
            await ChatBridgeClient.PostAsJsonAsync(chatUrl, new
            {
                gela = mezua.Gela.ToLowerInvariant(),
                mezua = $"{mezua.LangileIzena}: {mezua.Testua}",
                mota = "txat_mezua",
                datuak = mezua
            });
        }
        catch
        {
            // Socket.IO zerbitzaria geldituta badago ere, REST txatak guztiz funtzionatzen du
        }
    }
}
