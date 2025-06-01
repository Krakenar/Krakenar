using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Messages;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Message = Krakenar.Core.Messages.Message;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class MessageQuerier : IMessageQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Message> Messages { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public MessageQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Messages = context.Messages;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<MessageDto> ReadAsync(Message message, CancellationToken cancellationToken)
  {
    return await ReadAsync(message.Id, cancellationToken) ?? throw new InvalidOperationException($"The message entity 'StreamId={message.Id}' could not be found.");
  }
  public virtual async Task<MessageDto?> ReadAsync(MessageId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<MessageDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Message? message = await Messages.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Recipients).ThenInclude(x => x.User)
      .Include(x => x.Sender)
      .Include(x => x.Template)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return message is null ? null : await MapAsync(message, cancellationToken);
  }

  public virtual async Task<SearchResults<MessageDto>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Messages.Table).SelectAll(KrakenarDb.Messages.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Messages.RealmUid)
      .ApplyIdFilter(KrakenarDb.Messages.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Messages.Subject);

    if (payload.TemplateId.HasValue)
    {
      builder.Where(KrakenarDb.Messages.TemplateUid, Operators.IsEqualTo(payload.TemplateId.Value));
    }
    if (payload.IsDemo.HasValue)
    {
      builder.Where(KrakenarDb.Messages.IsDemo, Operators.IsEqualTo(payload.IsDemo.Value));
    }
    if (payload.Status.HasValue)
    {
      builder.Where(KrakenarDb.Messages.Status, Operators.IsEqualTo(payload.Status.Value.ToString()));
    }

    IQueryable<Entities.Message> query = Messages.FromQuery(builder).AsNoTracking()
      .Include(x => x.Sender)
      .Include(x => x.Template);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Message>? ordered = null;
    foreach (MessageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case MessageSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case MessageSort.RecipientCount:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.RecipientCount) : query.OrderBy(x => x.RecipientCount))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.RecipientCount) : ordered.ThenBy(x => x.RecipientCount));
          break;
        case MessageSort.Subject:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Subject) : ordered.ThenBy(x => x.Subject));
          break;
        case MessageSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Message[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<MessageDto> messages = await MapAsync(entities, cancellationToken);

    return new SearchResults<MessageDto>(messages, total);
  }

  protected virtual async Task<MessageDto> MapAsync(Entities.Message message, CancellationToken cancellationToken)
  {
    return (await MapAsync([message], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<MessageDto>> MapAsync(IEnumerable<Entities.Message> messages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = messages.SelectMany(message => message.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return messages.Select(message => mapper.ToMessage(message, realm)).ToList().AsReadOnly();
  }
}
