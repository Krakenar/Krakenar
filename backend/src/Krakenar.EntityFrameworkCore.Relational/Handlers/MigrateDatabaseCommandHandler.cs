using Krakenar.Infrastructure.Commands;
using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class MigrateDatabaseCommandHandler : ICommandHandler<MigrateDatabase, Unit>
{
  protected virtual EventContext EventContext { get; }
  protected virtual KrakenarContext KrakenarContext { get; }

  public MigrateDatabaseCommandHandler(EventContext eventContext, KrakenarContext krakenarContext)
  {
    EventContext = eventContext;
    KrakenarContext = krakenarContext;
  }

  public virtual async Task<Unit> HandleAsync(MigrateDatabase _, CancellationToken cancellationToken)
  {
    await EventContext.Database.MigrateAsync(cancellationToken);
    await KrakenarContext.Database.MigrateAsync(cancellationToken);
    return Unit.Value;
  }
}
