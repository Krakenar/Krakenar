using Krakenar.Core.Logging;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Krakenar.Web.Filters;

public class Logging : ActionFilterAttribute
{
  protected virtual ILoggingService LoggingService { get; }

  public Logging(ILoggingService loggingService)
  {
    LoggingService = loggingService;
  }

  public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
    {
      Operation operation = new(descriptor.ControllerName, descriptor.ActionName);
      LoggingService.SetOperation(operation);
    }

    return base.OnActionExecutionAsync(context, next);
  }
}
