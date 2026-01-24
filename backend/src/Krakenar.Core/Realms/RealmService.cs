using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Logitar.CQRS;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms;

public class RealmService : IRealmService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public RealmService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceRealmResult> CreateOrReplaceAsync(CreateOrReplaceRealmPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealm command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<RealmDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRealm command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<RealmDto?> ReadAsync(Guid? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    ReadRealm query = new(id, uniqueSlug);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<RealmDto>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    SearchRealms query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<RealmDto?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    UpdateRealm command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
