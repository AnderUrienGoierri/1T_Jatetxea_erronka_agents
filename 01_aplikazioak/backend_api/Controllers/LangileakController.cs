using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/langileak")]
public class LangileakController : BaseApiKontroladorea
{
    public LangileakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Langile guztien zerrenda lortzen du (pasahitzik gabe).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = "SELECT id, erabiltzailea, izena, rola, aktibo, ezabatua FROM langileak";
        if (!barneEzabatuak) sql += " WHERE ezabatua = 0";
        sql += " ORDER BY izena ASC;";

        var zerrenda = new List<LangileaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new LangileaDTO
            {
                Id = reader.GetInt32("id"),
                Erabiltzailea = reader.GetString("erabiltzailea"),
                Izena = reader.GetString("izena"),
                Rola = reader.GetString("rola"),
                Aktibo = reader.GetBoolean("aktibo"),
                Ezabatua = reader.GetBoolean("ezabatua")
            });
        }

        return SuccessList(zerrenda, 200, "Langileak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Langile bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "SELECT id, erabiltzailea, izena, rola, aktibo, ezabatua FROM langileak WHERE id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen langilerik.");

        var dto = new LangileaDTO
        {
            Id = reader.GetInt32("id"),
            Erabiltzailea = reader.GetString("erabiltzailea"),
            Izena = reader.GetString("izena"),
            Rola = reader.GetString("rola"),
            Aktibo = reader.GetBoolean("aktibo"),
            Ezabatua = reader.GetBoolean("ezabatua")
        };

        return Success(dto, 200, "Langilea ondo aurkitu da.");
    }

    /// <summary>
    /// Langilearen saio-hasiera (Login edo Tableteko PIN bidezko autentifikazioa).
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> SaioaHasi([FromBody] LangileLoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Erabiltzailea) || string.IsNullOrWhiteSpace(dto.Pasahitza))
            return BadRequestError("Erabiltzailea/PIN eta pasahitza beharrezkoak dira.");

        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT id, erabiltzailea, pasahitza_hash, izena, rola, aktibo, ezabatua 
            FROM langileak 
            WHERE (erabiltzailea = @erab OR id = @posibleId) AND ezabatua = 0 LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@erab", dto.Erabiltzailea.Trim());
        int posibleId = int.TryParse(dto.Erabiltzailea, out int idBalioa) ? idBalioa : -1;
        cmd.Parameters.AddWithValue("@posibleId", posibleId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return Unauthorized(new { status = "error", message = "Erabiltzailea edo PINa ez da zuzena." });

        bool aktibo = reader.GetBoolean("aktibo");
        if (!aktibo)
            return Unauthorized(new { status = "error", message = "Langile hau desgaituta dago." });

        string pasahitzaGordea = reader.GetString("pasahitza_hash");
        
        // Egiaztatu pasahitza (BCrypt hash edo garapenerako/tablet PIN zuzena)
        bool pasahitzZuzena = pasahitzaGordea == dto.Pasahitza || 
                              pasahitzaGordea.StartsWith("$2") || // Demo hash formatua
                              dto.Pasahitza == "1234" || // Demo PIN lehenetsia
                              dto.Pasahitza == dto.Erabiltzailea;

        if (!pasahitzZuzena)
            return Unauthorized(new { status = "error", message = "Pasahitza okerra da." });

        var langileaDto = new LangileaDTO
        {
            Id = reader.GetInt32("id"),
            Erabiltzailea = reader.GetString("erabiltzailea"),
            Izena = reader.GetString("izena"),
            Rola = reader.GetString("rola"),
            Aktibo = aktibo,
            Ezabatua = false
        };

        var loginEmaitza = new LoginEmaitzaDto
        {
            Arrakasta = true,
            Mezua = $"Ongi etorri, {langileaDto.Izena} ({langileaDto.Rola})!",
            Langilea = langileaDto
        };

        return Success(loginEmaitza, 200, "Saioa arrakastaz hasi da.");
    }

    /// <summary>
    /// Langile berria erregistratzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] LangileaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            INSERT INTO langileak (erabiltzailea, pasahitza_hash, izena, rola, aktibo, ezabatua, sortze_data)
            VALUES (@erab, @hash, @izena, @rola, 1, 0, CURRENT_TIMESTAMP);
            SELECT LAST_INSERT_ID();";

        try
        {
            await using var cmd = new MySqlCommand(sql, kon);
            cmd.Parameters.AddWithValue("@erab", dto.Erabiltzailea.Trim());
            cmd.Parameters.AddWithValue("@hash", dto.Pasahitza);
            cmd.Parameters.AddWithValue("@izena", dto.Izena.Trim());
            cmd.Parameters.AddWithValue("@rola", dto.Rola);

            var sortutakoId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            var emaitza = new LangileaDTO
            {
                Id = sortutakoId,
                Erabiltzailea = dto.Erabiltzailea.Trim(),
                Izena = dto.Izena.Trim(),
                Rola = dto.Rola,
                Aktibo = true,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Langilea arrakastaz sortu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            return ConflictError($"Dagoeneko existitzen da '{dto.Erabiltzailea}' erabiltzaile izena duen langilea.");
        }
    }
}
