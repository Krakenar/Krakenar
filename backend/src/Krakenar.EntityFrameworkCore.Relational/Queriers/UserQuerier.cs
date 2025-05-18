using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Roles;
using Krakenar.Core.Users;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using User = Krakenar.Core.Users.User;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class UserQuerier : IUserQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISqlHelper SqlHelper { get; }
  protected virtual DbSet<Entities.User> Users { get; }

  public UserQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    SqlHelper = sqlHelper;
    Users = context.Users;
  }

  public virtual async Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new UserId(streamId);
  }
  public virtual async Task<UserId?> FindIdAsync(Identifier key, CustomIdentifier value, CancellationToken cancellationToken)
  {
    string? streamId = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Identifiers)
      .Where(x => x.Identifiers.Any(i => i.Key == key.Value.Trim() && i.Value == value.Value.Trim()))
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new UserId(streamId);
  }
  public virtual async Task<IReadOnlyCollection<UserId>> FindIdsAsync(IEmail email, CancellationToken cancellationToken)
  {
    string emailAddressNormalized = Helper.Normalize(email);

    string[] streamIds = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.EmailAddressNormalized == emailAddressNormalized)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new UserId(streamId)).ToList().AsReadOnly();
  }
  public virtual async Task<IReadOnlyCollection<UserId>> FindIdsAsync(RoleId roleId, CancellationToken cancellationToken)
  {
    string[] streamIds = await Users.AsNoTracking()
      .Include(x => x.Roles)
      .Where(x => x.Roles.Any(r => r.StreamId == roleId.Value))
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new UserId(streamId)).ToList().AsReadOnly();
  }

  public virtual async Task<UserDto> ReadAsync(User user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id, cancellationToken) ?? throw new InvalidOperationException($"The user entity 'StreamId={user.Id}' could not be found.");
  }
  public virtual async Task<UserDto?> ReadAsync(UserId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<UserDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.User? user = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return user is null ? null : await MapAsync(user, cancellationToken);
  }
  public virtual async Task<UserDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    Entities.User? user = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return user is null ? null : await MapAsync(user, cancellationToken);
  }
  public virtual async Task<UserDto?> ReadAsync(CustomIdentifierDto identifier, CancellationToken cancellationToken)
  {
    string key = identifier.Key.Trim();
    string value = identifier.Value.Trim();

    Entities.User? user = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.Identifiers.Any(i => i.Key == key && i.Value == value), cancellationToken);

    return user is null ? null : await MapAsync(user, cancellationToken);
  }
  public virtual async Task<IReadOnlyCollection<UserDto>> ReadAsync(IEmail email, CancellationToken cancellationToken)
  {
    string emailAddressNormalized = Helper.Normalize(email);

    Entities.User[] users = await Users.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .Where(x => x.EmailAddressNormalized == emailAddressNormalized)
      .ToArrayAsync(cancellationToken);

    return await MapAsync(users, cancellationToken);
  }

  public virtual async Task<SearchResults<UserDto>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Users.Table).SelectAll(KrakenarDb.Users.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Users.RealmUid)
      .ApplyIdFilter(KrakenarDb.Users.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Users.UniqueName, KrakenarDb.Users.AddressFormatted, KrakenarDb.Users.EmailAddress,
      KrakenarDb.Users.PhoneE164Formatted, KrakenarDb.Users.FirstName, KrakenarDb.Users.MiddleName, KrakenarDb.Users.LastName, KrakenarDb.Users.Nickname,
      KrakenarDb.Users.Gender, KrakenarDb.Users.Locale, KrakenarDb.Users.TimeZone);

    if (payload.HasPassword.HasValue)
    {
      builder.Where(KrakenarDb.Users.HasPassword, Operators.IsEqualTo(payload.HasPassword.Value));
    }
    if (payload.IsDisabled.HasValue)
    {
      builder.Where(KrakenarDb.Users.IsDisabled, Operators.IsEqualTo(payload.IsDisabled.Value));
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder.Where(KrakenarDb.Users.IsConfirmed, Operators.IsEqualTo(payload.IsConfirmed.Value));
    }
    if (payload.HasAuthenticated.HasValue)
    {
      ConditionalOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(KrakenarDb.Users.AuthenticatedOn, @operator);
    }
    if (payload.RoleId.HasValue)
    {
      builder.Join(UserRoles.UserId, KrakenarDb.Users.UserId)
        .Join(Roles.RoleId, UserRoles.RoleId)
        .Where(Roles.Id, Operators.IsEqualTo(payload.RoleId.Value));
    }

    IQueryable<Entities.User> query = Users.FromQuery(builder).AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.User>? ordered = null;
    foreach (UserSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case UserSort.AuthenticatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.AuthenticatedOn) : query.OrderBy(x => x.AuthenticatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.AuthenticatedOn) : ordered.ThenBy(x => x.AuthenticatedOn));
          break;
        case UserSort.Birthdate:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Birthdate) : query.OrderBy(x => x.Birthdate))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Birthdate) : ordered.ThenBy(x => x.Birthdate));
          break;
        case UserSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case UserSort.DisabledOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisabledOn) : query.OrderBy(x => x.DisabledOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisabledOn) : ordered.ThenBy(x => x.DisabledOn));
          break;
        case UserSort.EmailAddress:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EmailAddress) : ordered.ThenBy(x => x.EmailAddress));
          break;
        case UserSort.FirstName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.FirstName) : ordered.ThenBy(x => x.FirstName));
          break;
        case UserSort.FullName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.FullName) : ordered.ThenBy(x => x.FullName));
          break;
        case UserSort.LastName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.LastName) : ordered.ThenBy(x => x.LastName));
          break;
        case UserSort.MiddleName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.MiddleName) : query.OrderBy(x => x.MiddleName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.MiddleName) : ordered.ThenBy(x => x.MiddleName));
          break;
        case UserSort.Nickname:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Nickname) : query.OrderBy(x => x.Nickname))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Nickname) : ordered.ThenBy(x => x.Nickname));
          break;
        case UserSort.PasswordChangedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PasswordChangedOn) : query.OrderBy(x => x.PasswordChangedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PasswordChangedOn) : ordered.ThenBy(x => x.PasswordChangedOn));
          break;
        case UserSort.PhoneNumber:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneNumber) : query.OrderBy(x => x.PhoneNumber))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PhoneNumber) : ordered.ThenBy(x => x.PhoneNumber));
          break;
        case UserSort.UniqueName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case UserSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.User[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<UserDto> users = await MapAsync(entities, cancellationToken);

    return new SearchResults<UserDto>(users, total);
  }

  protected virtual async Task<UserDto> MapAsync(Entities.User user, CancellationToken cancellationToken)
  {
    return (await MapAsync([user], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<UserDto>> MapAsync(IEnumerable<Entities.User> users, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return users.Select(user => mapper.ToUser(user, realm)).ToList().AsReadOnly();
  }
}
