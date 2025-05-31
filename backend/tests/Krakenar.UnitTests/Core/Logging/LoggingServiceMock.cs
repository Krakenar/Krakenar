
namespace Krakenar.Core.Logging;

internal class LoggingServiceMock : LoggingService
{
  public LoggingServiceMock(IApplicationContext applicationContext, IEnumerable<ILogRepository> repositories)
    : base(applicationContext, repositories)
  {
  }

  public new bool ShouldSaveLog() => base.ShouldSaveLog();
}
