using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("app")]
public class AppController : Controller
{
  [HttpGet("{**anything}")]
  public ActionResult Index() => View();
}
