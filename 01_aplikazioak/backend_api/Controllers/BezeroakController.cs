using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/bezeroak")]
public class BezeroakController : BaseApiKontroladorea
{
    public BezeroakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Bezero guztien zerrenda lortzen du.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = "SELECT id, izena, abizena, telefonoa, emaila, ezabatua FROM bezeroak";
        if (!barneEzabatuak) sql += " WHERE ezabatua = 0";
        sql += " ORDER BY abizena ASC, izena ASC;";

        var zerrenda = new List<BezeroaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new BezeroaDTO
            {
                Id = reader.GetInt32("id"),
                Izena = reader.GetString("izena"),
                Abizena = reader.GetString("abizena"),
                Telefonoa = reader.GetString("telefonoa"),
                Emaila = reader.GetString("emaila"),
                Ezabatua = reader.GetBoolean("ezabatua")
            });
        }

        return SuccessList(zerrenda, 200, "Bezeroak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Bezero bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "SELECT id, izena, abizena, telefonoa, emaila, ezabatua FROM bezeroak WHERE id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen bezerorik.");

        var dto = new BezeroaDTO
        {
            Id = reader.GetInt32("id"),
            Izena = reader.GetString("izena"),
            Abizena = reader.GetString("abizena"),
            Telefonoa = reader.GetString("telefonoa"),
            Emaila = reader.GetString("emaila"),
            Ezabatua = reader.GetBoolean("ezabatua")
        };

        return Success(dto, 200, "Bezeroa ondo aurkitu da.");
    }

    /// <summary>
    /// Bezero berria sortzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] BezeroaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            INSERT INTO bezeroak (izena, abizena, telefonoa, emaila, ezabatua, sortze_data)
            VALUES (@izena, @abizena, @telefonoa, @emaila, 0, CURRENT_TIMESTAMP);
            SELECT LAST_INSERT_ID();";

        try
        {
            await using var cmd = new MySqlCommand(sql, kon);
            cmd.Parameters.AddWithValue("@izena", dto.Izena.Trim());
            cmd.Parameters.AddWithValue("@abizena", dto.Abizena.Trim());
            cmd.Parameters.AddWithValue("@telefonoa", dto.Telefonoa.Trim());
            cmd.Parameters.AddWithValue("@emaila", dto.Emaila.Trim().ToLower());

            var sortutakoId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            var emaitza = new BezeroaDTO
            {
                Id = sortutakoId,
                Izena = dto.Izena.Trim(),
                Abizena = dto.Abizena.Trim(),
                Telefonoa = dto.Telefonoa.Trim(),
                Emaila = dto.Emaila.Trim().ToLower(),
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Bezeroa arrakastaz sortu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            return ConflictError($"Dagoeneko existitzen da '{dto.Emaila}' emaila duen bezerorik.");
        }
    }

    /// <summary>
    /// Bezeroaren datuak eguneratzen ditu.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] BezeroaSortuDto dto)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            UPDATE bezeroak 
            SET izena = @izena, abizena = @abizena, telefonoa = @telefonoa, emaila = @emaila 
            WHERE id = @id AND ezabatua = 0;";

        try
        {
            await using var cmd = new MySqlCommand(sql, kon);
            cmd.Parameters.AddWithValue("@izena", dto.Izena.Trim());
            cmd.Parameters.AddWithValue("@abizena", dto.Abizena.Trim());
            cmd.Parameters.AddWithValue("@telefonoa", dto.Telefonoa.Trim());
            cmd.Parameters.AddWithValue("@emaila", dto.Emaila.Trim().ToLower());
            cmd.Parameters.AddWithValue("@id", id);

            int rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen bezerorik.");

            return Success(new { id, izena = dto.Izena, emaila = dto.Emaila }, 200, "Bezeroaren datuak ondo eguneratu dira.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            return ConflictError($"Dagoeneko existitzen da beste bezero bat '{dto.Emaila}' emailarekin.");
        }
    }

    /// <summary>
    /// Bezeroa ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();

        if (hard)
        {
            const string sqlHard = "DELETE FROM bezeroak WHERE id = @id;";
            try
            {
                await using var cmd = new MySqlCommand(sqlHard, kon);
                cmd.Parameters.AddWithValue("@id", id);
                int errenkadak = await cmd.ExecuteNonQueryAsync();
                if (errenkadak == 0)
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen bezerorik.");

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Bezeroa behin betiko ezabatu da.");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                return ConflictError("Bezeroa ezin da behin betiko ezabatu, erlazionatutako erreserbak edo iritziak dituelako.");
            }
        }
        else
        {
            const string sqlSoft = "UPDATE bezeroak SET ezabatua = 1, ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id AND ezabatua = 0;";
            await using var cmd = new MySqlCommand(sqlSoft, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int errenkadak = await cmd.ExecuteNonQueryAsync();
            if (errenkadak == 0)
                return NotFoundError($"Ez da aurkitu {id} ID-a duen bezero aktiborik.");

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Bezeroa desgaitu da (soft delete).");
        }
    }
}
