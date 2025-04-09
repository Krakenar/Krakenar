using Krakenar.Core;
using Krakenar.Infrastructure.Commands;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class MigrateDatabaseCommandHandler : ICommandHandler<MigrateDatabase>
{
  protected virtual EventContext EventContext { get; }
  protected virtual KrakenarContext KrakenarContext { get; }

  public MigrateDatabaseCommandHandler(EventContext eventContext, KrakenarContext krakenarContet)
  {
    EventContext = eventContext;
    KrakenarContext = krakenarContet;
  }

  public virtual async Task HandleAsync(MigrateDatabase _, CancellationToken cancellationToken)
  {
    await EventContext.Database.MigrateAsync(cancellationToken);
    await KrakenarContext.Database.MigrateAsync(cancellationToken);
  }
}
