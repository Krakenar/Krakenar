using Krakenar.Contracts;
using Logitar.CQRS;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Queries;

public record ReadRealm(Guid? Id, string? UniqueSlug) : IQuery<RealmDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadRealmHandler : IQueryHandler<ReadRealm, RealmDto?>
{
  protected virtual IRealmQuerier RealmQuerier { get; }

  public ReadRealmHandler(IRealmQuerier realmQuerier)
  {
    RealmQuerier = realmQuerier;
  }

  public virtual async Task<RealmDto?> HandleAsync(ReadRealm query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RealmDto> realms = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RealmDto? realm = await RealmQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (realm is not null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueSlug))
    {
      RealmDto? realm = await RealmQuerier.ReadAsync(query.UniqueSlug, cancellationToken);
      if (realm is not null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw TooManyResultsException<RealmDto>.ExpectedSingle(realms.Count);
    }

    return realms.SingleOrDefault().Value;
  }
}
