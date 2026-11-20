using System.Data;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/fakturak")]
public class FakturakController : BaseApiKontroladorea
{
    public FakturakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Faktura guztien zerrenda lortzen du (salmenta eta erosketa fakturak).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak(
        [FromQuery] string? mota = null,
        [FromQuery] string? egoera = null,
        [FromQuery] bool barneEzabatuak = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = @"
            SELECT id, eskaera_id, faktura_zenbakia, mota, azpimota, kontzeptua, 
                   hornitzailea, hornitzaile_cif, bezero_nif, bezero_izena, bezero_helbidea,
                   oinarri_zergagarria, bez_ehunekoa, bez_zenbatekoa, guztira, 
                   ordainketa_metodoa, egoera, data, oharrak, ezabatua 
            FROM fakturak 
            WHERE 1=1";

        if (!barneEzabatuak) sql += " AND ezabatua = 0";
        if (!string.IsNullOrWhiteSpace(mota)) sql += " AND mota = @mota";
        if (!string.IsNullOrWhiteSpace(egoera)) sql += " AND egoera = @egoera";
        sql += " ORDER BY data DESC, id DESC;";

        var zerrenda = new List<FakturaDTO>();
        await using var cmd = new MySqlCommand(sql, kon);
        if (!string.IsNullOrWhiteSpace(mota)) cmd.Parameters.AddWithValue("@mota", mota);
        if (!string.IsNullOrWhiteSpace(egoera)) cmd.Parameters.AddWithValue("@egoera", egoera);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zerrenda.Add(MapFakturaDto(reader));
        }

        return SuccessList(zerrenda, 200, "Fakturak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Fakturen finantza-laburpena (Irabaziak, Gastuak, Mozkin garbia eta BEZa).
    /// </summary>
    [HttpGet("laburpena")]
    public async Task<IActionResult> LortuFinantzaLaburpena()
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT 
                COALESCE(SUM(CASE WHEN mota = 'Salmenta' AND egoera != 'Ezeztatua' THEN guztira ELSE 0 END), 0) AS salmentak_guztira,
                COALESCE(SUM(CASE WHEN mota = 'Erosketa' AND egoera != 'Ezeztatua' THEN guztira ELSE 0 END), 0) AS gastuak_guztira,
                COALESCE(SUM(CASE WHEN mota = 'Salmenta' AND egoera != 'Ezeztatua' THEN bez_zenbatekoa ELSE 0 END), 0) AS jasotako_bez,
                COALESCE(SUM(CASE WHEN mota = 'Erosketa' AND egoera != 'Ezeztatua' THEN bez_zenbatekoa ELSE 0 END), 0) AS ordaindutako_bez,
                COUNT(CASE WHEN mota = 'Salmenta' AND egoera != 'Ezeztatua' THEN 1 END) AS salmenta_kopurua,
                COUNT(CASE WHEN mota = 'Erosketa' AND egoera != 'Ezeztatua' THEN 1 END) AS gastu_kopurua
            FROM fakturak 
            WHERE ezabatua = 0;";

        await using var cmd = new MySqlCommand(sql, kon);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var laburpena = new FinantzaLaburpenaDto
            {
                SalmentakGuztira = reader.GetDecimal("salmentak_guztira"),
                GastuakGuztira = reader.GetDecimal("gastuak_guztira"),
                JasotakoBez = reader.GetDecimal("jasotako_bez"),
                OrdaindutakoBez = reader.GetDecimal("ordaindutako_bez"),
                SalmentaKopurua = reader.GetInt32("salmenta_kopurua"),
                GastuKopurua = reader.GetInt32("gastu_kopurua")
            };
            return Success(laburpena, 200, "Finantza laburpena ondo kalkulatu da.");
        }

