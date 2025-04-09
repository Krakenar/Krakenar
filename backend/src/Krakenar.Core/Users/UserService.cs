using FluentValidation;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;

namespace Krakenar.Core.Users;

public interface IUserService
{
  Task<FoundUsers> FindAsync(string user, CancellationToken cancellationToken = default);

  Task SaveAsync(User user, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public UserService(IApplicationContext applicationContext, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    ApplicationContext = applicationContext;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
  }

  public virtual async Task<FoundUsers> FindAsync(string user, CancellationToken cancellationToken)
  {
    User? byId = await FindByIdAsync(user, cancellationToken);
    User? byUniqueName = await FindByUniqueNameAsync(user, cancellationToken);
    User? byEmailAddress = await FindByEmailAddressAsync(user, cancellationToken);
    User? byCustomIdentifier = await FindByCustomIdentifierAsync(user, cancellationToken);
    return new FoundUsers(byId, byUniqueName, byEmailAddress, byCustomIdentifier);
  }
  protected virtual async Task<User?> FindByCustomIdentifierAsync(string user, CancellationToken cancellationToken)
  {
    int index = user.IndexOf(':');
    if (index >= 0)
    {
      try
      {
        Identifier key = new(user[..index]);
        CustomIdentifier value = new(user[(index + 1)..]);
        UserId? userId = await UserQuerier.FindIdAsync(key, value, cancellationToken);
        return userId.HasValue ? await UserRepository.LoadAsync(userId.Value, cancellationToken) : null;
      }
      catch (ValidationException)
      {
      }
    }
    return null;
  }
  protected virtual async Task<User?> FindByEmailAddressAsync(string user, CancellationToken cancellationToken)
  {
    if (ApplicationContext.RequireUniqueEmail)
    {
      try
      {
        Email email = new(user);
        IReadOnlyCollection<UserId> userIds = await UserQuerier.FindIdsAsync(email, cancellationToken);
        if (userIds.Count == 1)
        {
          return await UserRepository.LoadAsync(userIds.Single(), cancellationToken);
        }
      }
      catch (ValidationException)
      {
      }
    }
    return null;
  }
  protected virtual async Task<User?> FindByIdAsync(string user, CancellationToken cancellationToken)
  {
    if (Guid.TryParse(user, out Guid entityId))
    {
      UserId userId = new(entityId, ApplicationContext.RealmId);
      return await UserRepository.LoadAsync(userId, cancellationToken);
    }
    return null;
  }
  protected virtual async Task<User?> FindByUniqueNameAsync(string user, CancellationToken cancellationToken)
  {
    try
    {
      UniqueNameSettings settings = new(allowedCharacters: null);
      UniqueName uniqueName = new(settings, user);
      UserId? userId = await UserQuerier.FindIdAsync(uniqueName, cancellationToken);
      return userId.HasValue ? await UserRepository.LoadAsync(userId.Value, cancellationToken) : null;
    }
    catch (ValidationException)
    {
      return null;
    }
  }

  public virtual async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = user.Changes.Any(change => change is UserCreated || change is UserUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      UserId? conflictId = await UserQuerier.FindIdAsync(user.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(user.Id))
      {
        throw new UniqueNameAlreadyUsedException(user, conflictId.Value);
      }
    }

    // TODO(fpion): email address unicity
    // TODO(fpion): custom identifier unicity

    await UserRepository.SaveAsync(user, cancellationToken);
  }
}
