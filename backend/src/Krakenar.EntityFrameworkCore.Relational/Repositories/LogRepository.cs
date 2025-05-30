using Krakenar.Core.Logging;

namespace Krakenar.EntityFrameworkCore.Relational.Repositories;

public class LogRepository : ILogRepository
{
  public virtual Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
