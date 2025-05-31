using Krakenar.Contracts.Dashboard;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Dashboard.Queries;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ActorDto = Krakenar.Contracts.Actors.Actor;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers.Dashboard;

public class GetDashboardStatisticsHandler : IQueryHandler<GetDashboardStatistics, Statistics>
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual KrakenarContext KrakenarContext { get; }

  public GetDashboardStatisticsHandler(IActorService actorService, IApplicationContext applicationContext, KrakenarContext krakenarContext)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    KrakenarContext = krakenarContext;
  }

  public virtual async Task<Statistics> HandleAsync(GetDashboardStatistics _, CancellationToken cancellationToken)
  {
    Statistics statistics = new()
    {
      RealmCount = await KrakenarContext.Realms.AsNoTracking().CountAsync(cancellationToken),
      UserCount = await KrakenarContext.Users.AsNoTracking().Where(x => x.RealmId == null).CountAsync(cancellationToken),
      SessionCount = await KrakenarContext.Sessions.AsNoTracking().Where(x => x.RealmId == null).CountAsync(cancellationToken),
      MessageCount = await KrakenarContext.Messages.AsNoTracking().Where(x => x.RealmId == null).CountAsync(cancellationToken)
    };
    statistics.Sessions.AddRange(await LoadSessionsAsync(realm: null, cancellationToken));

    RealmDto? realm = ApplicationContext.Realm;
    if (realm is not null)
    {
      Guid realmUid = realm.Id;
      RealmStatistics realmStatistics = new()
      {
        UserCount = await KrakenarContext.Users.AsNoTracking().Where(x => x.RealmUid == realmUid).CountAsync(cancellationToken),
        SessionCount = await KrakenarContext.Sessions.AsNoTracking().Where(x => x.RealmUid == realmUid).CountAsync(cancellationToken),
        MessageCount = await KrakenarContext.Messages.AsNoTracking().Where(x => x.RealmUid == realmUid).CountAsync(cancellationToken),
        ContentCount = await KrakenarContext.Contents.AsNoTracking().Where(x => x.RealmUid == realmUid).CountAsync(cancellationToken)
      };
      realmStatistics.Sessions.AddRange(await LoadSessionsAsync(realm, cancellationToken));
      statistics.Realm = realmStatistics;
    }

    return statistics;
  }

  private async Task<IReadOnlyCollection<SessionDto>> LoadSessionsAsync(RealmDto? realm, CancellationToken cancellationToken)
  {
    Guid? realmUid = realm?.Id;

    Session[] sessions = await KrakenarContext.Sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .Where(x => x.RealmUid == realmUid && x.IsActive)
      .OrderByDescending(x => x.UpdatedOn)
      .Take(10)
      .ToArrayAsync(cancellationToken);

    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IReadOnlyDictionary<ActorId, ActorDto> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(session => mapper.ToSession(session, realm)).ToList().AsReadOnly();
  }
}
