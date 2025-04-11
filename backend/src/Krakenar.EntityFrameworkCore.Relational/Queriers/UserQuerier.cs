using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Users;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Microsoft.EntityFrameworkCore;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using User = Krakenar.Core.Users.User;
using UserDto = Krakenar.Contracts.Users.User;
using UserEntity = Krakenar.EntityFrameworkCore.Relational.Entities.User;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class UserQuerier : IUserQuerier // TODO(fpion): implement
{
  protected IActorService ActorService { get; }
  protected IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<UserEntity> Users { get; }

  public UserQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
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
  public virtual Task<UserId?> FindIdAsync(Identifier key, CustomIdentifier value, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<IReadOnlyCollection<UserId>> FindIdsAsync(IEmail email, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public virtual Task<UserDto> ReadAsync(User user, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(UserId id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(CustomIdentifierDto identifier, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<IReadOnlyCollection<UserDto>> ReadAsync(IEmail email, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
