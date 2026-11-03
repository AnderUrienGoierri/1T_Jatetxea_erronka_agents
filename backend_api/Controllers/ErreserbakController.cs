using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/erreserbak")]
public class ErreserbakController : BaseApiKontroladorea
{
    public ErreserbakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Erreserben zerrenda lortzen du (dataren, txandaren eta ezabatuen arabera iragazgarria).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak(
        [FromQuery] string? data = null,
        [FromQuery] string? txanda = null,
        [FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = @"
            SELECT e.id, e.bezeroa_id, b.izena AS bezero_izena, b.abizena AS bezero_abizena,
                   b.emaila AS bezero_emaila, b.telefonoa AS bezero_telefonoa,
                   e.mahaia_id, m.zenbakia AS mahai_zenbakia, m.kokapena,
                   e.data, e.txanda, e.pertsona_kopurua, e.egoera, e.oharrak, e.ezabatua
            FROM erreserbak e
            JOIN bezeroak b ON e.bezeroa_id = b.id
            JOIN mahaiak m ON e.mahaia_id = m.id
            WHERE 1=1";

        if (!barneEzabatuak) sql += " AND e.ezabatua = 0";
        if (!string.IsNullOrWhiteSpace(data)) sql += " AND e.data = @data";
        if (!string.IsNullOrWhiteSpace(txanda)) sql += " AND e.txanda = @txanda";
        sql += " ORDER BY e.data DESC, e.txanda ASC, m.zenbakia ASC;";

        var zerrenda = new List<ErreserbaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        if (!string.IsNullOrWhiteSpace(data)) cmd.Parameters.AddWithValue("@data", data);
        if (!string.IsNullOrWhiteSpace(txanda)) cmd.Parameters.AddWithValue("@txanda", txanda);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new ErreserbaDTO
            {
                Id = reader.GetInt32("id"),
                BezeroaId = reader.GetInt32("bezeroa_id"),
                BezeroIzena = reader.GetString("bezero_izena"),
                BezeroAbizena = reader.GetString("bezero_abizena"),
                BezeroEmaila = reader.GetString("bezero_emaila"),
                BezeroTelefonoa = reader.GetString("bezero_telefonoa"),
                MahaiaId = reader.GetInt32("mahaia_id"),
                MahaiZenbakia = reader.GetInt32("mahai_zenbakia"),
                Kokapena = reader.GetString("kokapena"),
                Data = reader.GetDateTime("data").ToString("yyyy-MM-dd"),
                Txanda = reader.GetString("txanda"),
                PertsonaKopurua = reader.GetInt32("pertsona_kopurua"),
                Egoera = reader.GetString("egoera"),
                Oharrak = reader.IsDBNull("oharrak") ? null : reader.GetString("oharrak"),
                Ezabatua = reader.GetBoolean("ezabatua")
            });
        }

        return SuccessList(zerrenda, 200, "Erreserbak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Erreserba bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT e.id, e.bezeroa_id, b.izena AS bezero_izena, b.abizena AS bezero_abizena,
                   b.emaila AS bezero_emaila, b.telefonoa AS bezero_telefonoa,
                   e.mahaia_id, m.zenbakia AS mahai_zenbakia, m.kokapena,
                   e.data, e.txanda, e.pertsona_kopurua, e.egoera, e.oharrak, e.ezabatua
            FROM erreserbak e
            JOIN bezeroak b ON e.bezeroa_id = b.id
            JOIN mahaiak m ON e.mahaia_id = m.id
            WHERE e.id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen erreserbarik.");

        var dto = new ErreserbaDTO
        {
            Id = reader.GetInt32("id"),
            BezeroaId = reader.GetInt32("bezeroa_id"),
            BezeroIzena = reader.GetString("bezero_izena"),
            BezeroAbizena = reader.GetString("bezero_abizena"),
            BezeroEmaila = reader.GetString("bezero_emaila"),
            BezeroTelefonoa = reader.GetString("bezero_telefonoa"),
            MahaiaId = reader.GetInt32("mahaia_id"),
            MahaiZenbakia = reader.GetInt32("mahai_zenbakia"),
            Kokapena = reader.GetString("kokapena"),
            Data = reader.GetDateTime("data").ToString("yyyy-MM-dd"),
            Txanda = reader.GetString("txanda"),
            PertsonaKopurua = reader.GetInt32("pertsona_kopurua"),
            Egoera = reader.GetString("egoera"),
            Oharrak = reader.IsDBNull("oharrak") ? null : reader.GetString("oharrak"),
            Ezabatua = reader.GetBoolean("ezabatua")
        };

        return Success(dto, 200, "Erreserba ondo aurkitu da.");
    }

