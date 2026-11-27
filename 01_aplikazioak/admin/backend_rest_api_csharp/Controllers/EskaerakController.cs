using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/eskaerak")]
public class EskaerakController : BaseApiKontroladorea
{
    public EskaerakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Eskaera guztien zerrenda lortzen du (Fetch bidez Mahaia eta Langilea kargatuz N+1 saihesteko).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak(
        [FromQuery] int? mahaiaId = null,
        [FromQuery] string? egoera = null,
        [FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        IQueryable<Eskaera> query = session.Query<Eskaera>();

        if (!barneEzabatuak) query = query.Where(e => !e.Ezabatua);
        if (mahaiaId.HasValue && mahaiaId.Value > 0) query = query.Where(e => e.Mahaia.Id == mahaiaId.Value);
        if (!string.IsNullOrWhiteSpace(egoera)) query = query.Where(e => e.Egoera == egoera);

        var eskaerak = await query
            .Fetch(e => e.Mahaia)
            .Fetch(e => e.Langilea)
            .OrderByDescending(e => e.SortzeData)
            .ToListAsync();

        var zerrenda = eskaerak.Select(e => new EskaeraDTO
        {
            Id = e.Id,
            MahaiaId = e.Mahaia.Id,
            MahaiZenbakia = e.Mahaia.Zenbakia,
            LangileaId = e.Langilea.Id,
            LangileIzena = e.Langilea.Izena,
            Egoera = e.Egoera,
            Guztira = e.Guztira,
            Ezabatua = e.Ezabatua,
            SortzeData = e.SortzeData.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return SuccessList(zerrenda, 200, "Eskaerak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Eskaera bakarra lortzen du bere lerro guztiekin (komanda osoa).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> LortuIdBidez(int id)
    {
        using var session = IrekiSaioa();

        var eskaera = await session.Query<Eskaera>()
            .Fetch(e => e.Mahaia)
            .Fetch(e => e.Langilea)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eskaera == null)
            return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaerarik.");

        var lerroak = await session.Query<EskaeraLerroa>()
            .Fetch(el => el.Platera)
            .Where(el => el.Eskaera.Id == id)
            .OrderBy(el => el.Id)
            .ToListAsync();

        var dto = new EskaeraDTO
        {
            Id = eskaera.Id,
            MahaiaId = eskaera.Mahaia.Id,
            MahaiZenbakia = eskaera.Mahaia.Zenbakia,
            LangileaId = eskaera.Langilea.Id,
            LangileIzena = eskaera.Langilea.Izena,
            Egoera = eskaera.Egoera,
            Guztira = eskaera.Guztira,
            Ezabatua = eskaera.Ezabatua,
            SortzeData = eskaera.SortzeData.ToString("yyyy-MM-dd HH:mm:ss"),
            Lerroak = lerroak.Select(el => new EskaeraLerroaDTO
            {
                Id = el.Id,
                EskaeraId = eskaera.Id,
                PlateraId = el.Platera.Id,
                PlaterIzena = el.Platera.Izena,
                Kategoria = el.Platera.Kategoria,
                Kantitatea = el.Kantitatea,
                UnitatePrezioa = el.UnitatePrezioa,
                Egoera = el.Egoera,
                Oharrak = el.Oharrak
            }).ToList()
        };

        return Success(dto, 200, "Eskaera ondo berreskuratu da.");
    }

    /// <summary>
    /// Eskaera / Komanda berria sortzen du mahai batean transakzio pesimistan (LockMode.Upgrade).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] EskaeraSortuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequestError("MahaiaId eta LangileaId beharrezkoak dira.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Egiaztatu mahaia aktibo dagoela eta blokeatu (SELECT ... FOR UPDATE)
            var mahaia = await session.GetAsync<Mahaia>(dto.MahaiaId, LockMode.Upgrade);
            if (mahaia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {dto.MahaiaId} ID-a duen mahaia.");
            }
            if (mahaia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Mahaia desgaituta dago.");
            }
            if (mahaia.Egoera == "Garbitzen")
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"{mahaia.Zenbakia}. mahaia garbitzen ari da une honetan. Garbitu ondoren ireki daiteke.");
            }
            if (dto.PertsonaKopurua > 0 && dto.PertsonaKopurua > mahaia.Edukiera)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError($"Mahaiaren aforoa gainditu da: {mahaia.Zenbakia}. mahaiak gehienez {mahaia.Edukiera} lagunentzako lekua du ({dto.PertsonaKopurua} eskatu dira).");
            }

            // 2. Egiaztatu langilea aktibo dagoela
            var langilea = await session.GetAsync<Langilea>(dto.LangileaId);
            if (langilea == null || langilea.Ezabatua || !langilea.Aktibo)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {dto.LangileaId} ID-a duen langile aktiborik.");
            }

