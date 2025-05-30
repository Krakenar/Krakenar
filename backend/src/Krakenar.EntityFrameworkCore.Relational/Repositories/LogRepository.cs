using Krakenar.Core.Logging;
using LogEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Log;

namespace Krakenar.EntityFrameworkCore.Relational.Repositories;

public class LogRepository : ILogRepository
{
  protected virtual KrakenarContext Context { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; } = new();

  public LogRepository(KrakenarContext context)
  {
    Context = context;

    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public virtual async Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    LogEntity entity = new(log, SerializerOptions);

    Context.Logs.Add(entity);

    await Context.SaveChangesAsync(cancellationToken);
  }
}