    /// <summary>
    /// Erreserba berria egiten du transakzio pesimistan (SELECT ... FOR UPDATE bidez).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] ErreserbaSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("Datu baliogabeak bidali dira.");

        await using var kon = await SortuKonexioaAsync();

        // 1. Mahaia zehaztu (ID bidez edo Zenbaki bidez)
        int mahaiaId = dto.MahaiaId ?? 0;
        if (mahaiaId <= 0 && dto.MahaiZenbakia.HasValue && dto.MahaiZenbakia.Value > 0)
        {
            const string sqlM = "SELECT id FROM mahaiak WHERE zenbakia = @zenbaki AND ezabatua = 0 LIMIT 1;";
            await using var cmdM = new MySqlCommand(sqlM, kon);
            cmdM.Parameters.AddWithValue("@zenbaki", dto.MahaiZenbakia.Value);
            var mId = await cmdM.ExecuteScalarAsync();
            if (mId != null) mahaiaId = Convert.ToInt32(mId);
        }

        if (mahaiaId <= 0)
            return BadRequestError("Mahaia zehaztu behar da (MahaiaId edo MahaiZenbakia).");

        // 2. Bezeroa zehaztu edo automatikoki erregistratu
        int bezeroaId = dto.BezeroaId ?? 0;
        string bezeroIzena = dto.BezeroIzena ?? "Bezeroa";
        string bezeroAbizena = dto.BezeroAbizena ?? "-";
        string bezeroEmaila = dto.BezeroEmaila ?? "";
        string bezeroTelefonoa = dto.BezeroTelefonoa ?? "-";

        if (bezeroaId <= 0 && !string.IsNullOrWhiteSpace(dto.BezeroEmaila))
        {
            const string sqlB = "SELECT id, izena, abizena, telefonoa FROM bezeroak WHERE emaila = @email LIMIT 1;";
            await using var cmdB = new MySqlCommand(sqlB, kon);
            cmdB.Parameters.AddWithValue("@email", dto.BezeroEmaila.Trim().ToLower());
            await using var readerB = await cmdB.ExecuteReaderAsync();
            if (await readerB.ReadAsync())
            {
                bezeroaId = readerB.GetInt32("id");
                bezeroIzena = readerB.GetString("izena");
                bezeroAbizena = readerB.GetString("abizena");
                bezeroTelefonoa = readerB.GetString("telefonoa");
            }
            await readerB.CloseAsync();

            if (bezeroaId <= 0)
            {
                const string sqlInsB = @"
                    INSERT INTO bezeroak (izena, abizena, telefonoa, emaila, ezabatua, sortze_data)
                    VALUES (@izena, @abizena, @telefonoa, @emaila, 0, CURRENT_TIMESTAMP);
                    SELECT LAST_INSERT_ID();";
                await using var cmdInsB = new MySqlCommand(sqlInsB, kon);
                cmdInsB.Parameters.AddWithValue("@izena", bezeroIzena);
                cmdInsB.Parameters.AddWithValue("@abizena", bezeroAbizena);
                cmdInsB.Parameters.AddWithValue("@telefonoa", bezeroTelefonoa);
                cmdInsB.Parameters.AddWithValue("@emaila", dto.BezeroEmaila.Trim().ToLower());
                bezeroaId = Convert.ToInt32(await cmdInsB.ExecuteScalarAsync());
            }
        }

