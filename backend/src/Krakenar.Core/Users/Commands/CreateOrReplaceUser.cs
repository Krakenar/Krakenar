using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;
using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Roles;
using Krakenar.Core.Users.Validators;
using Logitar.EventSourcing;
using TimeZone = Krakenar.Core.Localization.TimeZone;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record CreateOrReplaceUserResult(UserDto? User = null, bool Created = false);

public record CreateOrReplaceUser(Guid? Id, CreateOrReplaceUserPayload Payload, long? Version) : ICommand<CreateOrReplaceUserResult>;

/// <exception cref="EmailAddressAlreadyUsedException"></exception>
/// <exception cref="IncorrectUserPasswordException"></exception>
/// <exception cref="RolesNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="UserHasNoPasswordException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceUserHandler : ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult>
{
  protected virtual IAddressHelper AddressHelper { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordService PasswordService { get; }
  protected virtual IRoleService RoleService { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }
  protected virtual IUserService UserService { get; }

  public CreateOrReplaceUserHandler(
    IAddressHelper addressHelper,
    IApplicationContext applicationContext,
    IPasswordService passwordService,
    IRoleService roleService,
    IUserQuerier userQuerier,
    IUserRepository userRepository,
    IUserService userService)
  {
    AddressHelper = addressHelper;
    ApplicationContext = applicationContext;
    PasswordService = passwordService;
    RoleService = roleService;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
    UserService = userService;
  }

  public virtual async Task<CreateOrReplaceUserResult> HandleAsync(CreateOrReplaceUser command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    CreateOrReplaceUserPayload payload = command.Payload;
    new CreateOrReplaceUserValidator(uniqueNameSettings, ApplicationContext.PasswordSettings, AddressHelper).ValidateAndThrow(payload);

    UserId userId = UserId.NewId(ApplicationContext.RealmId);
    User? user = null;
    if (command.Id.HasValue)
    {
      userId = new(command.Id.Value, userId.RealmId);
      user = await UserRepository.LoadAsync(userId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    Password? password = payload.Password is null ? null : PasswordService.ValidateAndHash(payload.Password.New);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (user is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceUserResult();
      }

      user = new(uniqueName, password, actorId, userId);
      created = true;
    }
    else if (payload.Password is not null && password is not null)
    {
      if (payload.Password.Current is null)
      {
        user.SetPassword(password, actorId);
      }
      else
      {
        user.ChangePassword(payload.Password.Current, password, actorId);
      }
    }

    User reference = (command.Version.HasValue
      ? await UserRepository.LoadAsync(userId, command.Version, cancellationToken)
      : null) ?? user;

    if (reference.UniqueName != uniqueName)
    {
      user.SetUniqueName(uniqueName, actorId);
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

    UpdateContactInformation(payload, user, reference, actorId);
    UpdatePersonalInformation(payload, user, reference);
    user.SetCustomAttributes(payload.CustomAttributes, reference);

    await UpdateRolesAsync(payload, user, reference, actorId, cancellationToken);

    user.Update(actorId);
    await UserService.SaveAsync(user, cancellationToken);

    UserDto dto = await UserQuerier.ReadAsync(user, cancellationToken);
    return new CreateOrReplaceUserResult(dto, created);
  }

  protected virtual void UpdateContactInformation(CreateOrReplaceUserPayload payload, User user, User reference, ActorId? actorId)
  {
    Address? address = payload.Address is null ? null : new(AddressHelper, payload.Address, payload.Address.IsVerified);
    if (reference.Address != address)
    {
      user.SetAddress(address, actorId);
    }

    Email? email = payload.Email is null ? null : new(payload.Email, payload.Email.IsVerified);
    if (reference.Email != email)
    {
      user.SetEmail(email, actorId);
    }

    Phone? phone = payload.Phone is null ? null : new(payload.Phone, payload.Phone.IsVerified);
    if (reference.Phone != phone)
    {
      user.SetPhone(phone, actorId);
    }
  }

  protected virtual void UpdatePersonalInformation(CreateOrReplaceUserPayload payload, User user, User reference)
  {
    PersonName? firstName = PersonName.TryCreate(payload.FirstName);
    if (reference.FirstName != firstName)
    {
      user.FirstName = firstName;
    }
    PersonName? middleName = PersonName.TryCreate(payload.MiddleName);
    if (reference.MiddleName != middleName)
    {
      user.MiddleName = middleName;
    }
    PersonName? lastName = PersonName.TryCreate(payload.LastName);
    if (reference.LastName != lastName)
    {
      user.LastName = lastName;
    }
    PersonName? nickname = PersonName.TryCreate(payload.Nickname);
    if (reference.Nickname != nickname)
    {
      user.Nickname = nickname;
    }

    if (reference.Birthdate != payload.Birthdate)
    {
      user.Birthdate = payload.Birthdate;
    }
    Gender? gender = Gender.TryCreate(payload.Gender);
    if (reference.Gender != gender)
    {
      user.Gender = gender;
    }
    Locale? locale = Locale.TryCreate(payload.Locale);
    if (reference.Locale != locale)
    {
      user.Locale = locale;
    }
    TimeZone? timeZone = TimeZone.TryCreate(payload.TimeZone);
    if (reference.TimeZone != timeZone)
    {
      user.TimeZone = timeZone;
    }

    Url? picture = Url.TryCreate(payload.Picture);
    if (reference.Picture != picture)
    {
      user.Picture = picture;
    }
    Url? profile = Url.TryCreate(payload.Profile);
    if (reference.Profile != profile)
    {
      user.Profile = profile;
    }
    Url? website = Url.TryCreate(payload.Website);
    if (reference.Website != website)
    {
      user.Website = website;
    }
  }

  protected virtual async Task UpdateRolesAsync(CreateOrReplaceUserPayload payload, User user, User reference, ActorId? actorId, CancellationToken cancellationToken)
  {
    if (payload.Roles.Count > 0)
    {
      IReadOnlyDictionary<string, Role> roles = await RoleService.FindAsync(payload.Roles, nameof(payload.Roles), cancellationToken);

      HashSet<RoleId> roleIds = new(capacity: roles.Count);
      foreach (Role role in roles.Values)
      {
        roleIds.Add(role.Id);

        if (!reference.HasRole(role))
        {
          user.AddRole(role, actorId);
        }
      }

      foreach (RoleId roleId in reference.Roles)
      {
        if (!roleIds.Contains(roleId))
        {
          user.RemoveRole(roleId, actorId);
        }
      }
    }
  }
}
