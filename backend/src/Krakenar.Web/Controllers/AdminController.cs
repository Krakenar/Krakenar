using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AdminController : Controller
{
  public ActionResult Index() => View();
}
