using Jatetxea.Api.DTO;
using Jatetxea.Api.Models;
using Jatetxea.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace Jatetxea.Api.Controllers;

[ApiController]
public abstract class BaseApiKontroladorea : ControllerBase
{
    protected readonly ISessionFactory _sessionFactory;

    protected static readonly string[] EdariKategoriak =
    {
        "urak", "freskagarriak", "garagardoak", "sagardoak", "ardoak",
        "kafeak", "infusioak", "likoreak", "konbinatuak", "koktailak"
    };

    protected BaseApiKontroladorea(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    /// <summary>
    /// NHibernate lan-saio (ISession) berria irekitzen du.
    /// Beti 'using' bloke batean erabili behar da eskaera bakoitzean itxi dadin.
    /// </summary>
    protected ISession IrekiSaioa() => _sessionFactory.OpenSession();

    /// <summary>
    /// NHibernate-ren GenericADOException barruan bildutako MySqlException-a ateratzen du
    /// (adib. 1062 Unique, 1451/1452 FK, 1213 Deadlock).
    /// </summary>
    protected static MySqlException? LortuMySqlSalbuespena(Exception ex)
    {
        for (Exception? unekoa = ex; unekoa != null; unekoa = unekoa.InnerException)
        {
            if (unekoa is MySqlException mysqlEx)
                return mysqlEx;
        }
        return null;
    }

    protected static async Task RollbackSeguruaAsync(ITransaction tx)
    {
        if (tx.IsActive)
        {
            await tx.RollbackAsync();
        }
    }

    /// <summary>
    /// Zerbitzua aktibo dagoen egiaztatzen du NHibernate LINQ kontsulta bidez
    /// (okupatutako mahaiak edo kobratu/ezeztatu gabeko eskaerak dituzten mahaiak).
    /// </summary>
    protected async Task<ServiceStatusDto> LortuZerbitzuEgoeraAsync(ISession session)
    {
        var itxitakoEgoerak = new[] { "Kobratua", "Ezeztatua" };

        int openTables = await session.Query<Mahaia>()
            .Where(m => !m.Ezabatua && (
                m.Egoera == "Okupatuta" ||
                session.Query<Eskaera>().Any(e =>
                    e.Mahaia.Id == m.Id &&
                    !e.Ezabatua &&
                    !itxitakoEgoerak.Contains(e.Egoera))))
            .CountAsync();

        return new ServiceStatusDto
        {
            ServiceActive = openTables > 0,
            OpenTables = openTables
        };
    }

    protected async Task<IActionResult?> EgiaztatuZerbitzuaItxitaDagoelaAsync(ISession session)
    {
        ServiceStatusDto status = await LortuZerbitzuEgoeraAsync(session);
        if (status.ServiceActive)
        {
            return ConflictError(
                $"Zerbitzua aktibo dago ({status.OpenTables} mahai). Katalogoaren eta mahaien konfigurazioa blokeatuta dago.");
        }
        return null;
    }

    protected static bool DaEdaria(string? kategoria)
        => !string.IsNullOrWhiteSpace(kategoria) &&
            EdariKategoriak.Contains(kategoria.Trim().ToLowerInvariant());

    // =========================================================================
    // ARRAKASTA ERANTZUNAK (ApiResponse<T>)
    // =========================================================================

    protected IActionResult Success<T>(T data, int code = 200, string message = "OK")
        => StatusCode(code, ApiResponse<T>.Ok(data, code, message));

    protected IActionResult SuccessList<T>(List<T> datalist, int code = 200, string message = "OK")
        => StatusCode(code, ApiResponse<T>.Ok(datalist, code, message));

    protected IActionResult CreatedResult<T>(T data, int code = 201, string message = "Ondo sortu da")
        => StatusCode(code, ApiResponse<T>.Ok(data, code, message));

    // =========================================================================
    // AKATS ERANTZUNAK (ApiError)
    // =========================================================================

    protected IActionResult Error(int code, string type, string message, string? detail = null)
        => StatusCode(code, ApiError.Create(code, type, message, detail));

    protected IActionResult NotFoundError(string detail, string message = "Eskatutakoa ez da aurkitu")
        => Error(404, "NOT_FOUND", message, detail);

    protected IActionResult BadRequestError(string message, string? detail = null)
        => Error(400, "BAD_REQUEST", message, detail);

    protected IActionResult ConflictError(string message, string? detail = null)
        => Error(409, "CONFLICT", message, detail);

    protected IActionResult ServerError(string message, string? detail = null)
        => Error(500, "SERVER_ERROR", message, detail);
}