        if (bezeroaId <= 0)
            return BadRequestError("Bezeroa zehaztu behar da (BezeroaId edo BezeroEmaila bidaliz).");

        // 3. Transakzio pesimista hasi
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // Mahaia blokeatu eta erabilgarritasuna egiaztatu
            const string sqlBlokeatuMahaia = "SELECT zenbakia, edukiera, kokapena, ezabatua FROM mahaiak WHERE id = @mId FOR UPDATE;";
            int mahaiZenbakia;
            string kokapena;
            int edukiera;

            await using (var cmdLock = new MySqlCommand(sqlBlokeatuMahaia, kon, tx))
            {
                cmdLock.Parameters.AddWithValue("@mId", mahaiaId);
                await using var readerM = await cmdLock.ExecuteReaderAsync();
                if (!await readerM.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {mahaiaId} ID-a duen mahaia.");
                }

                if (readerM.GetBoolean("ezabatua"))
                {
                    await tx.RollbackAsync();
                    return ConflictError("Mahaia desgaituta dago. Ezin da erreserbarik egin bertan.");
                }

                mahaiZenbakia = readerM.GetInt32("zenbakia");
                edukiera = readerM.GetInt32("edukiera");
                kokapena = readerM.GetString("kokapena");
            }

            // Data eta txanda berean erreserbarik dagoen egiaztatu (SELECT ... FOR UPDATE)
            const string sqlCheckErreserba = @"
                SELECT id FROM erreserbak 
                WHERE mahaia_id = @mId AND data = @data AND txanda = @txanda AND ezabatua = 0 
                FOR UPDATE;";

            await using (var cmdCheck = new MySqlCommand(sqlCheckErreserba, kon, tx))
            {
                cmdCheck.Parameters.AddWithValue("@mId", mahaiaId);
                cmdCheck.Parameters.AddWithValue("@data", dto.Data);
                cmdCheck.Parameters.AddWithValue("@txanda", dto.Txanda);
                var dagoenId = await cmdCheck.ExecuteScalarAsync();
                if (dagoenId != null)
                {
                    await tx.RollbackAsync();
                    return ConflictError($"{mahaiZenbakia}. mahaia jada erreserbatuta dago {dto.Data} datan eta {dto.Txanda} txandan.");
                }
            }

            // Simulazio-itxaronaldia (konkurrentzia probetarako)
            if (dto.Itxaron > 0)
            {
                await Task.Delay(dto.Itxaron);
            }

            // Erreserba txertatu
            const string sqlInsert = @"
                INSERT INTO erreserbak (bezeroa_id, mahaia_id, data, txanda, pertsona_kopurua, egoera, oharrak, ezabatua, sortze_data)
                VALUES (@bezeroaId, @mahaiaId, @data, @txanda, @pertsonak, 'Baieztatua', @oharrak, 0, CURRENT_TIMESTAMP);
                SELECT LAST_INSERT_ID();";

            int erreserbaId;
            await using (var cmdIns = new MySqlCommand(sqlInsert, kon, tx))
            {
                cmdIns.Parameters.AddWithValue("@bezeroaId", bezeroaId);
                cmdIns.Parameters.AddWithValue("@mahaiaId", mahaiaId);
                cmdIns.Parameters.AddWithValue("@data", dto.Data);
                cmdIns.Parameters.AddWithValue("@txanda", dto.Txanda);
                cmdIns.Parameters.AddWithValue("@pertsonak", dto.PertsonaKopurua);
                cmdIns.Parameters.AddWithValue("@oharrak", (object?)dto.Oharrak ?? DBNull.Value);

                erreserbaId = Convert.ToInt32(await cmdIns.ExecuteScalarAsync());
            }

            await tx.CommitAsync();

