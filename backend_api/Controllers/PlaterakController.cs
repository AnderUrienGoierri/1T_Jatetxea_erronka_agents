using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/platerak")]
public class PlaterakController : BaseApiKontroladorea
{
    public PlaterakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Plater guztien zerrenda lortzen du (aukeran kategoriaren eta ezabatuen arabera iragazita).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] string? kategoria = null, [FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = @"
            SELECT id, izena, deskribapena, kategoria, prezioa, alergenoak, aktibo, ezabatua 
            FROM platerak WHERE 1=1";

        if (!barneEzabatuak) sql += " AND ezabatua = 0";
        if (!string.IsNullOrWhiteSpace(kategoria)) sql += " AND kategoria = @kategoria";
        sql += " ORDER BY kategoria ASC, izena ASC;";

        var zerrenda = new List<PlateraDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        if (!string.IsNullOrWhiteSpace(kategoria))
            cmd.Parameters.AddWithValue("@kategoria", kategoria);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new PlateraDTO
            {
                Id = reader.GetInt32("id"),
                Izena = reader.GetString("izena"),
                Deskribapena = reader.IsDBNull("deskribapena") ? null : reader.GetString("deskribapena"),
                Kategoria = reader.GetString("kategoria"),
                Prezioa = reader.GetDecimal("prezioa"),
                Alergenoak = reader.IsDBNull("alergenoak") ? null : reader.GetString("alergenoak"),
                Aktibo = reader.GetBoolean("aktibo"),
                Ezabatua = reader.GetBoolean("ezabatua")
            });
        }

        return SuccessList(zerrenda, 200, "Platerak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Plateren erabilgarritasuna lortzen du (v_plateren_stock_ahalmena ikuspegitik):
    /// Lehengaien uneko stockaren arabera zenbat plater prestatu daitezkeen kalkulatuta.
    /// </summary>
    [HttpGet("erabilgarritasuna")]
    public async Task<IActionResult> LortuErabilgarritasuna()
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT platera_id, plater_izena, egin_daitezkeen_platerak 
            FROM v_plateren_stock_ahalmena 
            ORDER BY egin_daitezkeen_platerak DESC;";

        var zerrenda = new List<PlaterErabilgarritasunaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new PlaterErabilgarritasunaDTO
            {
                PlateraId = reader.GetInt32("platera_id"),
                PlaterIzena = reader.GetString("plater_izena"),
                EginDaitezkeenPlaterak = Convert.ToInt32(reader["egin_daitezkeen_platerak"])
            });
        }

        return SuccessList(zerrenda, 200, "Plateren erabilgarritasuna kalkulatu da.");
    }

    /// <summary>
    /// Plater bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT id, izena, deskribapena, kategoria, prezioa, alergenoak, aktibo, ezabatua 
            FROM platerak WHERE id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen platerik.");

        var dto = new PlateraDTO
        {
            Id = reader.GetInt32("id"),
            Izena = reader.GetString("izena"),
            Deskribapena = reader.IsDBNull("deskribapena") ? null : reader.GetString("deskribapena"),
            Kategoria = reader.GetString("kategoria"),
            Prezioa = reader.GetDecimal("prezioa"),
            Alergenoak = reader.IsDBNull("alergenoak") ? null : reader.GetString("alergenoak"),
            Aktibo = reader.GetBoolean("aktibo"),
            Ezabatua = reader.GetBoolean("ezabatua")
        };

        return Success(dto, 200, "Platera ondo aurkitu da.");
    }

    /// <summary>
    /// Plater baten errezeta eta osagaien proportzioak (eskandalloa) lortzen ditu.
    /// </summary>
    [HttpGet("{id:int}/eskandalloa")]
    public async Task<IActionResult> LortuEskandalloa(int id)
    {
        await using var kon = await SortuKonexioaAsync();

        // 1. Plateraren oinarrizko datuak
        const string sqlPlatera = "SELECT id, izena, prezioa FROM platerak WHERE id = @id LIMIT 1;";
        string platerIzena;
        decimal prezioa;
        await using (var cmdP = new MySqlCommand(sqlPlatera, kon))
        {
            cmdP.Parameters.AddWithValue("@id", id);
            await using var readerP = await cmdP.ExecuteReaderAsync();
            if (!await readerP.ReadAsync())
                return NotFoundError($"Ez da aurkitu {id} ID-a duen platerik.");

            platerIzena = readerP.GetString("izena");
            prezioa = readerP.GetDecimal("prezioa");
        }

        // 2. Osagaiak
        const string sqlOsagaiak = @"
            SELECT po.lehengaia_id, po.kantitatea, l.izena AS lehengaia_izena, l.unitatea 
            FROM plater_osagaiak po
            JOIN lehengaiak l ON po.lehengaia_id = l.id
            WHERE po.platera_id = @id AND l.ezabatua = 0;";

        var osagaiak = new List<PlaterOsagaiaDto>();
        await using (var cmdO = new MySqlCommand(sqlOsagaiak, kon))
        {
            cmdO.Parameters.AddWithValue("@id", id);
            await using var readerO = await cmdO.ExecuteReaderAsync();
            while (await readerO.ReadAsync())
            {
                osagaiak.Add(new PlaterOsagaiaDto
                {
                    LehengaiaId = readerO.GetInt32("lehengaia_id"),
                    LehengaiaIzena = readerO.GetString("lehengaia_izena"),
                    Kantitatea = readerO.GetDecimal("kantitatea"),
                    Unitatea = readerO.GetString("unitatea")
                });
            }
        }

        var emaitza = new PlaterEskandalloaDTO
        {
            PlateraId = id,
            PlaterIzena = platerIzena,
            Prezioa = prezioa,
            Osagaiak = osagaiak
        };

        return Success(emaitza, 200, "Plateraren eskandalloa ondo berreskuratu da.");
    }

    /// <summary>
    /// Plater berria sortzen du eta nahi izanez gero errezetako osagaiak erlazionatzen ditu.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] PlateraSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            const string sqlPlatera = @"
                INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak, aktibo, ezabatua, sortze_data)
                VALUES (@izena, @deskribapena, @kategoria, @prezioa, @alergenoak, 1, 0, CURRENT_TIMESTAMP);
                SELECT LAST_INSERT_ID();";

            int platerId;
            await using (var cmd = new MySqlCommand(sqlPlatera, kon, tx))
            {
                cmd.Parameters.AddWithValue("@izena", dto.Izena.Trim());
                cmd.Parameters.AddWithValue("@deskribapena", (object?)dto.Deskribapena ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@kategoria", dto.Kategoria);
                cmd.Parameters.AddWithValue("@prezioa", dto.Prezioa);
                cmd.Parameters.AddWithValue("@alergenoak", (object?)dto.Alergenoak ?? DBNull.Value);

                platerId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            if (dto.Osagaiak != null && dto.Osagaiak.Count > 0)
            {
                const string sqlOsagaia = @"
                    INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
                    VALUES (@platerId, @lehengaiaId, @kantitatea);";

                foreach (var osagai in dto.Osagaiak)
                {
                    await using var cmdOsagai = new MySqlCommand(sqlOsagaia, kon, tx);
                    cmdOsagai.Parameters.AddWithValue("@platerId", platerId);
                    cmdOsagai.Parameters.AddWithValue("@lehengaiaId", osagai.LehengaiaId);
                    cmdOsagai.Parameters.AddWithValue("@kantitatea", osagai.Kantitatea);
                    await cmdOsagai.ExecuteNonQueryAsync();
                }
            }

            await tx.CommitAsync();

            var emaitza = new PlateraDTO
            {
                Id = platerId,
                Izena = dto.Izena.Trim(),
                Deskribapena = dto.Deskribapena,
                Kategoria = dto.Kategoria,
                Prezioa = dto.Prezioa,
                Alergenoak = dto.Alergenoak,
                Aktibo = true,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Platera arrakastaz sortu da.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea platera sortzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Platera ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();

        if (hard)
        {
            const string sqlHard = "DELETE FROM platerak WHERE id = @id;";
            try
            {
                await using var cmd = new MySqlCommand(sqlHard, kon);
                cmd.Parameters.AddWithValue("@id", id);
                int errenkadak = await cmd.ExecuteNonQueryAsync();
                if (errenkadak == 0)
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen platerik.");

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Platera behin betiko ezabatu da.");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                return ConflictError("Platera ezin da behin betiko ezabatu, eskaera lerroetan edo errezetetan erabilia dagoelako.");
            }
        }
        else
        {
            const string sqlSoft = "UPDATE platerak SET ezabatua = 1, ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id AND ezabatua = 0;";
            await using var cmd = new MySqlCommand(sqlSoft, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int errenkadak = await cmd.ExecuteNonQueryAsync();
            if (errenkadak == 0)
                return NotFoundError($"Ez da aurkitu {id} ID-a duen plater aktiborik.");

            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Platera desgaitu da (soft delete).");
        }
    }

    /// <summary>
    /// Desgaitutako platera berreskuratzen du (Restore).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE platerak SET ezabatua = 0, ezabatze_data = NULL WHERE id = @id AND ezabatua = 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        int errenkadak = await cmd.ExecuteNonQueryAsync();
        if (errenkadak == 0)
            return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen platerik.");

        return Success(new { id, berreskuratua = true }, 200, "Platera berreskuratu da.");
    }
}
