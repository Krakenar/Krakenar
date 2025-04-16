using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;
using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Roles;
using Krakenar.Core.Users.Validators;
using Logitar.EventSourcing;
using Role = Krakenar.Core.Roles.Role;
using TimeZone = Krakenar.Core.Localization.TimeZone;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record UpdateUser(Guid Id, UpdateUserPayload Payload) : ICommand<UserDto?>;

/// <exception cref="EmailAddressAlreadyUsedException"></exception>
/// <exception cref="IncorrectUserPasswordException"></exception>
/// <exception cref="RolesNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="UserHasNoPasswordException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateUserHandler : ICommandHandler<UpdateUser, UserDto?>
{
  protected virtual IAddressHelper AddressHelper { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordService PasswordService { get; }
  protected virtual IRoleManager RoleManager { get; }
  protected virtual IUserManager UserManager { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public UpdateUserHandler(
    IAddressHelper addressHelper,
    IApplicationContext applicationContext,
    IPasswordService passwordService,
    IRoleManager roleManager,
    IUserManager userManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    AddressHelper = addressHelper;
    ApplicationContext = applicationContext;
    PasswordService = passwordService;
    RoleManager = roleManager;
    UserQuerier = userQuerier;
    UserManager = userManager;
    UserRepository = userRepository;
  }

  public virtual async Task<UserDto?> HandleAsync(UpdateUser command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    UpdateUserPayload payload = command.Payload;
    new UpdateUserValidator(uniqueNameSettings, ApplicationContext.PasswordSettings, AddressHelper).ValidateAndThrow(payload);

    UserId userId = new(command.Id, ApplicationContext.RealmId);
    User? user = await UserRepository.LoadAsync(userId, cancellationToken);
    if (user is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      user.SetUniqueName(uniqueName, actorId);
    }
    if (payload.Password is not null)
    {
      Password password = PasswordService.ValidateAndHash(payload.Password.New);
      if (payload.Password.Current is null)
      {
        user.SetPassword(password, actorId);
      }
      else
      {
        user.ChangePassword(payload.Password.Current, password, actorId);
      }
    }
    if (payload.IsDisabled.HasValue)
    {
      if (payload.IsDisabled.Value)
      {
        user.Disable(actorId);
      }
      else
      {
        user.Enable(actorId);
      }
    }

    UpdateContactInformation(payload, user, actorId);
    UpdatePersonalInformation(payload, user);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      user.SetCustomAttribute(key, customAttribute.Value);
    }

    await UpdateRolesAsync(payload, user, actorId, cancellationToken);

    user.Update(actorId);
    await UserManager.SaveAsync(user, cancellationToken);

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }

  protected virtual void UpdateContactInformation(UpdateUserPayload payload, User user, ActorId? actorId)
  {
    if (payload.Address is not null)
    {
      Address? address = payload.Address.Value is null ? null : new(AddressHelper, payload.Address.Value, payload.Address.Value.IsVerified);
      user.SetAddress(address, actorId);
    }

    if (payload.Email is not null)
    {
      Email? email = payload.Email.Value is null ? null : new(payload.Email.Value, payload.Email.Value.IsVerified);
      user.SetEmail(email, actorId);
    }

    if (payload.Phone is not null)
    {
      Phone? phone = payload.Phone.Value is null ? null : new(payload.Phone.Value, payload.Phone.Value.IsVerified);
      user.SetPhone(phone, actorId);
    }
  }

  protected virtual void UpdatePersonalInformation(UpdateUserPayload payload, User user)
  {
    if (payload.FirstName is not null)
    {
      user.FirstName = PersonName.TryCreate(payload.FirstName.Value);
    }
    if (payload.MiddleName is not null)
    {
      user.MiddleName = PersonName.TryCreate(payload.MiddleName.Value);
    }
    if (payload.LastName is not null)
    {
      user.LastName = PersonName.TryCreate(payload.LastName.Value);
    }
    if (payload.Nickname is not null)
    {
      user.Nickname = PersonName.TryCreate(payload.Nickname.Value);
    }

    if (payload.Birthdate is not null)
    {
      user.Birthdate = payload.Birthdate.Value;
    }
    if (payload.Gender is not null)
    {
      user.Gender = Gender.TryCreate(payload.Gender.Value);
    }
    if (payload.Locale is not null)
    {
      user.Locale = Locale.TryCreate(payload.Locale.Value);
    }
    if (payload.TimeZone is not null)
    {
      user.TimeZone = TimeZone.TryCreate(payload.TimeZone.Value);
    }

    if (payload.Picture is not null)
    {
      user.Picture = Url.TryCreate(payload.Picture.Value);
    }
    if (payload.Profile is not null)
    {
      user.Profile = Url.TryCreate(payload.Profile.Value);
    }
    if (payload.Website is not null)
    {
      user.Website = Url.TryCreate(payload.Website.Value);
    }
  }

  protected virtual async Task UpdateRolesAsync(UpdateUserPayload payload, User user, ActorId? actorId, CancellationToken cancellationToken)
  {
    if (payload.Roles.Count > 0)
    {
      IReadOnlyDictionary<string, Role> roles = await RoleManager.FindAsync(payload.Roles.Select(role => role.Role), nameof(payload.Roles), cancellationToken);
      foreach (RoleChange change in payload.Roles)
      {
        Role role = roles[change.Role];
        switch (change.Action)
        {
          case CollectionAction.Add:
            user.AddRole(role, actorId);
            break;
          case CollectionAction.Remove:
            user.RemoveRole(role, actorId);
            break;
        }
      }
    }
  }
}