        return Success(new FinantzaLaburpenaDto(), 200, "Ez da fakturarik aurkitu.");
    }

    /// <summary>
    /// Faktura bakarra lortzen du ID bidez.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT id, eskaera_id, faktura_zenbakia, mota, azpimota, kontzeptua, 
                   hornitzailea, hornitzaile_cif, bezero_nif, bezero_izena, bezero_helbidea,
                   oinarri_zergagarria, bez_ehunekoa, bez_zenbatekoa, guztira, 
                   ordainketa_metodoa, egoera, data, oharrak, ezabatua 
            FROM fakturak 
            WHERE id = @id LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu {id} ID-a duen fakturarik.");

        return Success(MapFakturaDto(reader), 200, "Faktura aurkitu da.");
    }

    /// <summary>
    /// Faktura berria erregistratzen du (Salmenta edo Hornitzailearen Erosketa/Gastua).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] FakturaSortuDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Kontzeptua))
            return BadRequestError("Kontzeptua derrigorrezkoa da.");

        // BEZ eta totalak kalkulatu
        decimal oinarria = Math.Max(0, dto.OinarriZergagarria);
        decimal bezEhunekoa = dto.BezEhunekoa > 0 ? dto.BezEhunekoa : 10.00m;
        decimal bezZenbatekoa = Math.Round(oinarria * (bezEhunekoa / 100m), 2);
        decimal guztira = dto.Guztira.HasValue && dto.Guztira.Value > 0
            ? dto.Guztira.Value
            : oinarria + bezZenbatekoa;

        // Faktura zenbakia automatikoki sortu ez bada zehaztu
        string zenbakia = !string.IsNullOrWhiteSpace(dto.FakturaZenbakia)
            ? dto.FakturaZenbakia.Trim()
            : $"F{DateTime.Now.Year}-{(dto.Mota == "Erosketa" ? "G" : "S")}{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:D6}";

        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            INSERT INTO fakturak 
            (eskaera_id, faktura_zenbakia, mota, azpimota, kontzeptua, 
             hornitzailea, hornitzaile_cif, bezero_nif, bezero_izena, bezero_helbidea,
             oinarri_zergagarria, bez_ehunekoa, bez_zenbatekoa, guztira, 
             ordainketa_metodoa, egoera, data, oharrak)
            VALUES 
            (@eskaera_id, @zenbakia, @mota, @azpimota, @kontzeptua, 
             @hornitzailea, @hornitzaile_cif, @bezero_nif, @bezero_izena, @bezero_helbidea,
             @oinarria, @bezEhunekoa, @bezZenbatekoa, @guztira, 
             @ordainketa_metodoa, @egoera, @data, @oharrak);
            SELECT LAST_INSERT_ID();";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@eskaera_id", (object?)dto.EskaeraId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@zenbakia", zenbakia);
        cmd.Parameters.AddWithValue("@mota", dto.Mota);
        cmd.Parameters.AddWithValue("@azpimota", dto.Azpimota);
        cmd.Parameters.AddWithValue("@kontzeptua", dto.Kontzeptua.Trim());
        cmd.Parameters.AddWithValue("@hornitzailea", (object?)dto.Hornitzailea ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@hornitzaile_cif", (object?)dto.HornitzaileCif ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@bezero_nif", (object?)dto.BezeroNif ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@bezero_izena", (object?)dto.BezeroIzena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@bezero_helbidea", (object?)dto.BezeroHelbidea ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@oinarria", oinarria);
        cmd.Parameters.AddWithValue("@bezEhunekoa", bezEhunekoa);
        cmd.Parameters.AddWithValue("@bezZenbatekoa", bezZenbatekoa);
        cmd.Parameters.AddWithValue("@guztira", guztira);
        cmd.Parameters.AddWithValue("@ordainketa_metodoa", dto.OrdainketaMetodoa ?? "Txartela");
        cmd.Parameters.AddWithValue("@egoera", dto.Egoera ?? "Ordaindua");
        cmd.Parameters.AddWithValue("@data", dto.Data ?? DateTime.Now);
        cmd.Parameters.AddWithValue("@oharrak", (object?)dto.Oharrak ?? DBNull.Value);

        try
        {
            int berriaId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            var emaitza = new FakturaDTO
            {
                Id = berriaId,
                EskaeraId = dto.EskaeraId,
                FakturaZenbakia = zenbakia,
                Mota = dto.Mota,
                Azpimota = dto.Azpimota,
                Kontzeptua = dto.Kontzeptua.Trim(),
                Hornitzailea = dto.Hornitzailea,
                HornitzaileCif = dto.HornitzaileCif,
                BezeroNif = dto.BezeroNif,
                BezeroIzena = dto.BezeroIzena,
                BezeroHelbidea = dto.BezeroHelbidea,
                OinarriZergagarria = oinarria,
                BezEhunekoa = bezEhunekoa,
                BezZenbatekoa = bezZenbatekoa,
                Guztira = guztira,
                OrdainketaMetodoa = dto.OrdainketaMetodoa ?? "Txartela",
                Egoera = dto.Egoera ?? "Ordaindua",
                Data = dto.Data ?? DateTime.Now,
                Oharrak = dto.Oharrak,
                Ezabatua = false
            };

            return CreatedResult(emaitza, 201, "Faktura arrakastaz sortu da.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            return ConflictError($"Dagoeneko existitzen da '{zenbakia}' zenbakia duen faktura bat.");
        }
    }

    /// <summary>
    /// Fakturaren egoera aldatzen du ('Ordaindua', 'Zain', 'Ezeztatua').
    /// </summary>
    [HttpPut("{id:int}/egoera")]
    public async Task<IActionResult> EguneratuEgoera(int id, [FromBody] FakturaEgoeraEguneratuDto dto)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "UPDATE fakturak SET egoera = @egoera WHERE id = @id AND ezabatua = 0;";
        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@egoera", dto.Egoera);
        cmd.Parameters.AddWithValue("@id", id);

        int errenkadak = await cmd.ExecuteNonQueryAsync();
        if (errenkadak == 0)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen faktura aktiborik.");

        return Success(new { id, egoera = dto.Egoera }, 200, "Fakturaren egoera eguneratu da.");
    }

    /// <summary>
    /// Faktura ezabatzen du (Soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        await using var kon = await SortuKonexioaAsync();
        string sql = hard
            ? "DELETE FROM fakturak WHERE id = @id;"
            : "UPDATE fakturak SET ezabatua = 1, ezabatze_data = CURRENT_TIMESTAMP WHERE id = @id;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);

        int errenkadak = await cmd.ExecuteNonQueryAsync();
        if (errenkadak == 0)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen fakturarik.");

        return Success(new { id, ezabatua = true }, 200, "Faktura ezabatu da.");
    }

    private static FakturaDTO MapFakturaDto(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        EskaeraId = reader.IsDBNull("eskaera_id") ? null : reader.GetInt32("eskaera_id"),
        FakturaZenbakia = reader.GetString("faktura_zenbakia"),
        Mota = reader.GetString("mota"),
        Azpimota = reader.GetString("azpimota"),
        Kontzeptua = reader.GetString("kontzeptua"),
        Hornitzailea = reader.IsDBNull("hornitzailea") ? null : reader.GetString("hornitzailea"),
        HornitzaileCif = reader.IsDBNull("hornitzaile_cif") ? null : reader.GetString("hornitzaile_cif"),
        BezeroNif = reader.IsDBNull("bezero_nif") ? null : reader.GetString("bezero_nif"),
        BezeroIzena = reader.IsDBNull("bezero_izena") ? null : reader.GetString("bezero_izena"),
        BezeroHelbidea = reader.IsDBNull("bezero_helbidea") ? null : reader.GetString("bezero_helbidea"),
        OinarriZergagarria = reader.GetDecimal("oinarri_zergagarria"),
        BezEhunekoa = reader.GetDecimal("bez_ehunekoa"),
        BezZenbatekoa = reader.GetDecimal("bez_zenbatekoa"),
        Guztira = reader.GetDecimal("guztira"),
        OrdainketaMetodoa = reader.GetString("ordainketa_metodoa"),
        Egoera = reader.GetString("egoera"),
        Data = reader.GetDateTime("data"),
        Oharrak = reader.IsDBNull("oharrak") ? null : reader.GetString("oharrak"),
        Ezabatua = reader.GetBoolean("ezabatua")
    };
}
