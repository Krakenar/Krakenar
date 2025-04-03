using Krakenar.Models.Index;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Controllers;

[ApiController]
[Route("api")]
public class IndexController : ControllerBase
{
  [HttpGet]
  public ActionResult<ApiVersion> Get() => Ok(ApiVersion.Current);
}
