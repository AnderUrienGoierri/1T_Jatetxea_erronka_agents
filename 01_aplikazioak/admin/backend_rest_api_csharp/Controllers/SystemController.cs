using Jatetxea.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using NHibernate;

namespace Jatetxea.Api.Controllers;

[Route("api/v1/system")]
public sealed class SystemController : BaseApiKontroladorea
{
    public SystemController(ISessionFactory sessionFactory) : base(sessionFactory) { }

    [HttpGet("service-status")]
    public async Task<IActionResult> LortuZerbitzuEgoera()
    {
        using var session = IrekiSaioa();
        ServiceStatusDto status = await LortuZerbitzuEgoeraAsync(session);
        return Success(status, 200, "Zerbitzuaren egoera berreskuratu da.");
    }
}
