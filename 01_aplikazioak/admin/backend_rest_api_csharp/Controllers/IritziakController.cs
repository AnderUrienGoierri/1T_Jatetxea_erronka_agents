using System.Data;
using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Jatetxea.Api.Controllers;

[ApiController]
[Route("api/v1/iritziak")]
public class IritziakController : BaseApiKontroladorea
{
    public IritziakController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    /// <summary>
    /// Iritzi guztien zerrenda lortzen du bezeroaren informazioarekin (Fetch bidez N+1 saihestuz).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LortuGuztiak([FromQuery] bool barneEzabatuak = false)
    {
        using var session = IrekiSaioa();
        IQueryable<Iritzia> query = session.Query<Iritzia>();
        if (!barneEzabatuak)
            query = query.Where(i => !i.Ezabatua && i.Aktibo);

        var iritziak = await query
            .Fetch(i => i.Bezeroa)
            .OrderByDescending(i => i.SortzeData)
            .ToListAsync();

        var zerrenda = iritziak.Select(i => new IritziaDTO
        {
            Id = i.Id,
            BezeroaId = i.Bezeroa.Id,
            BezeroIzena = i.Bezeroa.Izena,
            BezeroEmaila = i.Bezeroa.Emaila,
            Balorazioa = i.Balorazioa,
            Testua = i.Testua,
            SortzeData = i.SortzeData.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return SuccessList(zerrenda, 200, "Iritziak ondo berreskuratu dira.");
    }

    /// <summary>
    /// Iritzien estatistika orokorrak lortzen ditu (v_balorazio_orokorrak ikuspegitik NHibernate bidez).
    /// </summary>
    [HttpGet("estatistikak")]
    public async Task<IActionResult> LortuEstatistikak()
    {
        using var session = IrekiSaioa();
        var errenkada = await session.CreateSQLQuery(@"
            SELECT guztira_iritziak, bataz_besteko_balorazioa,
                   bost_izarrak, lau_izarrak, hiru_izarrak, bi_izarrak, izar_bat, zero_izar
            FROM v_balorazio_orokorrak
            LIMIT 1;")
            .UniqueResultAsync<object[]>();

        if (errenkada == null)
            return Success(new BalorazioEstatistikakDTO(), 200, "Oraindik ez dago iritzirik.");

        var dto = new BalorazioEstatistikakDTO
        {
            GuztiraIritziak = errenkada[0] == null || errenkada[0] is DBNull ? 0 : Convert.ToInt32(errenkada[0]),
            BatazBestekoBalorazioa = errenkada[1] == null || errenkada[1] is DBNull ? 0 : Convert.ToDecimal(errenkada[1]),
            BostIzarrak = errenkada[2] == null || errenkada[2] is DBNull ? 0 : Convert.ToInt32(errenkada[2]),
            LauIzarrak = errenkada[3] == null || errenkada[3] is DBNull ? 0 : Convert.ToInt32(errenkada[3]),
            HiruIzarrak = errenkada[4] == null || errenkada[4] is DBNull ? 0 : Convert.ToInt32(errenkada[4]),
            BiIzarrak = errenkada[5] == null || errenkada[5] is DBNull ? 0 : Convert.ToInt32(errenkada[5]),
            IzarBat = errenkada[6] == null || errenkada[6] is DBNull ? 0 : Convert.ToInt32(errenkada[6]),
            ZeroIzar = errenkada[7] == null || errenkada[7] is DBNull ? 0 : Convert.ToInt32(errenkada[7])
        };

        return Success(dto, 200, "Iritzien estatistikak ondo kalkulatu dira.");
    }

    /// <summary>
    /// Bezeroaren iritzi berria gordetzen du (0 eta 5 izar artean) transakzio bakarrean.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Sortu([FromBody] IritziaSortuDto dto)
    {
        if (dto.Balorazioa < 0 || dto.Balorazioa > 5)
            return BadRequestError("Balorazioak 0 eta 5 izar artean egon behar du.");

        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // 1. Bezeroa zehaztu edo automatikoki erregistratu
            Bezeroa? bezeroa = null;
            if (dto.BezeroaId.HasValue && dto.BezeroaId.Value > 0)
            {
                bezeroa = await session.GetAsync<Bezeroa>(dto.BezeroaId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(dto.BezeroEmaila))
            {
                string emailNorm = dto.BezeroEmaila.Trim().ToLower();
                bezeroa = await session.Query<Bezeroa>()
                    .FirstOrDefaultAsync(b => b.Emaila == emailNorm);

                if (bezeroa == null)
                {
                    bezeroa = new Bezeroa
                    {
                        Izena = string.IsNullOrWhiteSpace(dto.BezeroIzena) ? "Bezero Anonimoa" : dto.BezeroIzena.Trim(),
                        Abizena = string.IsNullOrWhiteSpace(dto.BezeroAbizena) ? "-" : dto.BezeroAbizena.Trim(),
                        Telefonoa = string.IsNullOrWhiteSpace(dto.BezeroTelefonoa) ? "-" : dto.BezeroTelefonoa.Trim(),
                        Emaila = emailNorm,
                        Ezabatua = false,
                        SortzeData = DateTime.Now
                    };
                    await session.SaveAsync(bezeroa);
                }
            }

            if (bezeroa == null)
            {
                await RollbackSeguruaAsync(tx);
                return BadRequestError("Bezeroa ezinbestekoa da (bezeroaId edo bezeroEmaila bidali behar da).");
            }

            // 2. Iritzia gorde
            var iritzia = new Iritzia
            {
                Bezeroa = bezeroa,
                Balorazioa = dto.Balorazioa,
                Testua = dto.Testua,
                Aktibo = true,
                Ezabatua = false,
                SortzeData = DateTime.Now
            };

            await session.SaveAsync(iritzia);
            await tx.CommitAsync();

            var emaitza = new IritziaDTO
            {
                Id = iritzia.Id,
                BezeroaId = bezeroa.Id,
                BezeroIzena = bezeroa.Izena,
                BezeroEmaila = bezeroa.Emaila,
                Balorazioa = iritzia.Balorazioa,
                Testua = iritzia.Testua,
                SortzeData = iritzia.SortzeData.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreatedResult(emaitza, 201, "Iritzia arrakastaz gorde da.");
        }
        catch (Exception ex)
        {
            await RollbackSeguruaAsync(tx);
            return ServerError("Errorea iritzia gordetzean: " + ex.Message);
        }
    }

    /// <summary>
    /// Iritzia ezabatzen du (Soft Delete lehenetsia, Hard Delete hard=true bidez).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Ezabatu(int id, [FromQuery] bool hard = false)
    {
        using var session = IrekiSaioa();
        using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);

        var iritzia = await session.GetAsync<Iritzia>(id, LockMode.Upgrade);
        if (hard)
        {
            if (iritzia == null)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen iritzirik.");
            }

            await session.DeleteAsync(iritzia);
            await tx.CommitAsync();
            return Success(new { id, ezabatua = true, mota = "hard" }, 200, "Iritzia behin betiko ezabatu da.");
        }
        else
        {
            if (iritzia == null || iritzia.Ezabatua)
            {
                await RollbackSeguruaAsync(tx);
                return NotFoundError($"Ez da aurkitu {id} ID-a duen iritzi aktiborik.");
            }

            iritzia.Ezabatua = true;
            iritzia.Aktibo = false;
            iritzia.EzabatzeData = DateTime.Now;
            await session.UpdateAsync(iritzia);
            await tx.CommitAsync();
            return Success(new { id, ezabatua = true, mota = "soft" }, 200, "Iritzia desgaitu da.");
        }
    }
}
