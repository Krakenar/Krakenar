using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;
using Logitar;
using Logitar.EventSourcing;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;
using ConfigurationAggregate = Krakenar.Core.Configurations.Configuration;
using DictionaryEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Dictionary;
using LanguageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Language;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;

namespace Krakenar.EntityFrameworkCore.Relational;

public sealed class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public static Actor ToActor(ActorEntity source) => new(source.DisplayName)
  {
    Type = source.Type,
    Id = source.Id,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public Configuration ToConfiguration(ConfigurationAggregate source)
  {
    Configuration destination = new()
    {
      Secret = source.Secret.Value,
      UniqueNameSettings = new UniqueNameSettings(source.UniqueNameSettings),
      PasswordSettings = new PasswordSettings(source.PasswordSettings),
      LoggingSettings = new LoggingSettings(source.LoggingSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Dictionary ToDictionary(DictionaryEntity source, Realm? realm)
  {
    if (source.Language is null)
    {
      throw new ArgumentException($"The {nameof(source.Language)} is required.", nameof(source));
    }

    Dictionary destination = new()
    {
      Id = source.Id,
      Language = ToLanguage(source.Language, realm),
      EntryCount = source.EntryCount
    };
    destination.Entries.AddRange(source.Entries.Select(entry => new DictionaryEntry(entry.Key, entry.Value)));

    MapAggregate(source, destination);

    return destination;
  }

  public Language ToLanguage(LanguageEntity source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    Language destination = new()
    {
      Id = source.Id,
      Realm = realm,
      IsDefault = source.IsDefault,
      Locale = new Locale(source.Code)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Realm ToRealm(RealmEntity source)
  {
    Realm destination = new()
    {
      Id = source.Id,
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Secret = source.Secret,
      Url = source.Url,
      UniqueNameSettings = source.GetUniqueNameSettings(),
      PasswordSettings = source.GetPasswordSettings(),
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount
    };
    destination.CustomAttributes.AddRange(source.GetCustomAttributes().Select(customAttribute => new CustomAttribute(customAttribute)));

    MapAggregate(source, destination);

    return destination;
  }

  public Role ToRole(RoleEntity source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    Role destination = new()
    {
      Id = source.Id,
      Realm = realm,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description
    };
    destination.CustomAttributes.AddRange(source.GetCustomAttributes().Select(customAttribute => new CustomAttribute(customAttribute)));

    MapAggregate(source, destination);

    return destination;
  }

  public Session ToSession(Entities.Session source, Realm? realm)
  {
    if (source.User is null)
    {
      throw new ArgumentException($"The {nameof(source.User)} is required.", nameof(source));
    }

    Session destination = new()
    {
      Id = source.Id,
      User = ToUser(source.User, realm),
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryFindActor(source.SignedOutBy),
      SignedOutOn = source.SignedOutOn?.AsUniversalTime()
    };
    destination.CustomAttributes.AddRange(source.GetCustomAttributes().Select(customAttribute => new CustomAttribute(customAttribute)));

    MapAggregate(source, destination);

    return destination;
  }

  public User ToUser(Entities.User source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    User destination = new()
    {
      Id = source.Id,
      Realm = realm,
      UniqueName = source.UniqueName,
      HasPassword = source.HasPassword,
      PasswordChangedBy = TryFindActor(source.PasswordChangedBy),
      PasswordChangedOn = source.PasswordChangedOn?.AsUniversalTime(),
      DisabledBy = TryFindActor(source.DisabledBy),
      DisabledOn = source.DisabledOn?.AsUniversalTime(),
      IsDisabled = source.IsDisabled,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = source.Birthdate?.AsUniversalTime(),
      Gender = source.Gender,
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      AuthenticatedOn = source.AuthenticatedOn?.AsUniversalTime()
    };

    if (source.AddressStreet is not null && source.AddressLocality is not null && source.AddressCountry is not null && source.AddressFormatted is not null)
    {
      destination.Address = new Address(source.AddressStreet, source.AddressLocality, source.AddressPostalCode, source.AddressRegion, source.AddressCountry, source.AddressFormatted)
      {
        IsVerified = source.IsAddressVerified,
        VerifiedBy = TryFindActor(source.AddressVerifiedBy),
        VerifiedOn = source.AddressVerifiedOn?.AsUniversalTime()
      };
    }
    if (source.EmailAddress is not null)
    {
      destination.Email = new Email(source.EmailAddress)
      {
        IsVerified = source.IsEmailVerified,
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = source.EmailVerifiedOn?.AsUniversalTime()
      };
    }
    if (source.PhoneNumber is not null && source.PhoneE164Formatted is not null)
    {
      destination.Phone = new Phone(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted)
      {
        IsVerified = source.IsPhoneVerified,
        VerifiedBy = TryFindActor(source.PhoneVerifiedBy),
        VerifiedOn = source.PhoneVerifiedOn?.AsUniversalTime()
      };
    }

    if (source.Locale is not null)
    {
      destination.Locale = new Locale(source.Locale);
    }

    destination.CustomAttributes.AddRange(source.GetCustomAttributes().Select(customAttribute => new CustomAttribute(customAttribute)));
    destination.CustomIdentifiers.AddRange(source.Identifiers.Select(identifier => new CustomIdentifier(identifier.Key, identifier.Value)));
    destination.Roles.AddRange(source.Roles.Select(role => ToRole(role, realm)));

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }
  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor? TryFindActor(string? id) => id is null ? null : TryFindActor(new ActorId(id));
  private Actor? TryFindActor(ActorId? id) => id.HasValue && _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}