            // 3. Mahaiaren egoera 'Okupatuta'-ra pasa
            mahaia.Egoera = "Okupatuta";
            await session.UpdateAsync(mahaia);

            // 4. Eskaera sortu
            var eskaera = new Eskaera
            {
                Mahaia = mahaia,
                Langilea = langilea,
                Egoera = "Irekia",
                Guztira = 0.00m,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };
            await session.SaveAsync(eskaera);

            await tx.CommitAsync();

            var emaitza = new EskaeraDTO
            {
                Id = eskaera.Id,
                MahaiaId = mahaia.Id,
                MahaiZenbakia = mahaia.Zenbakia,
                LangileaId = langilea.Id,
                LangileIzena = langilea.Izena,
                Egoera = "Irekia",
                Guztira = 0.00m,
                Ezabatua = false,
                SortzeData = eskaera.SortzeData.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreatedResult(emaitza, 201, "Eskaera berria arrakastaz ireki da.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea eskaera sortzean: " + ex.Message);
        }
    }

    /// <summary>
    /// BATCH TRANSAKZIO KONPOSATUA: Mahai batean komanda ireki eta hainbat plater aldi berean eskatzen ditu
    /// transakzio pesimista bakar batean. Osagai bakar bat falta bada, komanda osoa eta aurreko plater guztien
    /// stock-deskontua atzera botatzen dira (Rollback atomikoa — dena edo ezer ez).
    /// </summary>
    [HttpPost("komanda-osoa")]
    public async Task<IActionResult> SortuKomandaOsoa([FromBody] KomandaOsoaSortuDto dto)
    {
        if (!ModelState.IsValid || dto.Lerroak == null || dto.Lerroak.Count == 0)
            return BadRequestError("MahaiaId, LangileaId eta gutxienez plater bat beharrezkoak dira.");

        if (dto.Lerroak.Any(l => l.Kantitatea <= 0))
            return BadRequestError("Plater guztien kantitateak 0 baino handiagoa izan behar du.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Mahaia blokeatu eta egiaztatu
            var mahaia = await session.GetAsync<Mahaia>(dto.MahaiaId, LockMode.Upgrade);
            if (mahaia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {dto.MahaiaId} ID-a duen mahaia.");
            }
            if (mahaia.Ezabatua || mahaia.Egoera == "Garbitzen")
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"{mahaia.Zenbakia}. mahaia ez dago erabilgarri ({mahaia.Egoera}).");
            }
            if (dto.PertsonaKopurua > 0 && dto.PertsonaKopurua > mahaia.Edukiera)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError($"Mahaiaren aforoa gainditu da ({mahaia.Edukiera} pax gehienez).");
            }

            // 2. Langilea egiaztatu
            var langilea = await session.GetAsync<Langilea>(dto.LangileaId);
            if (langilea == null || langilea.Ezabatua || !langilea.Aktibo)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {dto.LangileaId} ID-a duen langile aktiborik.");
            }

            // 3. Plater guztiak kargatu eta balidatu + lehengai behar metatuak kalkulatu
            var platerMapa = new Dictionary<int, Platera>();
            var beharMetatuak = new Dictionary<int, decimal>();

            foreach (var lerroDto in dto.Lerroak)
            {
                if (!platerMapa.TryGetValue(lerroDto.PlateraId, out var platera))
                {
                    platera = await session.GetAsync<Platera>(lerroDto.PlateraId);
                    if (platera == null)
                    {
                        await RollbackSeguruaAsync(tx);
                        return NotFoundError($"Ez da aurkitu {lerroDto.PlateraId} ID-a duen platerik.");
                    }
                    if (platera.Ezabatua || !platera.Aktibo)
                    {
                        await RollbackSeguruaAsync(tx);
                        return ConflictError($"'{platera.Izena}' platera desgaituta edo aktibo gabe dago.");
                    }
                    platerMapa[platera.Id] = platera;
                }

                var osagaiak = await session.Query<PlaterOsagaia>()
                    .Where(po => po.PlateraId == lerroDto.PlateraId)
                    .ToListAsync();

                foreach (var po in osagaiak)
                {
                    decimal behar = po.Kantitatea * lerroDto.Kantitatea;
                    beharMetatuak[po.LehengaiaId] = beharMetatuak.GetValueOrDefault(po.LehengaiaId) + behar;
                }
            }

            // 4. Blokeatu beharrezko lehengai GUZTIAK ID gorakorrean (deadlock prebentzioa) eta egiaztatu stock metatua
            var blokeatutakoLehengaiak = new Dictionary<int, Lehengaia>();
            foreach (var lehengaiaId in beharMetatuak.Keys.OrderBy(k => k))
            {
                var lehengaia = await session.GetAsync<Lehengaia>(lehengaiaId, LockMode.Upgrade);
                if (lehengaia == null || lehengaia.Ezabatua)
                    continue;

                decimal beharOsoa = beharMetatuak[lehengaiaId];
                if (lehengaia.UnekoStocka < beharOsoa)
                {
                    await RollbackSeguruaAsync(tx);
                    return ConflictError(
                        $"Batch komanda atzera bota da (Rollback): '{lehengaia.Izena}' lehengaiaren stock metatua ez da nahikoa. " +
                        $"(Behar dira: {beharOsoa} {lehengaia.Unitatea}, Unean: {lehengaia.UnekoStocka} {lehengaia.Unitatea}).");
                }

                blokeatutakoLehengaiak[lehengaiaId] = lehengaia;
            }

            // 5. Mahaia 'Okupatuta' jarri eta Eskaera sortu
            mahaia.Egoera = "Okupatuta";
            await session.UpdateAsync(mahaia);

            bool baduSukaldekoPlaterik = dto.Lerroak.Any(l => !DaEdaria(platerMapa[l.PlateraId].Kategoria));
            string hasierakoEgoera = (dto.BidaliSukaldera && baduSukaldekoPlaterik) ? "Sukaldean" : "Irekia";

            var eskaera = new Eskaera
            {
                Mahaia = mahaia,
                Langilea = langilea,
                Egoera = hasierakoEgoera,
                Guztira = 0.00m,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };
            await session.SaveAsync(eskaera);

            // 6. Stocka deskontatu eta auditoria erregistratu
            foreach (var (lehengaiaId, lehengaia) in blokeatutakoLehengaiak)
            {
                decimal gastua = beharMetatuak[lehengaiaId];
                lehengaia.UnekoStocka -= gastua;
                await session.UpdateAsync(lehengaia);

                await session.SaveAsync(new StockMugimendua
                {
                    Lehengaia = lehengaia,
                    Mota = "Kontsumoa_Platera",
                    Kantitatea = -gastua,
                    Eskaera = eskaera,
                    Oharrak = $"Batch komanda #{eskaera.Id} ({dto.Lerroak.Count} lerro)",
                    Data = DateTime.Now
                });
            }

            // 7. Lerro guztiak txertatu eta Guztira kalkulatu
            var emaitzaLerroak = new List<EskaeraLerroaDTO>();
            decimal guztiraOsoa = 0.00m;

            foreach (var lerroDto in dto.Lerroak)
            {
                var platera = platerMapa[lerroDto.PlateraId];
                string lerroEgoera = (dto.BidaliSukaldera && !DaEdaria(platera.Kategoria)) ? "Sukaldean" : "Eskatua";

                var lerroa = new EskaeraLerroa
                {
                    Eskaera = eskaera,
                    Platera = platera,
                    Kantitatea = lerroDto.Kantitatea,
                    UnitatePrezioa = platera.Prezioa,
                    Egoera = lerroEgoera,
                    Oharrak = lerroDto.Oharrak
                };
                await session.SaveAsync(lerroa);

                guztiraOsoa += platera.Prezioa * lerroDto.Kantitatea;
                emaitzaLerroak.Add(new EskaeraLerroaDTO
                {
                    Id = lerroa.Id,
                    EskaeraId = eskaera.Id,
                    PlateraId = platera.Id,
                    PlaterIzena = platera.Izena,
                    Kategoria = platera.Kategoria,
                    Kantitatea = lerroa.Kantitatea,
                    UnitatePrezioa = lerroa.UnitatePrezioa,
                    Egoera = lerroa.Egoera,
                    Oharrak = lerroa.Oharrak
                });
            }

            eskaera.Guztira = guztiraOsoa;
            await session.UpdateAsync(eskaera);

            await tx.CommitAsync();

            var emaitza = new EskaeraDTO
            {
                Id = eskaera.Id,
                MahaiaId = mahaia.Id,
                MahaiZenbakia = mahaia.Zenbakia,
                LangileaId = langilea.Id,
                LangileIzena = langilea.Izena,
                Egoera = eskaera.Egoera,
                Guztira = eskaera.Guztira,
                Ezabatua = false,
                SortzeData = eskaera.SortzeData.ToString("yyyy-MM-dd HH:mm:ss"),
                Lerroak = emaitzaLerroak
            };

            return CreatedResult(emaitza, 201, $"Komanda osoa ({emaitzaLerroak.Count} lerro) arrakastaz sortu da eta stock metatua deskontatu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da batch komanda sortzean. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea komanda osoa sortzean: " + ex.Message);
        }
    }

    /// <summary>
    /// GAKO TRANSAKZIOA: Platera gehitu komandara eta lehengaien stocka deskontatu pesimistoki
    /// (NHibernate IsolationLevel.ReadCommitted + LockMode.Upgrade ID gorakorrean deadlock-ak saihesteko).
    /// Prezioa backend-eko datu-basetik soilik irakurtzen da segurtasunagatik.
    /// </summary>
    [HttpPost("{id:int}/lerroak")]
    public async Task<IActionResult> GehituPlateraEtaDeskontatuStock(int id, [FromBody] EskaeraLerroaGehituDto dto)
    {
        if (dto.Kantitatea <= 0)
            return BadRequestError("Kantitateak 0 baino handiagoa izan behar du.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Blokeatu eskaera (SELECT ... FROM eskaerak WHERE id = ? FOR UPDATE)
            var eskaera = await session.GetAsync<Eskaera>(id, LockMode.Upgrade);
            if (eskaera == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaerarik.");
            }
            if (eskaera.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Eskaera ezabatuta dago.");
            }
            if (eskaera.Egoera is "Kobratua" or "Ezeztatua")
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"Ezin dira platerak gehitu egoera '{eskaera.Egoera}' duen eskaera batera.");
            }

            // 2. Lortu plateraren prezio ofiziala datu-basetik (Segurtasuna: ez onartu frontendeko preziorik!)
            var platera = await session.GetAsync<Platera>(dto.PlateraId);
            if (platera == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {dto.PlateraId} ID-a duen platerik.");
            }
            if (platera.Ezabatua || !platera.Aktibo)
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError("Platera desgaituta edo aktibo gabe dago.");
            }

            // 3. Lortu errezetako osagaiak ID gorakorrean (deadlock prebentzioa) eta BLOKEATU lehengai bakoitza (LockMode.Upgrade)
            var errezetaOsagaiak = await session.Query<PlaterOsagaia>()
                .Where(po => po.PlateraId == dto.PlateraId)
                .OrderBy(po => po.LehengaiaId)
                .ToListAsync();

            var blokeatutakoOsagaiak = new List<(Lehengaia Lehengaia, decimal Beharrezkoa)>();
            foreach (var po in errezetaOsagaiak)
            {
                var lehengaia = await session.GetAsync<Lehengaia>(po.LehengaiaId, LockMode.Upgrade);
                if (lehengaia == null || lehengaia.Ezabatua)
                    continue;

                blokeatutakoOsagaiak.Add((lehengaia, po.Kantitatea * dto.Kantitatea));
            }

            // 4. Egiaztatu lehengai GUZTIETATIK stock nahikoa dagoen
            foreach (var (lehengaia, beharrezkoa) in blokeatutakoOsagaiak)
            {
                if (lehengaia.UnekoStocka < beharrezkoa)
                {
                    await RollbackSeguruaAsync(tx);
                    return ConflictError(
                        $"Ez dago nahikoa stock '{platera.Izena}' prestatzeko: '{lehengaia.Izena}' lehengaia falta da. " +
                        $"(Behar dira: {beharrezkoa} {lehengaia.Unitatea}, Unean: {lehengaia.UnekoStocka} {lehengaia.Unitatea}).");
                }
            }

            // 5. Deskontatu lehengaien stocka eta erregistratu auditoria (stock_mugimenduak)
            foreach (var (lehengaia, beharrezkoa) in blokeatutakoOsagaiak)
            {
                lehengaia.UnekoStocka -= beharrezkoa;
                await session.UpdateAsync(lehengaia);

                var mugimendua = new StockMugimendua
                {
                    Lehengaia = lehengaia,
                    Mota = "Kontsumoa_Platera",
                    Kantitatea = -beharrezkoa,
                    Eskaera = eskaera,
                    Oharrak = $"Plater kontsumoa: {platera.Izena} x{dto.Kantitatea}",
                    Data = DateTime.Now
                };
                await session.SaveAsync(mugimendua);
            }

            // 6. Gehitu lerroa komandara
            var lerroa = new EskaeraLerroa
            {
                Eskaera = eskaera,
                Platera = platera,
                Kantitatea = dto.Kantitatea,
                UnitatePrezioa = platera.Prezioa,
                Egoera = "Eskatua",
                Oharrak = dto.Oharrak
            };
            await session.SaveAsync(lerroa);

            // 7. Eguneratu eskaeraren guztira zenbatekoa
            eskaera.Guztira += platera.Prezioa * dto.Kantitatea;
            await session.UpdateAsync(eskaera);

            await tx.CommitAsync();

            var emaitzaLerroa = new EskaeraLerroaDTO
            {
                Id = lerroa.Id,
                EskaeraId = eskaera.Id,
                PlateraId = platera.Id,
                PlaterIzena = platera.Izena,
                Kategoria = platera.Kategoria,
                Kantitatea = lerroa.Kantitatea,
                UnitatePrezioa = lerroa.UnitatePrezioa,
                Egoera = lerroa.Egoera,
                Oharrak = lerroa.Oharrak
            };

            return CreatedResult(emaitzaLerroa, 201, $"'{platera.Izena}' platera eskaerara gehitu da eta lehengaien stocka deskontatu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da aldi bereko stock deskontuan. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea platera eskatzean eta stocka deskontatzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Eskaeraren egoera eguneratzen du ('Irekia', 'Sukaldean', 'Prestatua', 'Zerbitzatua', 'Kobratua', 'Ezeztatua').
    /// </summary>
    [HttpPut("{id:int}/egoera")]
    public async Task<IActionResult> EguneratuEgoera(int id, [FromBody] EskaeraEgoeraEguneratuDto dto)
    {
        string egoera = dto.Egoera?.Trim() ?? "";
        if (egoera.Equals("Prestatuta", StringComparison.OrdinalIgnoreCase))
            egoera = "Prestatua";

        var baliozkoak = new[] { "Irekia", "Sukaldean", "Prestatua", "Zerbitzatua", "Kobratua", "Ezeztatua" };
        if (!baliozkoak.Contains(egoera))
            return BadRequestError($"Egoera ez da baliozkoa. Balio onargarriak: {string.Join(", ", baliozkoak)}");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var eskaera = await session.GetAsync<Eskaera>(id, LockMode.Upgrade);
        if (eskaera == null || eskaera.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaera aktiborik.");
        }

        if (egoera is "Sukaldean" or "Prestatua")
        {
            long count = await session.Query<EskaeraLerroa>().LongCountAsync(el => el.Eskaera.Id == id);
            if (count == 0)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError("Ezin da egoera aldatu: eskaerak gutxienez plater bat izan behar du.");
            }
        }

        eskaera.Egoera = egoera;
        await session.UpdateAsync(eskaera);

        if (egoera == "Prestatua")
        {
            var lerroak = await session.Query<EskaeraLerroa>()
                .Fetch(el => el.Platera)
                .Where(el => el.Eskaera.Id == id && (el.Egoera == "Eskatua" || el.Egoera == "Sukaldean"))
                .ToListAsync();

            foreach (var l in lerroak.Where(l => !DaEdaria(l.Platera.Kategoria)))
            {
                l.Egoera = "Prestatua";
                await session.UpdateAsync(l);
            }
        }
        else if (egoera == "Zerbitzatua")
        {
            var lerroak = await session.Query<EskaeraLerroa>()
                .Where(el => el.Eskaera.Id == id && (el.Egoera == "Sukaldean" || el.Egoera == "Prestatua"))
                .ToListAsync();

            foreach (var l in lerroak)
            {
                l.Egoera = "Zerbitzatua";
                await session.UpdateAsync(l);
            }
        }
        else if (egoera == "Kobratua")
        {
            var itxitakoak = new[] { "Kobratua", "Ezeztatua" };
            bool badiraBesteak = await session.Query<Eskaera>()
                .AnyAsync(e2 => e2.Mahaia.Id == eskaera.Mahaia.Id
                             && e2.Id != id
                             && !e2.Ezabatua
                             && !itxitakoak.Contains(e2.Egoera));

            if (!badiraBesteak)
            {
                var mahaia = await session.GetAsync<Mahaia>(eskaera.Mahaia.Id, LockMode.Upgrade);
                if (mahaia != null)
                {
                    mahaia.Egoera = "Garbitzen";
                    await session.UpdateAsync(mahaia);
                }
            }
        }

        await tx.CommitAsync();
        return Success(new { id, egoera = dto.Egoera }, 200, "Eskaeraren egoera eguneratu da.");
    }

    /// <summary>
    /// Eskaerako sukaldeko plater guztiak sukaldera bidaltzen ditu ('Sukaldean' egoerara pasaz, edariak ukitu gabe).
    /// </summary>
    [HttpPost("{id:int}/sukaldera")]
    public async Task<IActionResult> BidaliSukaldera(int id)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var eskaera = await session.GetAsync<Eskaera>(id, LockMode.Upgrade);
        if (eskaera == null || eskaera.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaera aktiborik.");
        }

        var lerroak = await session.Query<EskaeraLerroa>()
            .Fetch(el => el.Platera)
            .Where(el => el.Eskaera.Id == id)
            .ToListAsync();

        if (lerroak.Count == 0)
        {
            await RollbackSeguruaAsync(tx);
            return BadRequestError("Ezin da sukaldera bidali platerik ez duen eskaera huts bat.");
        }

        eskaera.Egoera = "Sukaldean";
        await session.UpdateAsync(eskaera);

        var mahaia = await session.GetAsync<Mahaia>(eskaera.Mahaia.Id, LockMode.Upgrade);
        if (mahaia != null && mahaia.Egoera != "Garbitzen")
        {
            mahaia.Egoera = "Okupatuta";
            await session.UpdateAsync(mahaia);
        }

        int aldatuak = 0;
        foreach (var l in lerroak.Where(l => l.Egoera == "Eskatua" && !DaEdaria(l.Platera.Kategoria)))
        {
            l.Egoera = "Sukaldean";
            await session.UpdateAsync(l);
            aldatuak++;
        }

        await tx.CommitAsync();
        return Success(new { eskaeraId = id, eguneratutakoPlaterak = aldatuak }, 200, $"{aldatuak} plater sukaldera bidali dira.");
    }

    /// <summary>
    /// Sukaldeko (KDS) eta Barrako (BDS) pantailetarako: eskaera-lerro baten egoera eguneratzen du.
    /// </summary>
    [HttpPut("lerroak/{lerroId:int}/egoera")]
    public async Task<IActionResult> EguneratuLerroEgoera(int lerroId, [FromBody] EskaeraLerroEgoeraEguneratuDto dto)
    {
        string egoera = dto.Egoera?.Trim() ?? "";
        if (egoera.Equals("Zain", StringComparison.OrdinalIgnoreCase) || egoera.Equals("Zai", StringComparison.OrdinalIgnoreCase))
            egoera = "Eskatua";
        else if (egoera.Equals("Prestatzen", StringComparison.OrdinalIgnoreCase)
              || egoera.Equals("Martxan", StringComparison.OrdinalIgnoreCase)
              || egoera.Equals("Prestatuta", StringComparison.OrdinalIgnoreCase)
              || egoera.Equals("Eginda", StringComparison.OrdinalIgnoreCase))
            egoera = "Prestatua";

        var baliozkoak = new[] { "Eskatua", "Sukaldean", "Prestatua", "Zerbitzatua" };
        if (!baliozkoak.Contains(egoera))
            return BadRequestError($"Egoera ez da baliozkoa. Balio onargarriak: {string.Join(", ", baliozkoak)}");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var lerroa = await session.GetAsync<EskaeraLerroa>(lerroId, LockMode.Upgrade);
        if (lerroa == null)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {lerroId} ID-a duen eskaera lerrorik.");
        }

        lerroa.Egoera = egoera;
        await session.UpdateAsync(lerroa);
        await session.FlushAsync();

        var eskaera = await session.GetAsync<Eskaera>(lerroa.Eskaera.Id, LockMode.Upgrade);
        if (eskaera != null)
        {
            if (egoera == "Sukaldean")
            {
                if (eskaera.Egoera == "Irekia")
                {
                    eskaera.Egoera = "Sukaldean";
                    await session.UpdateAsync(eskaera);
                }

                var mahaia = await session.GetAsync<Mahaia>(eskaera.Mahaia.Id, LockMode.Upgrade);
                if (mahaia != null && mahaia.Egoera != "Garbitzen")
                {
                    mahaia.Egoera = "Okupatuta";
                    await session.UpdateAsync(mahaia);
                }
            }
            else if (egoera == "Prestatua" && eskaera.Egoera is "Irekia" or "Sukaldean")
            {
                var lerroGuztiak = await session.Query<EskaeraLerroa>()
                    .Fetch(el => el.Platera)
                    .Where(el => el.Eskaera.Id == eskaera.Id)
                    .ToListAsync();

                bool faltaDaSukaldean = lerroGuztiak.Any(el =>
                    !DaEdaria(el.Platera.Kategoria) &&
                    el.Egoera is not ("Prestatua" or "Zerbitzatua"));

                if (!faltaDaSukaldean)
                {
                    eskaera.Egoera = "Prestatua";
                    await session.UpdateAsync(eskaera);
                }
            }
            else if (egoera == "Zerbitzatua" && eskaera.Egoera is "Irekia" or "Sukaldean" or "Prestatua")
            {
                bool badagoZerbitzatuGabea = await session.Query<EskaeraLerroa>()
                    .AnyAsync(el => el.Eskaera.Id == eskaera.Id && el.Egoera != "Zerbitzatua");

                if (!badagoZerbitzatuGabea)
                {
                    eskaera.Egoera = "Zerbitzatua";
                    await session.UpdateAsync(eskaera);
                }
            }
        }

        await tx.CommitAsync();
        return Success(new { lerroId, egoera = dto.Egoera }, 200, "Eskaera lerroaren egoera eguneratu da.");
    }

    /// <summary>
    /// Sukaldera bidali gabeko eskaera-lerroa ezabatzen du eta deskontatutako stocka lehengaietara leheneratzen du.
    /// </summary>
    [HttpDelete("lerroak/{lerroId:int}")]
    public async Task<IActionResult> EzabatuLerroa(int lerroId)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Blokeatu eskaera-lerroa
            var lerroa = await session.GetAsync<EskaeraLerroa>(lerroId, LockMode.Upgrade);
            if (lerroa == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {lerroId} ID-a duen eskaera lerrorik.");
            }

            var platera = await session.GetAsync<Platera>(lerroa.Platera.Id);
            string platerIzena = platera?.Izena ?? $"Platera #{lerroa.Platera.Id}";

            // 2. Egiaztatu sukaldera bidali gabeko platera dela
            if (!lerroa.Egoera.Equals("Eskatua", StringComparison.OrdinalIgnoreCase) &&
                !lerroa.Egoera.Equals("Zai", StringComparison.OrdinalIgnoreCase) &&
                !lerroa.Egoera.Equals("Zain", StringComparison.OrdinalIgnoreCase))
            {
                await RollbackSeguruaAsync(tx);
                return ConflictError($"'{platerIzena}' platera dagoeneko sukaldera bidalita edo prestatuta dago ({lerroa.Egoera}). Sukaldera bidali gabekoak soilik ezabatu daitezke.");
            }

            // Blokeatu eskaera gurasoa
            var eskaera = await session.GetAsync<Eskaera>(lerroa.Eskaera.Id, LockMode.Upgrade);

            // 3. Errezetako lehengaien stocka berreskuratu ID gorakorrean (deadlock prebentzioa)
            var errezetaOsagaiak = await session.Query<PlaterOsagaia>()
                .Where(po => po.PlateraId == lerroa.Platera.Id)
                .OrderBy(po => po.LehengaiaId)
                .ToListAsync();

            foreach (var po in errezetaOsagaiak)
            {
                var lehengaia = await session.GetAsync<Lehengaia>(po.LehengaiaId, LockMode.Upgrade);
                if (lehengaia == null || lehengaia.Ezabatua)
                    continue;

                decimal bueltatu = po.Kantitatea * lerroa.Kantitatea;
                lehengaia.UnekoStocka += bueltatu;
                await session.UpdateAsync(lehengaia);

                await session.SaveAsync(new StockMugimendua
                {
                    Lehengaia = lehengaia,
                    Mota = "Egokitzapena_Inbentarioa",
                    Kantitatea = bueltatu,
                    Eskaera = eskaera,
                    Oharrak = $"Plater ezabatzea: {platerIzena} x{lerroa.Kantitatea}",
                    Data = DateTime.Now
                });
            }

            // 4. Kendu zenbatekoa eskaeraren guztirakotik
            int eskaeraId = lerroa.Eskaera.Id;
            if (eskaera != null)
            {
                decimal kenduGuztira = lerroa.UnitatePrezioa * lerroa.Kantitatea;
                eskaera.Guztira = Math.Max(0.00m, eskaera.Guztira - kenduGuztira);
                await session.UpdateAsync(eskaera);
            }

            // 5. Eskaera lerroa ezabatu
            await session.DeleteAsync(lerroa);
            await tx.CommitAsync();

            return Success(new { lerroId, eskaeraId, mezua = $"'{platerIzena}' platera arrakastaz ezabatu da eta stocka leheneratu da." }, 200, "Platera ezabatu da.");
        }
        catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)
        {
            await RollbackSeguruaAsync(tx);
            return ConflictError("Deadlock gertatu da plater lerroa ezabatzean. Mesedez, saiatu berriro.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea plater lerroa ezabatzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Eskaera ezabatzen du (Soft Delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var eskaera = await session.GetAsync<Eskaera>(id, LockMode.Upgrade);
        if (eskaera == null || eskaera.Ezabatua)
        {
            await RollbackSeguruaAsync(tx);
            return NotFoundError($"Ez da aurkitu {id} ID-a duen eskaera aktiborik.");
        }

        eskaera.Ezabatua = true;
        eskaera.Egoera = "Ezeztatua";
        eskaera.EzabatzeData = DateTime.Now;
        await session.UpdateAsync(eskaera);
        await tx.CommitAsync();

        return Success(new { id, ezabatua = true }, 200, "Eskaera ezabatu da (soft delete).");
    }
}
