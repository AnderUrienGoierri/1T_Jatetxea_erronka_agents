using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/ordainketak")]
public class OrdainketakController : BaseApiKontroladorea
{
    public OrdainketakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Eskaera baten ordainketa segurua prozesatzen du transakzio pesimistan (LockMode.Upgrade)
    /// eta 80mm-ko inprimagailu termikorako tiket fiskal ofiziala sortzen du.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProzesatuOrdainketa([FromBody] OrdainketaEskaeraDto dto)
    {
        if (dto.Zenbatekoa <= 0)
            return BadRequestError("Ordainketa zenbatekoak 0 baino handiagoa izan behar du.");

        string motaNormalizatua = NormalizatuMota(dto.Mota);

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Blokeatu eskaera (SELECT ... FROM eskaerak WHERE id = ? FOR UPDATE) kobrantza bikoitzak saihesteko
            var eskaera = await session.GetAsync<Eskaera>(dto.EskaeraId, LockMode.Upgrade);
            if (eskaera == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu #{dto.EskaeraId} ID-a duen eskaerarik.");
            }
            if (eskaera.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Eskaera hau ezabatuta dago.");
            }
            if (eskaera.Egoera == "Kobratua")
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"#{dto.EskaeraId} eskaera dagoeneko kobratuta eta itxita dago.");
            }
            if (eskaera.Egoera == "Ezeztatua")
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"#{dto.EskaeraId} eskaera ezeztatuta dago, ezin da kobratu.");
            }

            var mahaia = await session.GetAsync<Mahaia>(eskaera.Mahaia.Id, LockMode.Upgrade);
            var langilea = await session.GetAsync<Langilea>(eskaera.Langilea.Id);
            int mahaiZenbakia = mahaia?.Zenbakia ?? 0;
            int edukiera = mahaia?.Edukiera ?? 4;
            int langileaId = dto.LangileaId ?? eskaera.Langilea.Id;
            string langileIzena = langilea?.Izena ?? "Zerbitzaria";

            // 2. Lortu eskaerako plater guztiak tiketa eraikitzeko (Fetch bidez)
            var lerroEntitateak = await session.Query<EskaeraLerroa>()
                .Fetch(el => el.Platera)
                .Where(el => el.Eskaera.Id == dto.EskaeraId)
                .OrderBy(el => el.Id)
                .ToListAsync();

            var lerroak = lerroEntitateak
                .Select(el => (el.Platera.Izena, el.Kantitatea, el.UnitatePrezioa, el.Kantitatea * el.UnitatePrezioa))
                .ToList();

            // 3. Ordainketa motaren balidazio zorrotzak
            decimal ordainduBeharrekoa = eskaera.Guztira > 0 ? eskaera.Guztira : dto.Zenbatekoa;
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
                    await RollbackSeguruaAsync(tx);
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
                    await RollbackSeguruaAsync(tx);
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
            DateTime orain = DateTime.Now;

            // 6. 80mm-ko Tiket Testu Profesionala Sortu
            string tiketTestua = EraikiTiketTestua(
                tiketZenbakia: tiketZenbakia,
                data: orain,
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

            // 7. Gorde ordainketa NHibernate bidez
            var ordainketa = new Ordainketa
            {
                Eskaera = eskaera,
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
                LangileaId = langileaId,
                TiketTestua = tiketTestua,
                Data = orain
            };
            await session.SaveAsync(ordainketa);

            // 8. Eskaera 'Kobratua' moduan markatu
            eskaera.Egoera = "Kobratua";
            await session.UpdateAsync(eskaera);

            // 9. Mahai berean beste eskaera aktiborik ez badago, mahaia 'Garbitzen' jarri
            if (mahaia != null)
            {
                var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
                bool badiraBesteak = await session.Query<Eskaera>()
                    .AnyAsync(e2 => e2.Mahaia.Id == mahaia.Id
                                 && e2.Id != dto.EskaeraId
                                 && !e2.Ezabatua
                                 && !itxitakoak.Contains(e2.Egoera));

                if (!badiraBesteak)
                {
                    mahaia.Egoera = "Garbitzen";
                    await session.UpdateAsync(mahaia);
                }
            }

            await tx.CommitAsync();

            var erantzuna = new OrdainketaErantzunaDto
            {
                Id = ordainketa.Id,
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
                Data = orain.ToString("yyyy-MM-dd HH:mm:ss"),
                TiketTestua = tiketTestua,
                Azpitotala = subtotal,
                Bez = bez
            };

            return CreatedResult(erantzuna, 201, $"#{dto.EskaeraId} eskaeraren ordainketa arrakastaz burutu da ({motaNormalizatua}) eta tiketa sortu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"#{dto.EskaeraId} eskaeraren tiketa dagoeneko sortuta dago (kobrantza bikoitza eragotzita).");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da ordainketa prozesatzean. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea ordainketa prozesatzean: " + ex.Message);
        }
    }

    /// <summary>
    /// TRANSAKZIO KONPOSATUA: Eskaera baten ordainketa hainbat ordainketa-motatan zatitzen du
    /// (adib. Eskudirua + Txartela + Bizum) eta nahi izanez gero salmenta-faktura ofiziala automatikoki
    /// sortzen du transakzio pesimista bakarrean (ordainketak + fakturak + eskaerak + mahaiak).
    /// </summary>
    [HttpPost("zatikatu")]
    public async Task<IActionResult> ProzesatuOrdainketaZatitua([FromBody] OrdainketaZatikatuDto dto)
    {
        if (!ModelState.IsValid || dto.Zatiak == null || dto.Zatiak.Count == 0)
            return BadRequestError("EskaeraId eta gutxienez ordainketa-zati bat beharrezkoak dira.");

        if (dto.Zatiak.Any(z => z.Zenbatekoa <= 0))
            return BadRequestError("Ordainketa-zati guztien zenbatekoak 0 baino handiagoa izan behar du.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Blokeatu eskaera eta mahaia
            var eskaera = await session.GetAsync<Eskaera>(dto.EskaeraId, LockMode.Upgrade);
            if (eskaera == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu #{dto.EskaeraId} ID-a duen eskaerarik.");
            }
            if (eskaera.Ezabatua || eskaera.Egoera is "Kobratua" or "Ezeztatua")
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"#{dto.EskaeraId} eskaera ezin da kobratu (egoera: {eskaera.Egoera}).");
            }

            decimal ordaindutakoaGuztira = Math.Round(dto.Zatiak.Sum(z => z.Zenbatekoa), 2);
            if (eskaera.Guztira > 0 && ordaindutakoaGuztira < eskaera.Guztira)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError(
                    $"Zatien batura ({ordaindutakoaGuztira:F2} €) eskaeraren totala ({eskaera.Guztira:F2} €) baino txikiagoa da.");
            }

            var mahaia = await session.GetAsync<Mahaia>(eskaera.Mahaia.Id, LockMode.Upgrade);
            var langilea = await session.GetAsync<Langilea>(eskaera.Langilea.Id);
            int mahaiZenbakia = mahaia?.Zenbakia ?? 0;
            int edukiera = mahaia?.Edukiera ?? 4;
            int langileaId = dto.LangileaId ?? eskaera.Langilea.Id;
            string langileIzena = langilea?.Izena ?? "Zerbitzaria";

            var lerroEntitateak = await session.Query<EskaeraLerroa>()
                .Fetch(el => el.Platera)
                .Where(el => el.Eskaera.Id == dto.EskaeraId)
                .OrderBy(el => el.Id)
                .ToListAsync();

            var lerroak = lerroEntitateak
                .Select(el => (el.Platera.Izena, el.Kantitatea, el.UnitatePrezioa, el.Kantitatea * el.UnitatePrezioa))
                .ToList();

            DateTime orain = DateTime.Now;
            var sortutakoOrdainketak = new List<OrdainketaErantzunaDto>();

            // 2. Zati bakoitza balidatu eta ordainketak taulan gorde
            for (int i = 0; i < dto.Zatiak.Count; i++)
            {
                var zatia = dto.Zatiak[i];
                string motaNorm = NormalizatuMota(zatia.Mota);
                decimal? jasotakoa = null;
                decimal? itzulitakoa = null;
                string? txartelMask = null;
                string? txartelMarka = null;
                string operazioKodea;
                string tokenZifratua = SortuTokenZifratua();

                if (motaNorm == "Eskudirua")
                {
                    jasotakoa = zatia.Jasotakoa ?? zatia.Zenbatekoa;
                    if (jasotakoa.Value < zatia.Zenbatekoa)
                    {
                        await RollbackSeguruaAsync(tx);
                        return BadRequestError($"{i + 1}. zatiko jasotako dirua ({jasotakoa.Value:F2} €) ez da nahikoa ({zatia.Zenbatekoa:F2} €).");
                    }
                    itzulitakoa = Math.Round(jasotakoa.Value - zatia.Zenbatekoa, 2);
                    operazioKodea = $"CASH-S{i + 1}-{orain:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
                }
                else if (motaNorm == "Txartela")
                {
                    string raw = (zatia.TxartelZenbakia ?? "").Replace(" ", "").Trim();
                    if (string.IsNullOrWhiteSpace(raw) || raw.Length < 13)
                        raw = "453200000000" + Random.Shared.Next(1000, 9999);
                    txartelMarka = DetektatuTxartelMarka(raw);
                    txartelMask = $"**** **** **** {raw[^4..]}";
                    operazioKodea = $"AUTH-S{i + 1}-{Random.Shared.Next(100000, 999999)}";
                }
                else if (motaNorm == "Bizum")
                {
                    string tlf = (zatia.BizumTelefonoa ?? "").Replace(" ", "").Trim();
                    if (!string.IsNullOrWhiteSpace(tlf) && !Regex.IsMatch(tlf, @"^[67]\d{8}$"))
                    {
                        await RollbackSeguruaAsync(tx);
                        return BadRequestError($"{i + 1}. zatiko Bizum telefono zenbakia ez da baliozkoa.");
                    }
                    operazioKodea = $"BIZUM-S{i + 1}-{Random.Shared.Next(100000, 999999)}";
                }
                else
                {
                    operazioKodea = $"WALLET-S{i + 1}-{Random.Shared.Next(100000, 999999)}";
                }

                string tiketZenbakia = $"TK-{orain:yyyyMMdd}-{dto.EskaeraId:D4}-P{i + 1}";
                decimal zatiSubtotal = Math.Round(zatia.Zenbatekoa / 1.10m, 2);
                decimal zatiBez = zatia.Zenbatekoa - zatiSubtotal;

                string tiketTestua = EraikiTiketTestua(
                    tiketZenbakia, orain, mahaiZenbakia, edukiera, dto.EskaeraId, langileIzena,
                    lerroak, zatiSubtotal, zatiBez, zatia.Zenbatekoa, motaNorm, jasotakoa, itzulitakoa,
                    txartelMask, txartelMarka, zatia.BizumTelefonoa, operazioKodea, tokenZifratua);

                var ordainketa = new Ordainketa
                {
                    Eskaera = eskaera,
                    TiketZenbakia = tiketZenbakia,
                    Mota = motaNorm,
                    Zenbatekoa = zatia.Zenbatekoa,
                    Jasotakoa = jasotakoa,
                    Itzulitakoa = itzulitakoa,
                    TxartelMaskaratua = txartelMask,
                    TxartelMarka = txartelMarka,
                    OperazioKodea = operazioKodea,
                    TokenZifratua = tokenZifratua,
                    Egoera = "Arrakastatsua",
                    LangileaId = langileaId,
                    TiketTestua = tiketTestua,
                    Data = orain
                };
                await session.SaveAsync(ordainketa);

                sortutakoOrdainketak.Add(new OrdainketaErantzunaDto
                {
                    Id = ordainketa.Id,
                    EskaeraId = dto.EskaeraId,
                    TiketZenbakia = tiketZenbakia,
                    Mota = motaNorm,
                    Zenbatekoa = zatia.Zenbatekoa,
                    Jasotakoa = jasotakoa,
                    Itzulitakoa = itzulitakoa,
                    TxartelMaskaratua = txartelMask,
                    TxartelMarka = txartelMarka,
                    OperazioKodea = operazioKodea,
                    TokenZifratua = tokenZifratua,
                    Egoera = "Arrakastatsua",
                    Data = orain.ToString("yyyy-MM-dd HH:mm:ss"),
                    TiketTestua = tiketTestua,
                    Azpitotala = zatiSubtotal,
                    Bez = zatiBez
                });
            }

            // 3. Salmenta-faktura ofiziala automatikoki sortu (aukerakoa, lehenetsita true)
            decimal oinarriaOsoa = Math.Round(ordaindutakoaGuztira / 1.10m, 2);
            decimal bezOsoa = ordaindutakoaGuztira - oinarriaOsoa;
            FakturaDTO? fakturaDto = null;

            if (dto.SortuFaktura)
            {
                string fakturaZenbakia = $"F{orain.Year}-S{dto.EskaeraId:D5}-{Random.Shared.Next(10, 99)}";
                string metodoLaburpena = string.Join(" + ", sortutakoOrdainketak.Select(o => o.Mota).Distinct());
                if (metodoLaburpena.Length > 50) metodoLaburpena = metodoLaburpena[..50];

                var faktura = new Faktura
                {
                    Eskaera = eskaera,
                    FakturaZenbakia = fakturaZenbakia,
                    Mota = "Salmenta",
                    Azpimota = string.IsNullOrWhiteSpace(dto.BezeroNif) ? "Sinplifikatua" : "Osoa",
                    Kontzeptua = $"#{dto.EskaeraId} eskaera (Mahaia {mahaiZenbakia}) — Ordainketa zatitua ({dto.Zatiak.Count} zati)",
                    BezeroNif = dto.BezeroNif,
                    BezeroIzena = dto.BezeroIzena,
                    BezeroHelbidea = dto.BezeroHelbidea,
                    OinarriZergagarria = oinarriaOsoa,
                    BezEhunekoa = 10.00m,
                    BezZenbatekoa = bezOsoa,
                    Guztira = ordaindutakoaGuztira,
                    OrdainketaMetodoa = metodoLaburpena,
                    Egoera = "Ordaindua",
                    Data = orain,
                    Ezabatua = false,
                    SortzeData = orain
                };
                await session.SaveAsync(faktura);

                fakturaDto = new FakturaDTO
                {
                    Id = faktura.Id,
                    EskaeraId = eskaera.Id,
                    FakturaZenbakia = faktura.FakturaZenbakia,
                    Mota = faktura.Mota,
                    Azpimota = faktura.Azpimota,
                    Kontzeptua = faktura.Kontzeptua,
                    BezeroNif = faktura.BezeroNif,
                    BezeroIzena = faktura.BezeroIzena,
                    BezeroHelbidea = faktura.BezeroHelbidea,
                    OinarriZergagarria = faktura.OinarriZergagarria,
                    BezEhunekoa = faktura.BezEhunekoa,
                    BezZenbatekoa = faktura.BezZenbatekoa,
                    Guztira = faktura.Guztira,
                    OrdainketaMetodoa = faktura.OrdainketaMetodoa,
                    Egoera = faktura.Egoera,
                    Data = faktura.Data,
                    Ezabatua = false
                };
            }

            // 4. Eskaera 'Kobratua' eta mahaia 'Garbitzen' (beste konturik ez badago)
            eskaera.Egoera = "Kobratua";
            await session.UpdateAsync(eskaera);

            if (mahaia != null)
            {
                var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
                bool badiraBesteak = await session.Query<Eskaera>()
                    .AnyAsync(e2 => e2.Mahaia.Id == mahaia.Id
                                 && e2.Id != dto.EskaeraId
                                 && !e2.Ezabatua
                                 && !itxitakoak.Contains(e2.Egoera));
                if (!badiraBesteak)
                {
                    mahaia.Egoera = "Garbitzen";
                    await session.UpdateAsync(mahaia);
                }
            }

            await tx.CommitAsync();

            var erantzuna = new OrdainketaZatikatuErantzunaDto
            {
                EskaeraId = dto.EskaeraId,
                EskaeraGuztira = eskaera.Guztira,
                OrdaindutakoaGuztira = ordaindutakoaGuztira,
                Azpitotala = oinarriaOsoa,
                Bez = bezOsoa,
                Ordainketak = sortutakoOrdainketak,
                Faktura = fakturaDto
            };

            return CreatedResult(erantzuna, 201, $"#{dto.EskaeraId} eskaera {dto.Zatiak.Count} zatitan kobratu da eta faktura erregistratu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1062)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError($"#{dto.EskaeraId} eskaeraren ordainketa zatitua dagoeneko erregistratuta dago.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da ordainketa zatitua prozesatzean. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea ordainketa zatitua prozesatzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Eskaera baten ordainketa eta tiketa lortzen ditu.
    /// </summary>
    [HttpGet("eskaera/{eskaeraId:int}")]
    public async Task<IActionResult> LortuEskaerarenOrdainketa(int eskaeraId)
    {
        using var session = IrekiSaioa();
        var o = await session.Query<Ordainketa>()
            .Where(x => x.Eskaera.Id == eskaeraId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        if (o == null)
            return NotFoundError($"Ez da aurkitu #{eskaeraId} eskaeraren ordainketarik.");

        decimal subtotal = Math.Round(o.Zenbatekoa / 1.10m, 2);

        var dto = new OrdainketaErantzunaDto
        {
            Id = o.Id,
            EskaeraId = eskaeraId,
            TiketZenbakia = o.TiketZenbakia,
            Mota = o.Mota,
            Zenbatekoa = o.Zenbatekoa,
            Jasotakoa = o.Jasotakoa,
            Itzulitakoa = o.Itzulitakoa,
            TxartelMaskaratua = o.TxartelMaskaratua,
            TxartelMarka = o.TxartelMarka,
            OperazioKodea = o.OperazioKodea,
            TokenZifratua = o.TokenZifratua,
            Egoera = o.Egoera,
            Data = o.Data.ToString("yyyy-MM-dd HH:mm:ss"),
            TiketTestua = o.TiketTestua ?? "",
            Azpitotala = subtotal,
            Bez = o.Zenbatekoa - subtotal
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
        using var session = IrekiSaioa();
        var ordainketa = await session.GetAsync<Ordainketa>(id);
        if (ordainketa == null || ordainketa.TiketTestua == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen tiketarik.");

        return Content(ordainketa.TiketTestua, "text/plain", Encoding.UTF8);
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
        const int W = 46;
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

        sb.AppendLine(marraBikoitza);
        sb.AppendLine(Zentroan("T1 JATETXEA"));
        sb.AppendLine(Zentroan("Italo-Euskal Gastronomia Bikaina"));
        sb.AppendLine(Zentroan("IFZ / CIF: B-20123456"));
        sb.AppendLine(Zentroan("Nafarroa Hiribidea 12, 20240 Ordizia"));
        sb.AppendLine(Zentroan("Tel: +34 943 12 34 56 · www.t1jatetxea.eus"));
        sb.AppendLine(marraBikoitza);

        sb.AppendLine(Banatuta("TIKET FISKALA:", tiketZenbakia));
        sb.AppendLine(Banatuta("DATA:", data.ToString("yyyy/MM/dd HH:mm:ss")));
        sb.AppendLine(Banatuta($"MAHAIA: {mahaiZenbakia} ({edukiera} pax)", $"ESKAERA: #{eskaeraId}"));
        sb.AppendLine(Banatuta("ZERBITZARIA:", langileIzena));
        sb.AppendLine(marraBakarra);

        sb.AppendLine("KOP  DESKRIBAPENA           P.UNT     GUZTIRA");
        sb.AppendLine(marraBakarra);

        foreach (var l in lerroak)
        {
            string izenLaburtua = l.Izena.Length > 21 ? l.Izena[..20] + "." : l.Izena.PadRight(21);
            string lerroTestua = $"{l.Kantitatea,2}x  {izenLaburtua} {l.UnitatePrezioa,6:F2}  {l.Guztira,7:F2} EUR";
            sb.AppendLine(lerroTestua);
        }

        sb.AppendLine(marraBakarra);
        sb.AppendLine(Banatuta("AZPITOTALA (Zergarik gabe):", $"{subtotal:F2} EUR"));
        sb.AppendLine(Banatuta("BEZ (%10 barne):", $"{bez:F2} EUR"));
        sb.AppendLine(marraBikoitza);
        sb.AppendLine(Banatuta("TOTALA ORDAINTZEKO:", $"{totala:F2} EUR"));
        sb.AppendLine(marraBikoitza);

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

        sb.AppendLine(marraBikoitza);
        sb.AppendLine(Zentroan("Eskerrik asko gurekin bazkaltzeagatik!"));
        sb.AppendLine(Zentroan("Grazie mille per la vostra visita!"));
        sb.AppendLine(Zentroan("Faktura osoa eskatzeko: info@t1jatetxea.eus"));
        sb.AppendLine(marraBikoitza);

        return sb.ToString();
    }
}
