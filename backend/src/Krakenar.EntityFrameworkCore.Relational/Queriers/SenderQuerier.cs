using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Senders;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Sender = Krakenar.Core.Senders.Sender;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class SenderQuerier : ISenderQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Sender> Senders { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public SenderQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Senders = context.Senders;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<int> CountAsync(SenderKind kind, CancellationToken cancellationToken)
  {
    int count = await Senders.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.Kind == kind)
      .CountAsync(cancellationToken);

    return count;
  }

  public virtual async Task<SenderId?> FindDefaultIdAsync(SenderKind kind, CancellationToken cancellationToken)
  {
    string? streamId = await Senders.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.Kind == kind && x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new SenderId(streamId);
  }

  public virtual async Task<SenderDto> ReadAsync(Sender sender, CancellationToken cancellationToken)
  {
    return await ReadAsync(sender.Id, cancellationToken) ?? throw new InvalidOperationException($"The sender entity 'StreamId={sender.Id}' could not be found.");
  }
  public virtual async Task<SenderDto?> ReadAsync(SenderId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<SenderDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Sender? sender = await Senders.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return sender is null ? null : await MapAsync(sender, cancellationToken);
  }

  public virtual async Task<SenderDto?> ReadDefaultAsync(SenderKind kind, CancellationToken cancellationToken)
  {
    Entities.Sender? sender = await Senders.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Kind == kind && x.IsDefault, cancellationToken);

    return sender is null ? null : await MapAsync(sender, cancellationToken);
  }

  public virtual async Task<SearchResults<SenderDto>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Senders.Table).SelectAll(KrakenarDb.Senders.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Senders.RealmUid)
      .ApplyIdFilter(KrakenarDb.Senders.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Senders.EmailAddress, KrakenarDb.Senders.PhoneE164Formatted, KrakenarDb.Senders.DisplayName);

    if (payload.Kind.HasValue)
    {
      builder.Where(KrakenarDb.Senders.Kind, Operators.IsEqualTo(payload.Kind.Value.ToString()));
    }
    if (payload.Provider.HasValue)
    {
      builder.Where(KrakenarDb.Senders.Provider, Operators.IsEqualTo(payload.Provider.Value.ToString()));
    }

    IQueryable<Entities.Sender> query = Senders.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Sender>? ordered = null;
    foreach (SenderSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SenderSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case SenderSort.DisplayName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case SenderSort.EmailAddress:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EmailAddress) : ordered.ThenBy(x => x.EmailAddress));
          break;
        case SenderSort.PhoneNumber:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneNumber) : query.OrderBy(x => x.PhoneNumber))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PhoneNumber) : ordered.ThenBy(x => x.PhoneNumber));
          break;
        case SenderSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Sender[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SenderDto> senders = await MapAsync(entities, cancellationToken);

    return new SearchResults<SenderDto>(senders, total);
  }

  protected virtual async Task<SenderDto> MapAsync(Entities.Sender sender, CancellationToken cancellationToken)
  {
    return (await MapAsync([sender], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<SenderDto>> MapAsync(IEnumerable<Entities.Sender> senders, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = senders.SelectMany(sender => sender.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return senders.Select(sender => mapper.ToSender(sender, realm)).ToList().AsReadOnly();
  }
}
