using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms;

public class RealmService : IRealmService
{
  protected virtual ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> CreateOrReplaceRealm { get; }
  protected virtual ICommandHandler<DeleteRealm, RealmDto?> DeleteRealm { get; }
  protected virtual IQueryHandler<ReadRealm, RealmDto?> ReadRealm { get; }
  protected virtual IQueryHandler<SearchRealms, SearchResults<RealmDto>> SearchRealms { get; }
  protected virtual ICommandHandler<UpdateRealm, RealmDto?> UpdateRealm { get; }

  public RealmService(
    ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> createOrReplaceRealm,
    ICommandHandler<DeleteRealm, RealmDto?> deleteRealm,
    IQueryHandler<ReadRealm, RealmDto?> readRealm,
    IQueryHandler<SearchRealms, SearchResults<RealmDto>> searchRealms,
    ICommandHandler<UpdateRealm, RealmDto?> updateRealm)
  {
    CreateOrReplaceRealm = createOrReplaceRealm;
    DeleteRealm = deleteRealm;
    ReadRealm = readRealm;
    SearchRealms = searchRealms;
    UpdateRealm = updateRealm;
  }

  public virtual async Task<CreateOrReplaceRealmResult> CreateOrReplaceAsync(CreateOrReplaceRealmPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealm command = new(id, payload, version);
    return await CreateOrReplaceRealm.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<RealmDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRealm command = new(id);
    return await DeleteRealm.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<RealmDto?> ReadAsync(Guid? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    ReadRealm query = new(id, uniqueSlug);
    return await ReadRealm.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<RealmDto>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    SearchRealms query = new(payload);
    return await SearchRealms.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<RealmDto?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    UpdateRealm command = new(id, payload);
    return await UpdateRealm.HandleAsync(command, cancellationToken);
  }
}
