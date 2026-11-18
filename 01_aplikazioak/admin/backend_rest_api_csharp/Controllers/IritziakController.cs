using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/iritziak")]
public class IritziakController : BaseApiKontroladorea
{
    public IritziakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Iritzi guztien zerrenda lortzen du bezeroaren informazioarekin.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = @"
            SELECT i.id, i.bezeroa_id, b.izena AS bezero_izena, b.emaila AS bezero_emaila,
                   i.balorazioa, i.testua, i.sortze_data, i.ezabatua
            FROM iritziak i
            JOIN bezeroak b ON i.bezeroa_id = b.id
            WHERE 1=1";

        if (!barneEzabatuak) sql += " AND i.ezabatua = 0 AND i.aktibo = 1";
        sql += " ORDER BY i.sortze_data DESC;";

        var zerrenda = new List<IritziaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new IritziaDTO
            {
                Id = reader.GetInt32("id"),
                BezeroaId = reader.GetInt32("bezeroa_id"),
                BezeroIzena = reader.GetString("bezero_izena"),
                BezeroEmaila = reader.GetString("bezero_emaila"),
                Balorazioa = reader.GetInt32("balorazioa"),
                Testua = reader.IsDBNull("testua") ? null : reader.GetString("testua"),
                SortzeData = reader.GetDateTime("sortze_data").ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        return SuccessList(zerrenda, 200, "Iritziak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Iritzien estatistika orokorrak lortzen ditu (v_balorazio_orokorrak ikuspegitik).
    /// </summary>
    [HttpGet("estatistikak")]
    public async Task<IActionResult> LortuEstatistikak()
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT guztira_iritziak, bataz_besteko_balorazioa,
                   bost_izarrak, lau_izarrak, hiru_izarrak, bi_izarrak, izar_bat, zero_izar
            FROM v_balorazio_orokorrak LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return Success(new BalorazioEstatistikakDTO(), 200, "Oraindik ez dago iritzirik.");
        }

        var dto = new BalorazioEstatistikakDTO
        {
            GuztiraIritziak = reader.IsDBNull("guztira_iritziak") ? 0 : Convert.ToInt32(reader["guztira_iritziak"]),
            BatazBestekoBalorazioa = reader.IsDBNull("bataz_besteko_balorazioa") ? 0 : Convert.ToDecimal(reader["bataz_besteko_balorazioa"]),
            BostIzarrak = reader.IsDBNull("bost_izarrak") ? 0 : Convert.ToInt32(reader["bost_izarrak"]),
            LauIzarrak = reader.IsDBNull("lau_izarrak") ? 0 : Convert.ToInt32(reader["lau_izarrak"]),
            HiruIzarrak = reader.IsDBNull("hiru_izarrak") ? 0 : Convert.ToInt32(reader["hiru_izarrak"]),
            BiIzarrak = reader.IsDBNull("bi_izarrak") ? 0 : Convert.ToInt32(reader["bi_izarrak"]),
            IzarBat = reader.IsDBNull("izar_bat") ? 0 : Convert.ToInt32(reader["izar_bat"]),
            ZeroIzar = reader.IsDBNull("zero_izar") ? 0 : Convert.ToInt32(reader["zero_izar"])
        };

        return Success(dto, 200, "Iritzien estatistikak ondo kalkulatu dira.");
    }

    /// <summary>
    /// Bezeroaren iritzi berria gordetzen du (0 eta 5 izar artean).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] IritziaSortuDto dto)
    {
        if (dto.Balorazioa < 0 || dto.Balorazioa > 5)
            return BadRequestError("Balorazioak 0 eta 5 izar artean egon behar du.");

        await using var kon = await SortuKonexioaAsync();

        // 1. Bezeroa zehaztu edo aurkitu
        int bezeroaId = dto.BezeroaId ?? 0;
        if (bezeroaId <= 0 && !string.IsNullOrWhiteSpace(dto.BezeroEmaila))
        {
            const string sqlBilatu = "SELECT id FROM bezeroak WHERE emaila = @email LIMIT 1;";
            await using var cmdB = new MySqlCommand(sqlBilatu, kon);
            cmdB.Parameters.AddWithValue("@email", dto.BezeroEmaila.Trim().ToLower());
            var aurkitutakoId = await cmdB.ExecuteScalarAsync();

            if (aurkitutakoId != null)
            {
                bezeroaId = Convert.ToInt32(aurkitutakoId);
            }
            else
            {
                const string sqlSortuBezeroa = @"
                    INSERT INTO bezeroak (izena, abizena, telefonoa, emaila, ezabatua, sortze_data)
                    VALUES (@izena, @abizena, @telefonoa, @emaila, 0, CURRENT_TIMESTAMP);
                    SELECT LAST_INSERT_ID();";

                await using var cmdS = new MySqlCommand(sqlSortuBezeroa, kon);
                cmdS.Parameters.AddWithValue("@izena", string.IsNullOrWhiteSpace(dto.BezeroIzena) ? "Bezero Anonimoa" : dto.BezeroIzena.Trim());
                cmdS.Parameters.AddWithValue("@abizena", string.IsNullOrWhiteSpace(dto.BezeroAbizena) ? "-" : dto.BezeroAbizena.Trim());
                cmdS.Parameters.AddWithValue("@telefonoa", string.IsNullOrWhiteSpace(dto.BezeroTelefonoa) ? "-" : dto.BezeroTelefonoa.Trim());
                cmdS.Parameters.AddWithValue("@emaila", dto.BezeroEmaila.Trim().ToLower());
                bezeroaId = Convert.ToInt32(await cmdS.ExecuteScalarAsync());
            }
        }

        if (bezeroaId <= 0)
            return BadRequestError("Bezeroa ezinbestekoa da (bezeroaId edo bezeroEmaila bidali behar da).");

        // 2. Iritzia gorde
        const string sqlIritzia = @"
            INSERT INTO iritziak (bezeroa_id, balorazioa, testua, aktibo, ezabatua, sortze_data)
            VALUES (@bezeroaId, @balorazioa, @testua, 1, 0, CURRENT_TIMESTAMP);
            SELECT LAST_INSERT_ID();";

        await using var cmdI = new MySqlCommand(sqlIritzia, kon);
        cmdI.Parameters.AddWithValue("@bezeroaId", bezeroaId);
        cmdI.Parameters.AddWithValue("@balorazioa", dto.Balorazioa);
        cmdI.Parameters.AddWithValue("@testua", (object?)dto.Testua ?? DBNull.Value);

        int iritziId = Convert.ToInt32(await cmdI.ExecuteScalarAsync());

        var emaitza = new IritziaDTO
        {
            Id = iritziId,
            BezeroaId = bezeroaId,
            BezeroIzena = dto.BezeroIzena ?? "Bezeroa",
            BezeroEmaila = dto.BezeroEmaila ?? "",
            Balorazioa = dto.Balorazioa,
            Testua = dto.Testua,
            SortzeData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return CreatedResult(emaitza, 201, "Iritzia arrakastaz gorde da.");
    }

    /// <summary>
    /// Iritzia ezabatzen du (Soft Delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();

        if (hard)
        {
            const string sqlHard = "DELETE FROM iritziak WHERE id = @id;";
            await using var cmd = new MySqlCommand(sqlHard, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen iritzirik.");
            return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Iritzia behin betiko ezabatu da.");
        }
        else
        {
            const string sqlSoft = "UPDATE iritziak SET ezabatua = 1, aktibo = 0, ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id AND ezabatua = 0;";
            await using var cmd = new MySqlCommand(sqlSoft, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen iritzi aktiborik.");
            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Iritzia desgaitu da.");
        }
    }
}