            var emaitza = new ErreserbaDTO
            {
                Id = erreserbaId,
                BezeroaId = bezeroaId,
                BezeroIzena = bezeroIzena,
                BezeroAbizena = bezeroAbizena,
                BezeroEmaila = bezeroEmaila,
                BezeroTelefonoa = bezeroTelefonoa,
                MahaiaId = mahaiaId,
                MahaiZenbakia = mahaiZenbakia,
                Kokapena = kokapena,
                Data = dto.Data,
                Txanda = dto.Txanda,
                PertsonaKopurua = dto.PertsonaKopurua,
                Egoera = "Baieztatua",
                Oharrak = dto.Oharrak,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Erreserba arrakastaz baieztatu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            await tx.RollbackAsync();
            return ConflictError($"Erreserba gatazka: mahaia jada erreserbatuta dago data eta txanda horretan.");
        }
        catch (MySqlException ex) when (ex.Number == 1213)
        {
            await tx.RollbackAsync();
            return ConflictError("Deadlock gertatu da aldi bereko erreserbak kudeatzean. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea erreserba egitean: " + ex.Message);
        }
    }

    /// <summary>
    /// Erreserba ezeztatzen edo ezabatzen du (Soft Delete lehenetsia).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();

        if (hard)
        {
            const string sqlHard = "DELETE FROM erreserbak WHERE id = @id;";
            await using var cmd = new MySqlCommand(sqlHard, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen erreserbarik.");
            return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Erreserba behin betiko ezabatu da.");
        }
        else
        {
            const string sqlSoft = @"
                UPDATE erreserbak 
                SET ezabatua = 1, egoera = 'Ezeztatua', ezabatze_data = CURRENT_TIMESTAMP 
                WHERE id = @id AND ezabatua = 0;";
            await using var cmd = new MySqlCommand(sqlSoft, kon);
            cmd.Parameters.AddWithValue("@id", id);
            int rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen erreserba aktiborik.");
            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Erreserba ezeztatu da (soft delete).");
        }
    }

    /// <summary>
    /// Ezeztatutako erreserba berreskuratzen du (mahai librearen egiaztapenarekin).
    /// </summary>
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> Berreskuratu(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            const string sqlDatuak = "SELECT mahaia_id, data, txanda FROM erreserbak WHERE id = @id AND ezabatua = 1 FOR UPDATE;";
            int mahaiaId;
            DateTime data;
            string txanda;

            await using (var cmdD = new MySqlCommand(sqlDatuak, kon, tx))
            {
                cmdD.Parameters.AddWithValue("@id", id);
                await using var readerD = await cmdD.ExecuteReaderAsync();
                if (!await readerD.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu berreskuratzeko {id} ID-a duen erreserbarik.");
                }

                mahaiaId = readerD.GetInt32("mahaia_id");
                data = readerD.GetDateTime("data");
                txanda = readerD.GetString("txanda");
            }

            // Egiaztatu hutsunea ez ote dagoen okupatuta beste erreserba aktibo batekin
            const string sqlCheck = @"
                SELECT id FROM erreserbak 
                WHERE mahaia_id = @mId AND data = @data AND txanda = @txanda AND ezabatua = 0 LIMIT 1;";

            await using (var cmdC = new MySqlCommand(sqlCheck, kon, tx))
            {
                cmdC.Parameters.AddWithValue("@mId", mahaiaId);
                cmdC.Parameters.AddWithValue("@data", data.ToString("yyyy-MM-dd"));
                cmdC.Parameters.AddWithValue("@txanda", txanda);
                var okupatuta = await cmdC.ExecuteScalarAsync();
                if (okupatuta != null)
                {
                    await tx.RollbackAsync();
                    return ConflictError("Ezin da berreskuratu: mahaia jada beste erreserba aktibo batek okupatu du txanda horretan.");
                }
            }

            const string sqlRestore = @"
                UPDATE erreserbak 
                SET ezabatua = 0, egoera = 'Baieztatua', ezabatze_data = NULL 
                WHERE id = @id;";

            await using (var cmdR = new MySqlCommand(sqlRestore, kon, tx))
            {
                cmdR.Parameters.AddWithValue("@id", id);
                await cmdR.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
            return Success(new { id, berreskuratua = true }, 200, "Erreserba arrakastaz berreskuratu da.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea erreserba berreskuratzean: " + ex.Message);
        }
    }
}
