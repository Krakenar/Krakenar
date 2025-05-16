using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Contents;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Content = Krakenar.Core.Contents.Content;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ContentQuerier : IContentQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Content> Contents { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public ContentQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Contents = context.Contents;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<ContentDto> ReadAsync(Content content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken) ?? throw new InvalidOperationException($"The content type entity 'StreamId={content.Id}' could not be found.");
  }
  public virtual async Task<ContentDto?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Content? content = await Contents.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.ContentType).ThenInclude(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return content is null ? null : await MapAsync(content, cancellationToken);
  }

  protected virtual async Task<ContentDto> MapAsync(Entities.Content content, CancellationToken cancellationToken)
  {
    return (await MapAsync([content], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<ContentDto>> MapAsync(IEnumerable<Entities.Content> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return contents.Select(content => mapper.ToContent(content, realm)).ToList().AsReadOnly();
  }
}
