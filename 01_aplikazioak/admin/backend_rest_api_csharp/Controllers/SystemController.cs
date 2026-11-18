using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Jatetxea.Api.Controllers;

[Route("api/v1/system")]
public sealed class SystemController : BaseApiKontroladorea
{
    public SystemController(IConfiguration config) : base(config) { }

    [HttpGet("service-status")]
    public async Task<IActionResult> LortuZerbitzuEgoera()
    {
        await using var kon = await SortuKonexioaAsync();
        ServiceStatusDto status = await base.LortuZerbitzuEgoeraAsync(kon);
        return Success(status, 200, "Zerbitzuaren egoera berreskuratu da.");
    }
}
