using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Templates;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Template = Krakenar.Core.Templates.Template;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class TemplateQuerier : ITemplateQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISqlHelper SqlHelper { get; }
  protected virtual DbSet<Entities.Template> Templates { get; }

  public TemplateQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    SqlHelper = sqlHelper;
    Templates = context.Templates;
  }

  public virtual async Task<TemplateId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await Templates.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new TemplateId(streamId);
  }

  public virtual async Task<TemplateDto> ReadAsync(Template template, CancellationToken cancellationToken)
  {
    return await ReadAsync(template.Id, cancellationToken) ?? throw new InvalidOperationException($"The template entity 'StreamId={template.Id}' could not be found.");
  }
  public virtual async Task<TemplateDto?> ReadAsync(TemplateId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<TemplateDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Template? template = await Templates.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return template is null ? null : await MapAsync(template, cancellationToken);
  }
  public virtual async Task<TemplateDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    Entities.Template? template = await Templates.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return template is null ? null : await MapAsync(template, cancellationToken);
  }

  public virtual async Task<SearchResults<TemplateDto>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Templates.Table).SelectAll(KrakenarDb.Templates.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Templates.RealmUid)
      .ApplyIdFilter(KrakenarDb.Templates.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Templates.UniqueName, KrakenarDb.Templates.DisplayName);

    if (!string.IsNullOrWhiteSpace(payload.ContentType))
    {
      string contentType = payload.ContentType.Trim().ToLowerInvariant();
      builder.Where(KrakenarDb.Templates.ContentType, Operators.IsEqualTo(contentType));
    }

    IQueryable<Entities.Template> query = Templates.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Template>? ordered = null;
    foreach (TemplateSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case TemplateSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case TemplateSort.DisplayName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case TemplateSort.Subject:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Subject) : ordered.ThenBy(x => x.Subject));
          break;
        case TemplateSort.UniqueName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case TemplateSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Template[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<TemplateDto> templates = await MapAsync(entities, cancellationToken);

    return new SearchResults<TemplateDto>(templates, total);
  }

  protected virtual async Task<TemplateDto> MapAsync(Entities.Template template, CancellationToken cancellationToken)
  {
    return (await MapAsync([template], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<TemplateDto>> MapAsync(IEnumerable<Entities.Template> templates, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = templates.SelectMany(template => template.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return templates.Select(template => mapper.ToTemplate(template, realm)).ToList().AsReadOnly();
  }
}
