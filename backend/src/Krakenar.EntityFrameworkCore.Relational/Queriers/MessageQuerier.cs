using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Messages;
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
