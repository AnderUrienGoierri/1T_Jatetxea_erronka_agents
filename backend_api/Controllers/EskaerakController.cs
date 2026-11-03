using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/eskaerak")]
public class EskaerakController : BaseApiKontroladorea
{
    public EskaerakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Eskaera guztien zerrenda lortzen du (mahaiaren edo egoeraren arabera iragazgarria).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak(
        [FromQuery] int? mahaiaId = null,
        [FromQuery] string? egoera = null,
        [FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = @"
            SELECT e.id, e.mahaia_id, m.zenbakia AS mahai_zenbakia,
                   e.langilea_id, l.izena AS langile_izena,
                   e.egoera, e.guztira, e.ezabatua, e.sortze_data
            FROM eskaerak e
            JOIN mahaiak m ON e.mahaia_id = m.id
            JOIN langileak l ON e.langilea_id = l.id
            WHERE 1=1";

        if (!barneEzabatuak) sql += " AND e.ezabatua = 0";
        if (mahaiaId.HasValue && mahaiaId.Value > 0) sql += " AND e.mahaia_id = @mahaiaId";
        if (!string.IsNullOrWhiteSpace(egoera)) sql += " AND e.egoera = @egoera";
        sql += " ORDER BY e.sortze_data DESC;";

        var zerrenda = new List<EskaeraDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        if (mahaiaId.HasValue && mahaiaId.Value > 0) cmd.Parameters.AddWithValue("@mahaiaId", mahaiaId.Value);
        if (!string.IsNullOrWhiteSpace(egoera)) cmd.Parameters.AddWithValue("@egoera", egoera);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(new EskaeraDTO
            {
                Id = reader.GetInt32("id"),
                MahaiaId = reader.GetInt32("mahaia_id"),
                MahaiZenbakia = reader.GetInt32("mahai_zenbakia"),
                LangileaId = reader.GetInt32("langilea_id"),
                LangileIzena = reader.GetString("langile_izena"),
                Egoera = reader.GetString("egoera"),
                Guztira = reader.GetDecimal("guztira"),
                Ezabatua = reader.GetBoolean("ezabatua"),
                SortzeData = reader.GetDateTime("sortze_data").ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        return SuccessList(zerrenda, 200, "Eskaerak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Eskaera bakarra lortzen du bere lerro guztiekin (komanda osoa).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();

        // 1. Eskaeraren burua
        const string sqlEskaera = @"
            SELECT e.id, e.mahaia_id, m.zenbakia AS mahai_zenbakia,
                   e.langilea_id, l.izena AS langile_izena,
                   e.egoera, e.guztira, e.ezabatua, e.sortze_data
            FROM eskaerak e
            JOIN mahaiak m ON e.mahaia_id = m.id
            JOIN langileak l ON e.langilea_id = l.id
            WHERE e.id = @id LIMIT 1;";

        EskaeraDTO dto;
        await using (var cmdE = new MySqlCommand(sqlEskaera, kon))
        {
            cmdE.Parameters.AddWithValue("@id", id);
            await using var readerE = await cmdE.ExecuteReaderAsync();
            if (!await readerE.ReadAsync())
                return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaerarik.");

            dto = new EskaeraDTO
            {
                Id = readerE.GetInt32("id"),
                MahaiaId = readerE.GetInt32("mahaia_id"),
                MahaiZenbakia = readerE.GetInt32("mahai_zenbakia"),
                LangileaId = readerE.GetInt32("langilea_id"),
                LangileIzena = readerE.GetString("langile_izena"),
                Egoera = readerE.GetString("egoera"),
                Guztira = readerE.GetDecimal("guztira"),
                Ezabatua = readerE.GetBoolean("ezabatua"),
                SortzeData = readerE.GetDateTime("sortze_data").ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        // 2. Eskaera lerroak
        const string sqlLerroak = @"
            SELECT el.id, el.eskaera_id, el.platera_id, p.izena AS plater_izena,
                   el.kantitatea, el.unitate_prezioa, el.egoera, el.oharrak
            FROM eskaera_lerroak el
            JOIN platerak p ON el.platera_id = p.id
            WHERE el.eskaera_id = @id
            ORDER BY el.id ASC;";

        await using (var cmdL = new MySqlCommand(sqlLerroak, kon))
        {
            cmdL.Parameters.AddWithValue("@id", id);
            await using var readerL = await cmdL.ExecuteReaderAsync();
            while (await readerL.ReadAsync())
            {
                dto.Lerroak.Add(new EskaeraLerroaDTO
                {
                    Id = readerL.GetInt32("id"),
                    EskaeraId = readerL.GetInt32("eskaera_id"),
                    PlateraId = readerL.GetInt32("platera_id"),
                    PlaterIzena = readerL.GetString("plater_izena"),
                    Kantitatea = readerL.GetInt32("kantitatea"),
                    UnitatePrezioa = readerL.GetDecimal("unitate_prezioa"),
                    Egoera = readerL.GetString("egoera"),
                    Oharrak = readerL.IsDBNull("oharrak") ? null : readerL.GetString("oharrak")
                });
            }
        }

        return Success(dto, 200, "Eskaera ondo berreskuratu da.");
    }

    /// <summary>
    /// Eskaera / Komanda berria sortzen du mahai batean.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] EskaeraSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("MahaiaId eta LangileaId beharrezkoak dira.");

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // Egiaztatu mahaia aktibo dagoela eta blokeatu
            const string sqlMahaia = "SELECT zenbakia, egoera, ezabatua FROM mahaiak WHERE id = @mId FOR UPDATE;";
            int mahaiZenbakia;
            await using (var cmdM = new MySqlCommand(sqlMahaia, kon, tx))
            {
                cmdM.Parameters.AddWithValue("@mId", dto.MahaiaId);
                await using var readerM = await cmdM.ExecuteReaderAsync();
                if (!await readerM.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {dto.MahaiaId} ID-a duen mahaia.");
                }
                if (readerM.GetBoolean("ezabatua"))
                {
                    await tx.RollbackAsync();
                    return ConflictError("Mahaia desgaituta dago.");
                }
                mahaiZenbakia = readerM.GetInt32("zenbakia");
            }

            // Egiaztatu langilea aktibo dagoela
            const string sqlLangilea = "SELECT izena FROM langileak WHERE id = @lId AND ezabatua = 0 AND aktibo = 1;";
            string langileIzena;
            await using (var cmdL = new MySqlCommand(sqlLangilea, kon, tx))
            {
                cmdL.Parameters.AddWithValue("@lId", dto.LangileaId);
                var izenaObj = await cmdL.ExecuteScalarAsync();
                if (izenaObj == null)
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {dto.LangileaId} ID-a duen langile aktiborik.");
                }
                langileIzena = izenaObj.ToString()!;
            }

            // Mahaiaren egoera 'Okupatuta'-ra pasa
            const string sqlUpdMahaia = "UPDATE mahaiak SET egoera = 'Okupatuta' WHERE id = @mId;";
            await using (var cmdUpdM = new MySqlCommand(sqlUpdMahaia, kon, tx))
            {
                cmdUpdM.Parameters.AddWithValue("@mId", dto.MahaiaId);
                await cmdUpdM.ExecuteNonQueryAsync();
            }

            // Eskaera txertatu
            const string sqlInsEskaera = @"
                INSERT INTO eskaerak (mahaia_id, langilea_id, egoera, guztira, ezabatua, sortze_data)
                VALUES (@mId, @lId, 'Irekia', 0.00, 0, CURRENT_TIMESTAMP);
                SELECT LAST_INSERT_ID();";

            int eskaeraId;
            await using (var cmdInsE = new MySqlCommand(sqlInsEskaera, kon, tx))
            {
                cmdInsE.Parameters.AddWithValue("@mId", dto.MahaiaId);
                cmdInsE.Parameters.AddWithValue("@lId", dto.LangileaId);
                eskaeraId = Convert.ToInt32(await cmdInsE.ExecuteScalarAsync());
            }

            await tx.CommitAsync();

            var emaitza = new EskaeraDTO
            {
                Id = eskaeraId,
                MahaiaId = dto.MahaiaId,
                MahaiZenbakia = mahaiZenbakia,
                LangileaId = dto.LangileaId,
                LangileIzena = langileIzena,
                Egoera = "Irekia",
                Guztira = 0.00m,
                Ezabatua = false,
                SortzeData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreatedResult(emaitza, 201, "Eskaera berria arrakastaz ireki da.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea eskaera sortzean: " + ex.Message);
        }
    }

    /// <summary>
    /// GAKO TRANSAKZIOA: Platera gehitu komandara eta lehengaien stocka deskontatu pesimistoki (SELECT ... FOR UPDATE).
    /// Prezioa backend-eko datu-basetik soilik irakurtzen da segurtasunagatik.
    /// </summary>
    [HttpPost("{id:int}/lerroak")]
    public async Task<IActionResult> GehituPlateraEtaDeskontatuStock(int id, [FromBody] EskaeraLerroaGehituDto dto)
    {
        if (dto.Kantitatea <= 0)
            return BadRequestError("Kantitateak 0 baino handiagoa izan behar du.");

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Egiaztatu eskaera irekita dagoela
            const string sqlEskaera = "SELECT id, egoera, ezabatua FROM eskaerak WHERE id = @id FOR UPDATE;";
            await using (var cmdE = new MySqlCommand(sqlEskaera, kon, tx))
            {
                cmdE.Parameters.AddWithValue("@id", id);
                await using var readerE = await cmdE.ExecuteReaderAsync();
                if (!await readerE.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaerarik.");
                }

                if (readerE.GetBoolean("ezabatua"))
                {
                    await tx.RollbackAsync();
                    return ConflictError("Eskaera ezabatuta dago.");
                }

                string egoera = readerE.GetString("egoera");
                if (egoera == "Kobratua" || egoera == "Ezeztatua")
                {
                    await tx.RollbackAsync();
                    return ConflictError($"Ezin dira platerak gehitu egoera '{egoera}' duen eskaera batera.");
                }
            }

            // 2. Lortu plateraren prezio ofiziala datu-basetik (Segurtasuna: ez onartu frontendeko preziorik!)
            const string sqlPlatera = "SELECT izena, prezioa, aktibo, ezabatua FROM platerak WHERE id = @plateraId;";
            string platerIzena;
            decimal unitatePrezioa;

            await using (var cmdP = new MySqlCommand(sqlPlatera, kon, tx))
            {
                cmdP.Parameters.AddWithValue("@plateraId", dto.PlateraId);
                await using var readerP = await cmdP.ExecuteReaderAsync();
                if (!await readerP.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {dto.PlateraId} ID-a duen platerik.");
                }

                if (readerP.GetBoolean("ezabatua") || !readerP.GetBoolean("aktibo"))
                {
                    await tx.RollbackAsync();
                    return ConflictError("Platera desgaituta edo aktibo gabe dago.");
                }

                platerIzena = readerP.GetString("izena");
                unitatePrezioa = readerP.GetDecimal("prezioa");
            }

            // 3. Lortu errezetako lehengai guztiak eta BLOKEATU errenkadak (SELECT ... FOR UPDATE)
            const string sqlErrezeta = @"
                SELECT po.lehengaia_id, po.kantitatea AS beharrezkoa_anoko,
                       l.izena, l.uneko_stocka, l.unitatea
                FROM plater_osagaiak po
                JOIN lehengaiak l ON po.lehengaia_id = l.id
                WHERE po.platera_id = @plateraId AND l.ezabatua = 0
                FOR UPDATE;";

            var osagaiak = new List<dynamic>();
            await using (var cmdErrezeta = new MySqlCommand(sqlErrezeta, kon, tx))
            {
                cmdErrezeta.Parameters.AddWithValue("@plateraId", dto.PlateraId);
                await using var readerO = await cmdErrezeta.ExecuteReaderAsync();
                while (await readerO.ReadAsync())
                {
                    osagaiak.Add(new
                    {
                        LehengaiaId = readerO.GetInt32("lehengaia_id"),
                        Izena = readerO.GetString("izena"),
                        Beharrezkoa = readerO.GetDecimal("beharrezkoa_anoko") * dto.Kantitatea,
                        UnekoStocka = readerO.GetDecimal("uneko_stocka"),
                        Unitatea = readerO.GetString("unitatea")
                    });
                }
            }

            // 4. Egiaztatu lehengai GUZTIETATIK stock nahikoa dagoen
            foreach (var osagai in osagaiak)
            {
                if (osagai.UnekoStocka < osagai.Beharrezkoa)
                {
                    await tx.RollbackAsync();
                    return ConflictError(
                        $"Ez dago nahikoa stock '{platerIzena}' prestatzeko: '{osagai.Izena}' lehengaia falta da. " +
                        $"(Behar dira: {osagai.Beharrezkoa} {osagai.Unitatea}, Unean: {osagai.UnekoStocka} {osagai.Unitatea}).");
                }
            }

            // 5. Deskontatu lehengaien stocka eta erregistratu auditoria (stock_mugimenduak)
            foreach (var osagai in osagaiak)
            {
                const string sqlUpdateStock = @"
                    UPDATE lehengaiak 
                    SET uneko_stocka = uneko_stocka - @gastua 
                    WHERE id = @lehengaiaId;

                    INSERT INTO stock_mugimenduak (lehengaia_id, mota, kantitatea, eskaera_id, oharrak, data)
                    VALUES (@lehengaiaId, 'Kontsumoa_Platera', -@gastua, @eskaeraId, @oharra, CURRENT_TIMESTAMP);";

                await using var cmdUpdStock = new MySqlCommand(sqlUpdateStock, kon, tx);
                cmdUpdStock.Parameters.AddWithValue("@gastua", osagai.Beharrezkoa);
                cmdUpdStock.Parameters.AddWithValue("@lehengaiaId", osagai.LehengaiaId);
                cmdUpdStock.Parameters.AddWithValue("@eskaeraId", id);
                cmdUpdStock.Parameters.AddWithValue("@oharra", $"Plater kontsumoa: {platerIzena} x{dto.Kantitatea}");
                await cmdUpdStock.ExecuteNonQueryAsync();
            }

            // 6. Gehitu lerroa komandara
            const string sqlInsertLerroa = @"
                INSERT INTO eskaera_lerroak (eskaera_id, platera_id, kantitatea, unitate_prezioa, egoera, oharrak)
                VALUES (@eskaeraId, @plateraId, @kantitatea, @unitatePrezioa, 'Eskatua', @oharrak);
                SELECT LAST_INSERT_ID();";

            int lerroId;
            await using (var cmdInsL = new MySqlCommand(sqlInsertLerroa, kon, tx))
            {
                cmdInsL.Parameters.AddWithValue("@eskaeraId", id);
                cmdInsL.Parameters.AddWithValue("@plateraId", dto.PlateraId);
                cmdInsL.Parameters.AddWithValue("@kantitatea", dto.Kantitatea);
                cmdInsL.Parameters.AddWithValue("@unitatePrezioa", unitatePrezioa);
                cmdInsL.Parameters.AddWithValue("@oharrak", (object?)dto.Oharrak ?? DBNull.Value);

                lerroId = Convert.ToInt32(await cmdInsL.ExecuteScalarAsync());
            }

            // 7. Eguneratu eskaeraren guztira zenbatekoa
            decimal gehituGuztira = unitatePrezioa * dto.Kantitatea;
            const string sqlUpdGuztira = "UPDATE eskaerak SET guztira = guztira + @gehitu WHERE id = @eskaeraId;";
            await using (var cmdUpdG = new MySqlCommand(sqlUpdGuztira, kon, tx))
            {
                cmdUpdG.Parameters.AddWithValue("@gehitu", gehituGuztira);
                cmdUpdG.Parameters.AddWithValue("@eskaeraId", id);
                await cmdUpdG.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();

            var emaitzaLerroa = new EskaeraLerroaDTO
            {
                Id = lerroId,
                EskaeraId = id,
                PlateraId = dto.PlateraId,
                PlaterIzena = platerIzena,
                Kantitatea = dto.Kantitatea,
                UnitatePrezioa = unitatePrezioa,
                Egoera = "Eskatua",
                Oharrak = dto.Oharrak
            };

            return CreatedResult(emaitzaLerroa, 201, $"'{platerIzena}' platera eskaerara gehitu da eta lehengaien stocka deskontatu da.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea platera eskatzean eta stocka deskontatzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Eskaeraren egoera eguneratzen du ('Irekia', 'Sukaldean', 'Zerbitzatua', 'Kobratua', 'Ezeztatua').
    /// </summary>
    [HttpPut("{id:int}/egoera")]
    public async Task<IActionResult> EguneratuEgoera(int id, [FromBody] EskaeraEgoeraEguneratuDto dto)
    {
        var baliozkoak = new[] { "Irekia", "Sukaldean", "Zerbitzatua", "Kobratua", "Ezeztatua" };
        if (!baliozkoak.Contains(dto.Egoera))
            return BadRequestError($"Egoera ez da baliozkoa. Balio onargarriak: {string.Join(", ", baliozkoak)}");

        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE eskaerak SET egoera = @egoera WHERE id = @id AND ezabatua = 0;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@egoera", dto.Egoera);
        cmd.Parameters.AddWithValue("@id", id);

        int rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaera aktiborik.");

        // Kobratua edo Ezeztatua bada, mahaia 'Garbitzen' edo 'Libre' jarri
        if (dto.Egoera == "Kobratua")
        {
            const string sqlMahaiaGarbitzen = @"
                UPDATE mahaiak m
                JOIN eskaerak e ON m.id = e.mahaia_id
                SET m.egoera = 'Garbitzen'
                WHERE e.id = @id;";
            await using var cmdM = new MySqlCommand(sqlMahaiaGarbitzen, kon);
            cmdM.Parameters.AddWithValue("@id", id);
            await cmdM.ExecuteNonQueryAsync();
        }

        return Success(new { id, egoera = dto.Egoera }, 200, "Eskaeraren egoera eguneratu da.");
    }

    /// <summary>
    /// Sukaldeko pantailarentzat (KDS): eskaera-lerro baten egoera eguneratzen du ('Eskatua', 'Sukaldean', 'Prestatua', 'Zerbitzatua').
    /// </summary>
    [HttpPut("lerroak/{lerroId:int}/egoera")]
    public async Task<IActionResult> EguneratuLerroEgoera(int lerroId, [FromBody] EskaeraLerroEgoeraEguneratuDto dto)
    {
        var baliozkoak = new[] { "Eskatua", "Sukaldean", "Prestatua", "Zerbitzatua" };
        if (!baliozkoak.Contains(dto.Egoera))
            return BadRequestError($"Egoera ez da baliozkoa. Balio onargarriak: {string.Join(", ", baliozkoak)}");

        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE eskaera_lerroak SET egoera = @egoera WHERE id = @lerroId;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@egoera", dto.Egoera);
        cmd.Parameters.AddWithValue("@lerroId", lerroId);

        int rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0) return NotFoundError($"Ez da aurkitu {lerroId} ID-a duen eskaera lerrorik.");

        return Success(new { lerroId, egoera = dto.Egoera }, 200, "Eskaera lerroaren egoera eguneratu da.");
    }

    /// <summary>
    /// Eskaera ezabatzen du (Soft Delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE eskaerak SET ezabatua = 1, egoera = 'Ezeztatua', ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id AND ezabatua = 0;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0) return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaera aktiborik.");

        return Success(new { id, ezabatua = true }, 200, "Eskaera ezabatu da (soft delete).");
    }
}
