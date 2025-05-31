using Krakenar.Contracts.Dashboard;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
  protected virtual IDashboardService DashboardService { get; }

  public DashboardController(IDashboardService dashboardService)
  {
    DashboardService = dashboardService;
  }

  [HttpGet]
  public virtual async Task<ActionResult<Statistics>> GetAsync(CancellationToken cancellationToken)
  {
    Statistics statistics = await DashboardService.GetStatisticsAsync(cancellationToken);
    return Ok(statistics);
  }
}
