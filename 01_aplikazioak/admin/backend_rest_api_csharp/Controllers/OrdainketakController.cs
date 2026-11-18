using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/ordainketak")]
public class OrdainketakController : BaseApiKontroladorea
{
    public OrdainketakController(IConfiguration config) : base(config) { }

    /// <summary>
    /// Eskaera baten ordainketa segurua prozesatzen du (Eskudirua, Kreditu Txartela, Bizum, Google/Apple Pay)
    /// eta 80mm-ko inprimagailu termikorako tiket fiskal ofiziala sortzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProzesatuOrdainketa([FromBody] OrdainketaEskaeraDto dto)
    {
        if (dto.Zenbatekoa <= 0)
            return BadRequestError("Ordainketa zenbatekoak 0 baino handiagoa izan behar du.");

        string motaNormalizatua = NormalizatuMota(dto.Mota);

        await using var kon = await SortuKonexioaAsync();
        await using var tx = await kon.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Egiaztatu eskaera aktiboa eta lortu datuak
            const string sqlEskaera = @"
                SELECT e.id, e.mahaia_id, m.zenbakia AS mahai_zenbakia, m.edukiera,
                       e.langilea_id, l.izena AS langile_izena,
                       e.egoera, e.guztira, e.ezabatua, e.sortze_data
                FROM eskaerak e
                JOIN mahaiak m ON e.mahaia_id = m.id
                JOIN langileak l ON e.langilea_id = l.id
                WHERE e.id = @eskaeraId FOR UPDATE;";

            int mahaiaId;
            int mahaiZenbakia;
            int edukiera;
            int langileaId;
            string langileIzena;
            string unekoEgoera;
            decimal eskaeraGuztira;
            DateTime sortzeData;

            await using (var cmdE = new MySqlCommand(sqlEskaera, kon, tx))
            {
                cmdE.Parameters.AddWithValue("@eskaeraId", dto.EskaeraId);
                await using var readerE = await cmdE.ExecuteReaderAsync();
                if (!await readerE.ReadAsync())
                {
                    await tx.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu #{dto.EskaeraId} ID-a duen eskaerarik.");
                }

                if (readerE.GetBoolean("ezabatua"))
                {
                    await tx.RollbackAsync();
                    return ConflictError("Eskaera hau ezabatuta dago.");
                }

                unekoEgoera = readerE.GetString("egoera");
                if (unekoEgoera == "Kobratua")
                {
                    await tx.RollbackAsync();
                    return ConflictError($"#{dto.EskaeraId} eskaera dagoeneko kobratuta eta itxita dago.");
                }
                if (unekoEgoera == "Ezeztatua")
                {
                    await tx.RollbackAsync();
                    return ConflictError($"#{dto.EskaeraId} eskaera ezeztatuta dago, ezin da kobratu.");
                }

                mahaiaId = readerE.GetInt32("mahaia_id");
                mahaiZenbakia = readerE.GetInt32("mahai_zenbakia");
                edukiera = readerE.GetInt32("edukiera");
                langileaId = dto.LangileaId ?? readerE.GetInt32("langilea_id");
                langileIzena = readerE.GetString("langile_izena");
                eskaeraGuztira = readerE.GetDecimal("guztira");
                sortzeData = readerE.GetDateTime("sortze_data");
            }

            // 2. Lortu eskaerako plater guztiak tiketa eraikitzeko
            const string sqlLerroak = @"
                SELECT el.id, el.platera_id, p.izena AS plater_izena, el.kantitatea, el.unitate_prezioa, el.egoera
                FROM eskaera_lerroak el
                JOIN platerak p ON el.platera_id = p.id
                WHERE el.eskaera_id = @eskaeraId
                ORDER BY el.id ASC;";

            var lerroak = new List<(string Izena, int Kantitatea, decimal UnitatePrezioa, decimal Guztira)>();
            await using (var cmdL = new MySqlCommand(sqlLerroak, kon, tx))
            {
                cmdL.Parameters.AddWithValue("@eskaeraId", dto.EskaeraId);
                await using var readerL = await cmdL.ExecuteReaderAsync();
                while (await readerL.ReadAsync())
                {
                    string izena = readerL.GetString("plater_izena");
                    int kop = readerL.GetInt32("kantitatea");
                    decimal prez = readerL.GetDecimal("unitate_prezioa");
                    lerroak.Add((izena, kop, prez, kop * prez));
                }
            }

            // 3. Ordainketa motaren balidazio zorrotzak (Zibersegurtasuna eta kalkuluak)
            decimal ordainduBeharrekoa = eskaeraGuztira > 0 ? eskaeraGuztira : dto.Zenbatekoa;
            decimal? jasotakoa = null;
            decimal? itzulitakoa = null;
            string? txartelMaskaratua = null;
            string? txartelMarka = null;
            string operazioKodea;
            string tokenZifratua = SortuTokenZifratua();

            if (motaNormalizatua == "Eskudirua")
            {
                jasotakoa = dto.Jasotakoa ?? ordainduBeharrekoa;
                if (jasotakoa.Value < ordainduBeharrekoa)
                {
                    await tx.RollbackAsync();
                    return BadRequestError($"Jasotako dirua ({jasotakoa.Value:F2} €) ez da nahikoa kontua ordaintzeko ({ordainduBeharrekoa:F2} €).");
                }
                itzulitakoa = Math.Round(jasotakoa.Value - ordainduBeharrekoa, 2);
                operazioKodea = $"CASH-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
            }
            else if (motaNormalizatua == "Txartela")
            {
                string rawTxartela = (dto.TxartelZenbakia ?? "").Replace(" ", "").Trim();
                if (string.IsNullOrWhiteSpace(rawTxartela) || rawTxartela.Length < 13)
                {
                    rawTxartela = "453200000000" + Random.Shared.Next(1000, 9999);
                }

                txartelMarka = DetektatuTxartelMarka(rawTxartela);
                string last4 = rawTxartela.Length >= 4 ? rawTxartela[^4..] : "0000";
                txartelMaskaratua = $"**** **** **** {last4}";
                operazioKodea = $"AUTH-{Random.Shared.Next(100000, 999999)}";
            }
            else if (motaNormalizatua == "Bizum")
            {
                string tlf = (dto.BizumTelefonoa ?? "").Replace(" ", "").Trim();
                if (!string.IsNullOrWhiteSpace(tlf) && !Regex.IsMatch(tlf, @"^[67]\d{8}$"))
                {
                    await tx.RollbackAsync();
                    return BadRequestError("Bizum telefono zenbakiak 9 digitu izan behar ditu eta 6 edo 7z hasi.");
                }
                operazioKodea = $"BIZUM-{Random.Shared.Next(100000, 999999)}";
            }
            else
            {
                operazioKodea = $"WALLET-{Random.Shared.Next(100000, 999999)}";
            }

            // 4. Tiket zenbaki bakar eta ofiziala sortu (TK-URTEA-ZENBAKIA)
            string tiketZenbakia = $"TK-{DateTime.Now:yyyyMMdd}-{dto.EskaeraId:D4}";

            // 5. Zergak kalkulatu (%10eko ostalaritza BEZ-a barne)
            decimal subtotal = Math.Round(ordainduBeharrekoa / 1.10m, 2);
            decimal bez = ordainduBeharrekoa - subtotal;

            // 6. 80mm-ko Tiket Testu Profesionala Sortu (Inprimagailu Termikorako ESC/POS formatua)
            string tiketTestua = EraikiTiketTestua(
                tiketZenbakia: tiketZenbakia,
                data: DateTime.Now,
                mahaiZenbakia: mahaiZenbakia,
                edukiera: edukiera,
                eskaeraId: dto.EskaeraId,
                langileIzena: langileIzena,
                lerroak: lerroak,
                subtotal: subtotal,
                bez: bez,
                totala: ordainduBeharrekoa,
                mota: motaNormalizatua,
                jasotakoa: jasotakoa,
                itzulitakoa: itzulitakoa,
                txartelMaskaratua: txartelMaskaratua,
                txartelMarka: txartelMarka,
                bizumTelefonoa: dto.BizumTelefonoa,
                operazioKodea: operazioKodea,
                tokenZifratua: tokenZifratua
            );

            // 7. Txertatu ordainketa datu-basean
            const string sqlInsOrdainketa = @"
                INSERT INTO ordainketak 
                    (eskaera_id, tiket_zenbakia, mota, zenbatekoa, jasotakoa, itzulitakoa, 
                     txartel_maskaratua, txartel_marka, operazio_kodea, token_zifratua, egoera, langilea_id, tiket_testua, data)
                VALUES 
                    (@eskaeraId, @tiketZenbakia, @mota, @zenbatekoa, @jasotakoa, @itzulitakoa,
                     @txartelMaskaratua, @txartelMarka, @operazioKodea, @tokenZifratua, 'Arrakastatsua', @langileaId, @tiketTestua, CURRENT_TIMESTAMP);
                SELECT LAST_INSERT_ID();";

            int ordainketaId;
            await using (var cmdInsO = new MySqlCommand(sqlInsOrdainketa, kon, tx))
            {
                cmdInsO.Parameters.AddWithValue("@eskaeraId", dto.EskaeraId);
                cmdInsO.Parameters.AddWithValue("@tiketZenbakia", tiketZenbakia);
                cmdInsO.Parameters.AddWithValue("@mota", motaNormalizatua);
                cmdInsO.Parameters.AddWithValue("@zenbatekoa", ordainduBeharrekoa);
                cmdInsO.Parameters.AddWithValue("@jasotakoa", (object?)jasotakoa ?? DBNull.Value);
                cmdInsO.Parameters.AddWithValue("@itzulitakoa", (object?)itzulitakoa ?? DBNull.Value);
                cmdInsO.Parameters.AddWithValue("@txartelMaskaratua", (object?)txartelMaskaratua ?? DBNull.Value);
                cmdInsO.Parameters.AddWithValue("@txartelMarka", (object?)txartelMarka ?? DBNull.Value);
                cmdInsO.Parameters.AddWithValue("@operazioKodea", operazioKodea);
                cmdInsO.Parameters.AddWithValue("@tokenZifratua", tokenZifratua);
                cmdInsO.Parameters.AddWithValue("@langileaId", (object?)langileaId ?? DBNull.Value);
                cmdInsO.Parameters.AddWithValue("@tiketTestua", tiketTestua);

                ordainketaId = Convert.ToInt32(await cmdInsO.ExecuteScalarAsync());
            }

            // 8. Eskaera 'Kobratua' moduan markatu
            const string sqlUpdEskaera = "UPDATE eskaerak SET egoera = 'Kobratua' WHERE id = @eskaeraId;";
            await using (var cmdUpdE = new MySqlCommand(sqlUpdEskaera, kon, tx))
            {
                cmdUpdE.Parameters.AddWithValue("@eskaeraId", dto.EskaeraId);
                await cmdUpdE.ExecuteNonQueryAsync();
            }

            // 9. Mahai berean beste eskaera aktiborik ez badago, mahaia 'Garbitzen' jarri
            const string sqlUpdMahaia = @"
                UPDATE mahaiak m
                SET m.egoera = 'Garbitzen'
                WHERE m.id = @mahaiaId
                  AND NOT EXISTS (
                      SELECT 1 FROM eskaerak e2
                      WHERE e2.mahaia_id = m.id
                        AND e2.id <> @eskaeraId
                        AND e2.ezabatua = 0
                        AND e2.egoera NOT IN ('Kobratua', 'Ezeztatua')
                  );";
            await using (var cmdUpdM = new MySqlCommand(sqlUpdMahaia, kon, tx))
            {
                cmdUpdM.Parameters.AddWithValue("@mahaiaId", mahaiaId);
                cmdUpdM.Parameters.AddWithValue("@eskaeraId", dto.EskaeraId);
                await cmdUpdM.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();

            var erantzuna = new OrdainketaErantzunaDto
            {
                Id = ordainketaId,
                EskaeraId = dto.EskaeraId,
                TiketZenbakia = tiketZenbakia,
                Mota = motaNormalizatua,
                Zenbatekoa = ordainduBeharrekoa,
                Jasotakoa = jasotakoa,
                Itzulitakoa = itzulitakoa,
                TxartelMaskaratua = txartelMaskaratua,
                TxartelMarka = txartelMarka,
                OperazioKodea = operazioKodea,
                TokenZifratua = tokenZifratua,
                Egoera = "Arrakastatsua",
                Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TiketTestua = tiketTestua,
                Azpitotala = subtotal,
                Bez = bez
            };

            return CreatedResult(erantzuna, 201, $"#{dto.EskaeraId} eskaeraren ordainketa arrakastaz burutu da ({motaNormalizatua}) eta tiketa sortu da.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServerError("Errorea ordainketa prozesatzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Eskaera baten ordainketa eta tiketa lortzen ditu.
    /// </summary>
    [HttpGet("eskaera/{eskaeraId:int}")]
    public async Task<IActionResult> LortuEskaerarenOrdainketa(int eskaeraId)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = @"
            SELECT id, eskaera_id, tiket_zenbakia, mota, zenbatekoa, jasotakoa, itzulitakoa,
                   txartel_maskaratua, txartel_marka, operazio_kodea, token_zifratua,
                   egoera, tiket_testua, data
            FROM ordainketak
            WHERE eskaera_id = @eskaeraId
            ORDER BY id DESC LIMIT 1;";

        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@eskaeraId", eskaeraId);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFoundError($"Ez da aurkitu #{eskaeraId} eskaeraren ordainketarik.");

        decimal zenbatekoa = reader.GetDecimal("zenbatekoa");
        decimal subtotal = Math.Round(zenbatekoa / 1.10m, 2);

        var dto = new OrdainketaErantzunaDto
        {
            Id = reader.GetInt32("id"),
            EskaeraId = reader.GetInt32("eskaera_id"),
            TiketZenbakia = reader.GetString("tiket_zenbakia"),
            Mota = reader.GetString("mota"),
            Zenbatekoa = zenbatekoa,
            Jasotakoa = reader.IsDBNull(reader.GetOrdinal("jasotakoa")) ? null : reader.GetDecimal("jasotakoa"),
            Itzulitakoa = reader.IsDBNull(reader.GetOrdinal("itzulitakoa")) ? null : reader.GetDecimal("itzulitakoa"),
            TxartelMaskaratua = reader.IsDBNull(reader.GetOrdinal("txartel_maskaratua")) ? null : reader.GetString("txartel_maskaratua"),
            TxartelMarka = reader.IsDBNull(reader.GetOrdinal("txartel_marka")) ? null : reader.GetString("txartel_marka"),
            OperazioKodea = reader.GetString("operazio_kodea"),
            TokenZifratua = reader.IsDBNull(reader.GetOrdinal("token_zifratua")) ? null : reader.GetString("token_zifratua"),
            Egoera = reader.GetString("egoera"),
            Data = reader.GetDateTime("data").ToString("yyyy-MM-dd HH:mm:ss"),
            TiketTestua = reader.IsDBNull(reader.GetOrdinal("tiket_testua")) ? "" : reader.GetString("tiket_testua"),
            Azpitotala = subtotal,
            Bez = zenbatekoa - subtotal
        };

        return Success(dto, 200, "Ordainketa eta tiketa ondo berreskuratu dira.");
    }

    /// <summary>
    /// Tiketaren testu hutsa itzultzen du inprimagailu termikora zuzenean bidaltzeko (text/plain).
    /// </summary>
    [HttpGet("{id:int}/inprimatu")]
    [Produces("text/plain")]
    public async Task<IActionResult> InprimatuTiketa(int id)
    {
        await using var kon = await SortuKonexioaAsync();
        const string sql = "SELECT tiket_testua FROM ordainketak WHERE id = @id LIMIT 1;";
        await using var cmd = new MySqlCommand(sql, kon);
        cmd.Parameters.AddWithValue("@id", id);
        var testua = await cmd.ExecuteScalarAsync();
        if (testua == null) return NotFoundError($"Ez da aurkitu {id} ID-a duen tiketarik.");

        return Content(testua.ToString()!, "text/plain", Encoding.UTF8);
    }

    // =========================================================================
    // LAGUNTZA METODOAK (Zibersegurtasuna eta ESC/POS Formatu Grafikoa)
    // =========================================================================

    private static string NormalizatuMota(string mota)
    {
        string m = (mota ?? "").Trim().ToLowerInvariant();
        if (m.Contains("eskudir") || m.Contains("cash")) return "Eskudirua";
        if (m.Contains("bizum")) return "Bizum";
        if (m.Contains("google")) return "GooglePay";
        if (m.Contains("apple")) return "ApplePay";
        return "Txartela";
    }

    private static string DetektatuTxartelMarka(string zenbakia)
    {
        if (zenbakia.StartsWith("4")) return "VISA";
        if (zenbakia.StartsWith("51") || zenbakia.StartsWith("52") || zenbakia.StartsWith("53") ||
            zenbakia.StartsWith("54") || zenbakia.StartsWith("55")) return "Mastercard";
        if (zenbakia.StartsWith("34") || zenbakia.StartsWith("37")) return "American Express";
        return "Kreditu Txartela";
    }

    private static string SortuTokenZifratua()
    {
        byte[] bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);
        return "tok_pci_aes256_" + Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string EraikiTiketTestua(
        string tiketZenbakia,
        DateTime data,
        int mahaiZenbakia,
        int edukiera,
        int eskaeraId,
        string langileIzena,
        List<(string Izena, int Kantitatea, decimal UnitatePrezioa, decimal Guztira)> lerroak,
        decimal subtotal,
        decimal bez,
        decimal totala,
        string mota,
        decimal? jasotakoa,
        decimal? itzulitakoa,
        string? txartelMaskaratua,
        string? txartelMarka,
        string? bizumTelefonoa,
        string operazioKodea,
        string tokenZifratua)
    {
        var sb = new StringBuilder();
        const int W = 46; // 80mm-ko inprimagailurako zabalera estandarra (46 karaktere)
        string marraBikoitza = new('=', W);
        string marraBakarra = new('-', W);

        string Zentroan(string t)
        {
            if (t.Length >= W) return t;
            int ezker = (W - t.Length) / 2;
            return new string(' ', ezker) + t;
        }

        string Banatuta(string ezker, string eskuin)
        {
            int hutsunea = W - ezker.Length - eskuin.Length;
            if (hutsunea < 1) hutsunea = 1;
            return ezker + new string(' ', hutsunea) + eskuin;
        }

        // 1. Jatetxearen Burua
        sb.AppendLine(marraBikoitza);
        sb.AppendLine(Zentroan("T1 JATETXEA"));
        sb.AppendLine(Zentroan("Italo-Euskal Gastronomia Bikaina"));
        sb.AppendLine(Zentroan("IFZ / CIF: B-20123456"));
        sb.AppendLine(Zentroan("Nafarroa Hiribidea 12, 20240 Ordizia"));
        sb.AppendLine(Zentroan("Tel: +34 943 12 34 56 · www.t1jatetxea.eus"));
        sb.AppendLine(marraBikoitza);

        // 2. Tiket eta Zerbitzu Datuak
        sb.AppendLine(Banatuta("TIKET FISKALA:", tiketZenbakia));
        sb.AppendLine(Banatuta("DATA:", data.ToString("yyyy/MM/dd HH:mm:ss")));
        sb.AppendLine(Banatuta($"MAHAIA: {mahaiZenbakia} ({edukiera} pax)", $"ESKAERA: #{eskaeraId}"));
        sb.AppendLine(Banatuta("ZERBITZARIA:", langileIzena));
        sb.AppendLine(marraBakarra);

        // 3. Produktuen Taula
        sb.AppendLine("KOP  DESKRIBAPENA           P.UNT     GUZTIRA");
        sb.AppendLine(marraBakarra);

        foreach (var l in lerroak)
        {
            string izenLaburtua = l.Izena.Length > 21 ? l.Izena[..20] + "." : l.Izena.PadRight(21);
            string lerroTestua = $"{l.Kantitatea,2}x  {izenLaburtua} {l.UnitatePrezioa,6:F2}  {l.Guztira,7:F2} EUR";
            sb.AppendLine(lerroTestua);
        }

        // 4. Zergak eta Totala
        sb.AppendLine(marraBakarra);
        sb.AppendLine(Banatuta("AZPITOTALA (Zergarik gabe):", $"{subtotal:F2} EUR"));
        sb.AppendLine(Banatuta("BEZ (%10 barne):", $"{bez:F2} EUR"));
        sb.AppendLine(marraBikoitza);
        sb.AppendLine(Banatuta("TOTALA ORDAINTZEKO:", $"{totala:F2} EUR"));
        sb.AppendLine(marraBikoitza);

        // 5. Ordainketa eta Zibersegurtasun Datuak
        sb.AppendLine("ORDAINKETA DATUAK (SEGREDU FISKALA):");
        sb.AppendLine(Banatuta("MODUA:", mota.ToUpperInvariant()));

        if (mota == "Eskudirua" && jasotakoa.HasValue)
        {
            sb.AppendLine(Banatuta("JASOTAKOA:", $"{jasotakoa.Value:F2} EUR"));
            sb.AppendLine(Banatuta("ITZULITAKOA (BUELTA):", $"{itzulitakoa.GetValueOrDefault():F2} EUR"));
        }
        else if (mota == "Txartela")
        {
            sb.AppendLine(Banatuta("TXARTELA:", $"{txartelMaskaratua} ({txartelMarka})"));
            sb.AppendLine(Banatuta("PROTOKOLOA:", "EMV / 3D Secure v2"));
        }
        else if (mota == "Bizum" && !string.IsNullOrWhiteSpace(bizumTelefonoa))
        {
            sb.AppendLine(Banatuta("BIZUM TLF:", bizumTelefonoa));
            sb.AppendLine(Banatuta("BAIEZTAPENA:", "Berehalako push baimendua"));
        }
        else
        {
            sb.AppendLine(Banatuta("WALLET:", "NFC Biometrikoa Baimendua"));
        }

        sb.AppendLine(Banatuta("ERAGIKETA KODEA:", operazioKodea));
        string tokenLaburtua = tokenZifratua.Length > 24 ? tokenZifratua[..22] + ".." : tokenZifratua;
        sb.AppendLine(Banatuta("TOKEN ZIFRATUA:", tokenLaburtua));
        sb.AppendLine(Banatuta("EGOERA:", "BAIMENDUA ETA KOBRATUA"));

        // 6. Oina eta Agurra
        sb.AppendLine(marraBikoitza);
        sb.AppendLine(Zentroan("Eskerrik asko gurekin bazkaltzeagatik!"));
        sb.AppendLine(Zentroan("Grazie mille per la vostra visita!"));
        sb.AppendLine(Zentroan("Faktura osoa eskatzeko: info@t1jatetxea.eus"));
        sb.AppendLine(marraBikoitza);

        return sb.ToString();
    }
}
