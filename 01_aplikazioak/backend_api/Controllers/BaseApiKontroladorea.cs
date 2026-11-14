using Jatetxea.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Jatetxea.Api.Controllers;

[ApiController]
public abstract class BaseApiKontroladorea : ControllerBase
{
    protected readonly IConfiguration _config;

    protected BaseApiKontroladorea(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// MySQL datu-basearekin konexio asinkronoa irekitzen du.
    /// Ingurune aldagaia lehenesten da, bestela appsettings konfigurazioa.
    /// </summary>
    protected async Task<MySqlConnection> SortuKonexioaAsync()
    {
        string? envConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        string katea = !string.IsNullOrWhiteSpace(envConn)
            ? envConn
            : (_config.GetConnectionString("Jatetxea") ?? _config.GetConnectionString("JatetxeaRoot") ?? "");

        var kon = new MySqlConnection(katea);
        await kon.OpenAsync();
        return kon;
    }

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
