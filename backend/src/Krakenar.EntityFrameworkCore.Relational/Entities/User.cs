using Krakenar.Core.Passwords;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public class User : Aggregate, ISegregatedEntity
{
  public int UserId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }

  public string? PasswordHash { get; private set; }
  public string? PasswordChangedBy { get; private set; }
  public DateTime? PasswordChangedOn { get; private set; }
  public bool HasPassword
  {
    get => PasswordChangedOn.HasValue;
    private set { }
  }

  public string? DisabledBy { get; private set; }
  public DateTime? DisabledOn { get; private set; }
  public bool IsDisabled
  {
    get => DisabledOn.HasValue;
    private set { }
  }

  public string? AddressStreet { get; private set; }
  public string? AddressLocality { get; private set; }
  public string? AddressPostalCode { get; private set; }
  public string? AddressRegion { get; private set; }
  public string? AddressCountry { get; private set; }
  public string? AddressFormatted { get; private set; }
  public string? AddressVerifiedBy { get; private set; }
  public DateTime? AddressVerifiedOn { get; private set; }
  public bool IsAddressVerified
  {
    get => AddressVerifiedOn.HasValue;
    private set { }
  }

  public string? EmailAddress { get; private set; }
  public string? EmailAddressNormalized
  {
    get => EmailAddress == null ? null : Helper.Normalize(EmailAddress);
    private set { }
  }
  public string? EmailVerifiedBy { get; private set; }
  public DateTime? EmailVerifiedOn { get; private set; }
  public bool IsEmailVerified
  {
    get => EmailVerifiedOn.HasValue;
    private set { }
  }

  public string? PhoneCountryCode { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; private set; }
  public string? PhoneE164Formatted { get; private set; }
  public string? PhoneVerifiedBy { get; private set; }
  public DateTime? PhoneVerifiedOn { get; private set; }
  public bool IsPhoneVerified
  {
    get => PhoneVerifiedOn.HasValue;
    private set { }
  }

  public bool IsConfirmed
  {
    get => IsAddressVerified || IsEmailVerified || IsPhoneVerified;
    private set { }
  }

  public string? FirstName { get; private set; }
  public string? MiddleName { get; private set; }
  public string? LastName { get; private set; }
  public string? FullName { get; private set; }
  public string? Nickname { get; private set; }

  public DateTime? Birthdate { get; private set; }
  public string? Gender { get; private set; }
  public string? Locale { get; private set; }
  public string? TimeZone { get; private set; }

  public string? Picture { get; private set; }
  public string? Profile { get; private set; }
  public string? Website { get; private set; }

  public DateTime? AuthenticatedOn { get; private set; }

  public string? CustomAttributes { get; private set; }

  public List<UserIdentifier> Identifiers { get; private set; } = [];
  public List<Role> Roles { get; private set; } = [];
  public List<Session> Sessions { get; private set; } = [];

  public User(Realm? realm, UserCreated @event) : base(@event)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

    Id = new UserId(@event.StreamId).EntityId;

    UniqueName = @event.UniqueName.Value;

    if (@event.Password != null)
    {
      SetPassword(@event.Password, @event);
    }
  }

  private User() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipRoles: false, skipSessions: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipRoles, bool skipSessions)
  {
    List<ActorId> actorIds = [];
    actorIds.AddRange(base.GetActorIds());

    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }

    if (PasswordChangedBy != null)
    {
      actorIds.Add(new ActorId(PasswordChangedBy));
    }

    if (DisabledBy != null)
    {
      actorIds.Add(new ActorId(DisabledBy));
    }

    if (AddressVerifiedBy != null)
    {
      actorIds.Add(new ActorId(AddressVerifiedBy));
    }
    if (EmailVerifiedBy != null)
    {
      actorIds.Add(new ActorId(EmailVerifiedBy));
    }
    if (PhoneVerifiedBy != null)
    {
      actorIds.Add(new ActorId(PhoneVerifiedBy));
    }

    if (!skipRoles)
    {
      foreach (Role role in Roles)
      {
        actorIds.AddRange(role.GetActorIds());
      }
    }

    if (!skipSessions)
    {
      foreach (Session session in Sessions)
      {
        actorIds.AddRange(session.GetActorIds(skipUser: true));
      }
    }

    return actorIds.AsReadOnly();
  }

  public void AddRole(Role role, UserRoleAdded @event)
  {
    Update(@event);

    Roles.Add(role);
  }

  public void Authenticate(UserAuthenticated @event)
  {
    Update(@event);

    AuthenticatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Disable(UserDisabled @event)
  {
    Update(@event);

    DisabledBy = @event.ActorId?.Value;
    DisabledOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Enable(UserEnabled @event)
  {
    Update(@event);

    DisabledBy = null;
    DisabledOn = null;
  }

  public void RemoveCustomIdentifier(UserIdentifierRemoved @event)
  {
    Update(@event);

    UserIdentifier? identifier = Identifiers.SingleOrDefault(x => x.Key == @event.Key.Value);
    if (identifier != null)
    {
      Identifiers.Remove(identifier);
    }
  }

  public void RemoveRole(UserRoleRemoved @event)
  {
    Update(@event);

    Role? role = Roles.SingleOrDefault(x => x.StreamId == @event.RoleId.StreamId.Value);
    if (role != null)
    {
      Roles.Remove(role);
    }
  }

  public void SetAddress(UserAddressChanged @event)
  {
    Update(@event);

    AddressStreet = @event.Address?.Street;
    AddressLocality = @event.Address?.Locality;
    AddressPostalCode = @event.Address?.PostalCode;
    AddressRegion = @event.Address?.Region;
    AddressCountry = @event.Address?.Country;
    AddressFormatted = @event.Address?.ToString();

    if (!IsAddressVerified && @event.Address?.IsVerified == true)
    {
      AddressVerifiedBy = @event.ActorId?.Value;
      AddressVerifiedOn = @event.OccurredOn.AsUniversalTime();
    }
    else if (IsAddressVerified && @event.Address?.IsVerified != true)
    {
      AddressVerifiedBy = null;
      AddressVerifiedOn = null;
    }
  }

  public void SetCustomIdentifier(UserIdentifierChanged @event)
  {
    Update(@event);

    UserIdentifier? identifier = Identifiers.SingleOrDefault(x => x.Key == @event.Key.Value);
    if (identifier == null)
    {
      identifier = new(this, @event);
      Identifiers.Add(identifier);
    }
    else
    {
      identifier.Update(@event);
    }
  }

  public void SetEmail(UserEmailChanged @event)
  {
    Update(@event);

    EmailAddress = @event.Email?.Address;

    if (!IsEmailVerified && @event.Email?.IsVerified == true)
    {
      EmailVerifiedBy = @event.ActorId?.Value;
      EmailVerifiedOn = @event.OccurredOn.AsUniversalTime();
    }
    else if (IsEmailVerified && @event.Email?.IsVerified != true)
    {
      EmailVerifiedBy = null;
      EmailVerifiedOn = null;
    }
  }

  public void RemovePassword(UserPasswordRemoved @event)
  {
    Update(@event);

    PasswordHash = null;
    PasswordChangedBy = null;
    PasswordChangedOn = null;
  }

  public void SetPassword(UserPasswordEvent @event)
  {
    Update(@event);

    SetPassword(@event.Password, @event);
  }
  private void SetPassword(Password password, DomainEvent @event)
  {
    PasswordHash = password.Encode();
    PasswordChangedBy = @event.ActorId?.Value;
    PasswordChangedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void SetPhone(UserPhoneChanged @event)
  {
    Update(@event);

    PhoneCountryCode = @event.Phone?.CountryCode;
    PhoneNumber = @event.Phone?.Number;
    PhoneExtension = @event.Phone?.Extension;
    PhoneE164Formatted = @event.Phone?.FormatToE164();

    if (!IsPhoneVerified && @event.Phone?.IsVerified == true)
    {
      PhoneVerifiedBy = @event.ActorId?.Value;
      PhoneVerifiedOn = @event.OccurredOn.AsUniversalTime();
    }
    else if (IsPhoneVerified && @event.Phone?.IsVerified != true)
    {
      PhoneVerifiedBy = null;
      PhoneVerifiedOn = null;
    }
  }

  public void SetUniqueName(UserUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void SignIn(UserSignedIn @event)
  {
    Update(@event);

    AuthenticatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Update(UserUpdated @event)
  {
    base.Update(@event);

    if (@event.FirstName != null)
    {
      FirstName = @event.FirstName.Value?.Value;
    }
    if (@event.MiddleName != null)
    {
      MiddleName = @event.MiddleName.Value?.Value;
    }
    if (@event.LastName != null)
    {
      LastName = @event.LastName.Value?.Value;
    }
    if (@event.FullName != null)
    {
      FullName = @event.FullName.Value;
    }
    if (@event.Nickname != null)
    {
      Nickname = @event.Nickname.Value?.Value;
    }

    if (@event.Birthdate != null)
    {
      Birthdate = @event.Birthdate.Value?.AsUniversalTime();
    }
    if (@event.Gender != null)
    {
      Gender = @event.Gender.Value?.Value;
    }
    if (@event.Locale != null)
    {
      Locale = @event.Locale.Value?.Code;
    }
    if (@event.TimeZone != null)
    {
      TimeZone = @event.TimeZone.Value?.Id;
    }

    if (@event.Picture != null)
    {
      Picture = @event.Picture.Value?.Value;
    }
    if (@event.Profile != null)
    {
      Profile = @event.Profile.Value?.Value;
    }
    if (@event.Website != null)
    {
      Website = @event.Website.Value?.Value;
    }

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Core.Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        customAttributes.Remove(customAttribute.Key.Value);
      }
      else
      {
        customAttributes[customAttribute.Key.Value] = customAttribute.Value;
      }
    }
    SetCustomAttributes(customAttributes);
  }

  public Dictionary<string, string> GetCustomAttributes()
  {
    return (CustomAttributes == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
