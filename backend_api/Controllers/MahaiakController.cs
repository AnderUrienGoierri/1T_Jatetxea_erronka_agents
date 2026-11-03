using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/mahaiak")]
public class MahaiakController : BaseApiKontroladorea
{
    public MahaiakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Mahai guztien zerrenda lortzen du.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = "SELECT id, zenbakia, edukiera, kokapena, egoera, ezabatua FROM mahaiak";
        if (!barneEzabatuak) sql += " WHERE ezabatua = 0";
        sql += " ORDER BY zenbakia ASC;";

        var zerrenda = new List<MahaiaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new MahaiaDTO
            {
                Id = reader.GetInt32("id"),
                Zenbakia = reader.GetInt32("zenbakia"),
                Edukiera = reader.GetInt32("edukiera"),
                Kokapena = reader.GetString("kokapena"),
                Egoera = reader.GetString("egoera"),
                Ezabatua = reader.GetBoolean("ezabatua")
            });
        }

        return SuccessList(zerrenda, 200, "Mahaiak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Mahai bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "SELECT id, zenbakia, edukiera, kokapena, egoera, ezabatua FROM mahaiak WHERE id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen mahaia.");

        var dto = new MahaiaDTO
        {
            Id = reader.GetInt32("id"),
            Zenbakia = reader.GetInt32("zenbakia"),
            Edukiera = reader.GetInt32("edukiera"),
            Kokapena = reader.GetString("kokapena"),
            Egoera = reader.GetString("egoera"),
            Ezabatua = reader.GetBoolean("ezabatua")
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

        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            INSERT INTO mahaiak (zenbakia, edukiera, kokapena, egoera, ezabatua, sortze_data)
            VALUES (@zenbakia, @edukiera, @kokapena, 'Libre', 0, CURRENT_TIMESTAMP);
            SELECT LAST_INSERT_ID();";

        try
        {
            await using var cmd = new MySqlCommand(sql, kon);
            cmd.Parameters.AddWithValue("@zenbakia", dto.Zenbakia);
            cmd.Parameters.AddWithValue("@edukiera", dto.Edukiera);
            cmd.Parameters.AddWithValue("@kokapena", dto.Kokapena);

            var sortutakoId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            var emaitza = new MahaiaDTO
            {
                Id = sortutakoId,
                Zenbakia = dto.Zenbakia,
                Edukiera = dto.Edukiera,
                Kokapena = dto.Kokapena,
                Egoera = "Libre",
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Mahaia arrakastaz sortu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
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

        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE mahaiak SET egoera = @egoera WHERE id = @id AND ezabatua = 0;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@egoera", dto.Egoera);
        cmd.Parameters.AddWithValue("@id", id);

        int errenkadak = await cmd.ExecuteNonQueryAsync();
        if (errenkadak == 0)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen mahai aktiborik.");

        return Success(new { id, egoera = dto.Egoera }, 200, "Mahaiaren egoera eguneratu da.");
    }

    /// <summary>
    /// Mahaia ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();

        if (hard)
        {
            const string sqlHard = "DELETE FROM mahaiak WHERE id = @id;";
            try
            {
                await using var cmd = new MySqlCommand(sqlHard, kon);
                cmd.Parameters.AddWithValue("@id", id);
                int errenkadak = await cmd.ExecuteNonQueryAsync();
                if (errenkadak == 0)
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen mahaia.");

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Mahaia behin betiko ezabatu da.");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                return ConflictError("Mahaia ezin da behin betiko ezabatu, erlazionatutako erreserbak edo eskaerak dituelako.");
            }
        }
        else
        {
            const string sqlSoft = "UPDATE mahaiak SET ezabatua = 1, ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id AND ezabatua = 0;";
            await using var cmd = new MySqlCommand(sqlSoft, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int errenkadak = await cmd.ExecuteNonQueryAsync();
            if (errenkadak == 0)
                return NotFoundError($"Ez da aurkitu {id} ID-a duen mahai aktiborik.");

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Mahaia desgaitu da (soft delete).");
        }
    }

    /// <summary>
    /// Desgaitutako mahaia berreskuratzen du (Restore).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE mahaiak SET ezabatua = 0, ezabatze_data = NULL WHERE id = @id AND ezabatua = 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        int errenkadak = await cmd.ExecuteNonQueryAsync();
        if (errenkadak == 0)
            return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen mahairik.");

        return Success(new { id, berreskuratua = true }, 200, "Mahaia berreskuratu da.");
    }
}
