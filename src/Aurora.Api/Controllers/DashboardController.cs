using Aurora.Application.Dashboard;
using Aurora.Contracts.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Aurora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get(CancellationToken cancellationToken)
    {
        var response = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(response);
    }
}
