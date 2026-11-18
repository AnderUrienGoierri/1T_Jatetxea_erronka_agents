using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/lehengaiak")]
public class LehengaiakController : BaseApiKontroladorea
{
    public LehengaiakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Lehengai guztien zerrenda lortzen du uneko stockarekin.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = @"
            SELECT id, izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea, ezabatua 
            FROM lehengaiak";
        if (!barneEzabatuak) sql += " WHERE ezabatua = 0";
        sql += " ORDER BY izena ASC;";

        var zerrenda = new List<LehengaiaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new LehengaiaDTO
            {
                Id = reader.GetInt32("id"),
                Izena = reader.GetString("izena"),
                Unitatea = reader.GetString("unitatea"),
                UnekoStocka = reader.GetDecimal("uneko_stocka"),
                GutxienekoStocka = reader.GetDecimal("gutxieneko_stocka"),
                KostuUnitarioa = reader.GetDecimal("kostu_unitarioa"),
                Hornitzailea = reader.IsDBNull("hornitzailea") ? null : reader.GetString("hornitzailea"),
                Ezabatua = reader.GetBoolean("ezabatua")
            });
        }

        return SuccessList(zerrenda, 200, "Lehengaiak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Lehengai bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT id, izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea, ezabatua 
            FROM lehengaiak WHERE id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengaia.");

        var dto = new LehengaiaDTO
        {
            Id = reader.GetInt32("id"),
            Izena = reader.GetString("izena"),
            Unitatea = reader.GetString("unitatea"),
            UnekoStocka = reader.GetDecimal("uneko_stocka"),
            GutxienekoStocka = reader.GetDecimal("gutxieneko_stocka"),
            KostuUnitarioa = reader.GetDecimal("kostu_unitarioa"),
            Hornitzailea = reader.IsDBNull("hornitzailea") ? null : reader.GetString("hornitzailea"),
            Ezabatua = reader.GetBoolean("ezabatua")
        };

        return Success(dto, 200, "Lehengaia ondo aurkitu da.");
    }

    /// <summary>
    /// Erosketa-zerrenda: gutxieneko stocketik behera dauden lehengaiak (v_erosi_beharreko_lehengaiak ikuspegitik).
    /// </summary>
    [HttpGet("alertak")]
    public async Task<IActionResult> LortuAlertak()
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT lehengaia_id, izena, unitatea, uneko_stocka, gutxieneko_stocka, proposatutako_erosketa, hornitzailea 
            FROM v_erosi_beharreko_lehengaiak 
            ORDER BY uneko_stocka ASC;";

        var alertak = new List<LehengaiaAlertaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            alertak.Add(new LehengaiaAlertaDTO
            {
                LehengaiaId = reader.GetInt32("lehengaia_id"),
                Izena = reader.GetString("izena"),
                Unitatea = reader.GetString("unitatea"),
                UnekoStocka = reader.GetDecimal("uneko_stocka"),
                GutxienekoStocka = reader.GetDecimal("gutxieneko_stocka"),
                ProposatutakoErosketa = reader.GetDecimal("proposatutako_erosketa"),
                Hornitzailea = reader.IsDBNull("hornitzailea") ? null : reader.GetString("hornitzailea")
            });
        }

        return SuccessList(alertak, 200, "Stock alertak (erosketa-zerrenda) berreskuratu dira.");
    }

    /// <summary>
    /// Lehengai berria sortzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] LehengaiaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");
        if (!UnitateBaliozkoa(dto.Unitatea))
            return BadRequestError("Unitatea ez da baliozkoa. Erabili kg, g, l, ml edo unitatea.");

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        const string sql = @"
            INSERT INTO lehengaiak (izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea, ezabatua, sortze_data)
            VALUES (@izena, @unitatea, @uneko, @gutxi, @kostu, @hornitzailea, 0, CURRENT_TIMESTAMP);
            SELECT LAST_INSERT_ID();";

        try
        {
            await using var cmd = new MySqlCommand(sql, kon, tx);
            cmd.Parameters.AddWithValue("@izena", dto.Izena.Trim());
            cmd.Parameters.AddWithValue("@unitatea", dto.Unitatea);
            cmd.Parameters.AddWithValue("@uneko", dto.UnekoStocka);
            cmd.Parameters.AddWithValue("@gutxi", dto.GutxienekoStocka);
            cmd.Parameters.AddWithValue("@kostu", dto.KostuUnitarioa);
            cmd.Parameters.AddWithValue("@hornitzailea", (object?)dto.Hornitzailea?.Trim() ?? DBNull.Value);

            var sortutakoId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            if (dto.UnekoStocka > 0)
            {
                const string stockSql = @"
                    INSERT INTO stock_mugimenduak (lehengaia_id, mota, kantitatea, oharrak, data)
                    VALUES (@id, 'Egokitzapena_Inbentarioa', @kantitatea, 'Hasierako inbentarioa', CURRENT_TIMESTAMP);";
                await using var stockCmd = new MySqlCommand(stockSql, kon, tx);
                stockCmd.Parameters.AddWithValue("@id", sortutakoId);
                stockCmd.Parameters.AddWithValue("@kantitatea", dto.UnekoStocka);
                await stockCmd.ExecuteNonQueryAsync();
            }
            await tx.CommitAsync();

            var emaitza = new LehengaiaDTO
            {
                Id = sortutakoId,
                Izena = dto.Izena.Trim(),
                Unitatea = dto.Unitatea,
                UnekoStocka = dto.UnekoStocka,
                GutxienekoStocka = dto.GutxienekoStocka,
                KostuUnitarioa = dto.KostuUnitarioa,
                Hornitzailea = dto.Hornitzailea,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Lehengaia arrakastaz sortu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            await tx.RollbackAsync();
            return ConflictError($"Dagoeneko existitzen da '{dto.Izena}' izena duen lehengaia.");
        }
    }

    /// <summary>
    /// Lehengaiaren datu nagusiak eguneratzen ditu, uneko stocka eta historia aldatu gabe.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Eguneratu(int id, [FromBody] LehengaiaEguneratuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");
        if (!UnitateBaliozkoa(dto.Unitatea))
            return BadRequestError("Unitatea ez da baliozkoa. Erabili kg, g, l, ml edo unitatea.");

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        const string lockSql = @"
            SELECT unitatea, ezabatua
            FROM lehengaiak
            WHERE id = @id
            FOR UPDATE;";
        string unitateZaharra;
        await using (var lockCmd = new MySqlCommand(lockSql, kon, tx))
        {
            lockCmd.Parameters.AddWithValue("@id", id);
            await using var reader = await lockCmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengaia.");
            if (reader.GetBoolean("ezabatua"))
                return ConflictError("Desgaitutako lehengai bat ezin da editatu.");
            unitateZaharra = reader.GetString("unitatea");
        }

        if (!string.Equals(unitateZaharra, dto.Unitatea, StringComparison.Ordinal))
        {
            const string historySql = @"
                SELECT EXISTS(SELECT 1 FROM stock_mugimenduak WHERE lehengaia_id = @id)
                    OR EXISTS(SELECT 1 FROM plater_osagaiak WHERE lehengaia_id = @id);";
            await using var historyCmd = new MySqlCommand(historySql, kon, tx);
            historyCmd.Parameters.AddWithValue("@id", id);
            if (Convert.ToInt32(await historyCmd.ExecuteScalarAsync()) != 0)
                return ConflictError("Unitatea ezin da aldatu stock-historia edo errezeta bati lotuta dagoenean.");
        }

        const string updateSql = @"
            UPDATE lehengaiak
            SET izena = @izena,
                unitatea = @unitatea,
                gutxieneko_stocka = @gutxi,
                kostu_unitarioa = @kostu,
                hornitzailea = @hornitzailea
            WHERE id = @id AND ezabatua = 0;";
        try
        {
            await using var cmd = new MySqlCommand(updateSql, kon, tx);
            cmd.Parameters.AddWithValue("@izena", dto.Izena.Trim());
            cmd.Parameters.AddWithValue("@unitatea", dto.Unitatea);
            cmd.Parameters.AddWithValue("@gutxi", dto.GutxienekoStocka);
            cmd.Parameters.AddWithValue("@kostu", dto.KostuUnitarioa);
            cmd.Parameters.AddWithValue("@hornitzailea", (object?)dto.Hornitzailea?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
            await tx.CommitAsync();

            return Success(new
            {
                id,
                izena = dto.Izena.Trim(),
                unitatea = dto.Unitatea,
                gutxienekoStocka = dto.GutxienekoStocka,
                kostuUnitarioa = dto.KostuUnitarioa,
                hornitzailea = dto.Hornitzailea
            }, 200, "Lehengaiaren datuak eguneratu dira; stocka aldatu gabe mantendu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            await tx.RollbackAsync();
            return ConflictError($"Dagoeneko existitzen da '{dto.Izena}' izena duen lehengaia.");
        }
    }

    /// <summary>
    /// Hornitzailearen albarana erregistratu eta lehengaiaren stocka gehitu transakzionalean.
    /// </summary>
    [HttpPost("sarrera")]
    public async Task<IActionResult> ErregistratuSarrera([FromBody] AlbaranaSarreraDto dto)
    {
        if (dto.Kantitatea <= 0)
            return BadRequestError("Sarrerako kantitateak 0 baino handiagoa izan behar du.");

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Lehengaia blokeatu eta egiaztatu
            const string sqlCheck = "SELECT izena, uneko_stocka, unitatea, ezabatua FROM lehengaiak WHERE id = @id FOR UPDATE;";
            string izena;
            decimal unekoStocka;
            string unitatea;

            await using (var cmdCheck = new MySqlCommand(sqlCheck, kon, tx))
            {
                cmdCheck.Parameters.AddWithValue("@id", dto.LehengaiaId);
                await using var reader = await cmdCheck.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {dto.LehengaiaId} ID-a duen lehengaia.");
                }

                if (reader.GetBoolean("ezabatua"))
                {
                    await tx.RollbackAsync();
                    return ConflictError("Lehengaia desgaituta dago. Berreskuratu lehenago.");
                }

                izena = reader.GetString("izena");
                unekoStocka = reader.GetDecimal("uneko_stocka");
                unitatea = reader.GetString("unitatea");
            }

            // 2. Stocka eguneratu
            decimal stockBerria = unekoStocka + dto.Kantitatea;
            const string sqlUpdate = "UPDATE lehengaiak SET uneko_stocka = @berria WHERE id = @id;";
            await using (var cmdUpd = new MySqlCommand(sqlUpdate, kon, tx))
            {
                cmdUpd.Parameters.AddWithValue("@berria", stockBerria);
                cmdUpd.Parameters.AddWithValue("@id", dto.LehengaiaId);
                await cmdUpd.ExecuteNonQueryAsync();
            }

            // 3. Auditoria erregistratu stock_mugimenduak taulan
            const string sqlMugimendua = @"
                INSERT INTO stock_mugimenduak (lehengaia_id, mota, kantitatea, oharrak, data)
                VALUES (@id, 'Sarrera_Albarana', @kantitatea, @oharrak, CURRENT_TIMESTAMP);";
            await using (var cmdMug = new MySqlCommand(sqlMugimendua, kon, tx))
            {
                cmdMug.Parameters.AddWithValue("@id", dto.LehengaiaId);
                cmdMug.Parameters.AddWithValue("@kantitatea", dto.Kantitatea);
                string oharra = dto.Oharrak ?? (dto.Hornitzailea != null ? $"Hornitzailea: {dto.Hornitzailea}" : "Albaran sarrera");
                cmdMug.Parameters.AddWithValue("@oharrak", oharra);
                await cmdMug.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();

            return Success(new
            {
                lehengaiaId = dto.LehengaiaId,
                izena,
                aurrekoStocka = unekoStocka,
                gehitutakoKantitatea = dto.Kantitatea,
                unekoStockBerria = stockBerria,
                unitatea
            }, 200, "Albarana ondo erregistratu da eta stocka eguneratu da.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea albarana erregistratzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Lehengaia ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();

        if (hard)
        {
            const string sqlHard = "DELETE FROM lehengaiak WHERE id = @id;";
            try
            {
                await using var cmd = new MySqlCommand(sqlHard, kon);
                cmd.Parameters.AddWithValue("@id", id);
                int errenkadak = await cmd.ExecuteNonQueryAsync();
                if (errenkadak == 0)
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengaia.");

                return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Lehengaia behin betiko ezabatu da.");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                return ConflictError("Lehengaia ezin da behin betiko ezabatu, plateren errezetetan edo stock mugimenduetan erabilia dagoelako.");
            }
        }
        else
        {
            await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            const string lockSql = "SELECT ezabatua FROM lehengaiak WHERE id = @id FOR UPDATE;";
            await using (var lockCmd = new MySqlCommand(lockSql, kon, tx))
            {
                lockCmd.Parameters.AddWithValue("@id", id);
                var deletedValue = await lockCmd.ExecuteScalarAsync();
                if (deletedValue == null)
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengai aktiborik.");
                if (Convert.ToBoolean(deletedValue))
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengai aktiborik.");
            }

            const string recipeUseSql = @"
                SELECT EXISTS(
                    SELECT 1
                    FROM plater_osagaiak po
                    JOIN platerak p ON p.id = po.platera_id
                    WHERE po.lehengaia_id = @id AND p.ezabatua = 0
                );";
            await using (var useCmd = new MySqlCommand(recipeUseSql, kon, tx))
            {
                useCmd.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(await useCmd.ExecuteScalarAsync()) != 0)
                    return ConflictError("Lehengaia ezin da desgaitu plater aktibo baten errezetan erabiltzen den bitartean.");
            }

            const string sqlSoft = "UPDATE lehengaiak SET ezabatua = 1, ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id AND ezabatua = 0;";
            await using var cmd = new MySqlCommand(sqlSoft, kon, tx);
            cmd.Parameters.AddWithValue("@id", id);
            int errenkadak = await cmd.ExecuteNonQueryAsync();
            if (errenkadak == 0)
                return NotFoundError($"Ez da aurkitu {id} ID-a duen lehengai aktiborik.");

            await tx.CommitAsync();
            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Lehengaia desgaitu da (soft delete).");
        }
    }

    /// <summary>
    /// Desgaitutako lehengaia berreskuratzen du (Restore).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE lehengaiak SET ezabatua = 0, ezabatze_data = NULL WHERE id = @id AND ezabatua = 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        int errenkadak = await cmd.ExecuteNonQueryAsync();
        if (errenkadak == 0)
            return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen lehengairik.");

        return Success(new { id, berreskuratua = true }, 200, "Lehengaia berreskuratu da.");
    }

    private static bool UnitateBaliozkoa(string unitatea)
        => new[] { "kg", "g", "l", "ml", "unitatea" }.Contains(unitatea, StringComparer.Ordinal);
}
