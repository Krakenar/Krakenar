using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Passwords;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Templates;
using Krakenar.Contracts.Users;
using Logitar;
using Logitar.EventSourcing;
using SenderProviderNotSupportedException = Krakenar.Core.Senders.SenderProviderNotSupportedException;

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

  public static Actor ToActor(Entities.Actor source) => new(source.DisplayName)
  {
    Type = source.Type,
    Id = source.Id,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public ApiKey ToApiKey(Entities.ApiKey source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    ApiKey destination = new()
    {
      Id = source.Id,
      Realm = realm,
      Name = source.Name,
      Description = source.Description,
      ExpiresOn = source.ExpiresOn?.AsUniversalTime(),
      AuthenticatedOn = source.AuthenticatedOn?.AsUniversalTime()
    };

    destination.CustomAttributes.AddRange(source.GetCustomAttributes().Select(customAttribute => new CustomAttribute(customAttribute)));
    destination.Roles.AddRange(source.Roles.Select(role => ToRole(role, realm)));

    MapAggregate(source, destination);

    return destination;
  }

  public Configuration ToConfiguration(Core.Configurations.Configuration source)
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

  public Dictionary ToDictionary(Entities.Dictionary source, Realm? realm)
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

  public Language ToLanguage(Entities.Language source, Realm? realm)
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

  public Message ToMessage(Entities.Message source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    Message destination = new()
    {
      Id = source.Id,
      Realm = realm,
      Subject = source.Subject,
      Body = new Content(source.BodyType, source.BodyText),
      RecipientCount = source.RecipientCount,
      IgnoreUserLocale = source.IgnoreUserLocale,
      IsDemo = source.IsDemo,
      Status = source.Status
    };

    foreach (Entities.Recipient recipient in source.Recipients)
    {
      destination.Recipients.Add(ToRecipient(recipient, realm));
    }

    if (source.Sender is not null)
    {
      destination.Sender = ToSender(source.Sender, realm);
    }
    else
    {
      destination.Sender = new Sender
      {
        Id = source.SenderUid,
        Realm = realm,
        IsDefault = source.SenderIsDefault,
        DisplayName = source.SenderDisplayName,
        Provider = source.SenderProvider
      };
      if (source.SenderEmailAddress is not null)
      {
        destination.Sender.Kind = SenderKind.Email;
        destination.Sender.Email = new Email(source.SenderEmailAddress);
      }
      else if (source.SenderPhoneNumber is not null && source.SenderPhoneE164Formatted is not null)
      {
        destination.Sender.Kind = SenderKind.Phone;
        destination.Sender.Phone = new Phone(source.SenderPhoneCountryCode, source.SenderPhoneNumber, source.SenderPhoneExtension, source.SenderPhoneE164Formatted);
      }
    }

    if (source.Template is not null)
    {
      destination.Template = ToTemplate(source.Template, realm);
    }
    else
    {
      destination.Template = new Template
      {
        Id = source.TemplateUid,
        UniqueName = source.TemplateUniqueName,
        DisplayName = source.TemplateDisplayName
      };
    }

    if (source.Locale is not null)
    {
      destination.Locale = new Locale(source.Locale);
    }

    foreach (KeyValuePair<string, string> variable in source.GetVariables())
    {
      destination.Variables.Add(new Variable(variable));
    }

    foreach (KeyValuePair<string, string> result in source.GetResults())
    {
      destination.Results.Add(new ResultData(result));
    }

    MapAggregate(source, destination);

    return destination;
  }
  public Recipient ToRecipient(Entities.Recipient source, Realm? realm)
  {
    Recipient destination = new()
    {
      Id = source.Id,
      Type = source.Type,
      DisplayName = source.DisplayName
    };

    if (source.EmailAddress is not null)
    {
      destination.Email = new Email(source.EmailAddress);
    }

    if (source.PhoneNumber is not null && source.PhoneE164Formatted is not null)
    {
      destination.Phone = new Phone(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted);
    }

    if (source.User is not null)
    {
      destination.User = ToUser(source.User, realm);
    }
    else if (source.UserId.HasValue)
    {
      throw new ArgumentException("The user is required.", nameof(source));
    }

    return destination;
  }

  public OneTimePassword ToOneTimePassword(Entities.OneTimePassword source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    OneTimePassword destination = new()
    {
      Id = source.Id,
      Realm = realm,
      ExpiresOn = source.ExpiresOn?.AsUniversalTime(),
      MaximumAttempts = source.MaximumAttempts,
      AttemptCount = source.AttemptCount,
      ValidationSucceededOn = source.ValidationSucceededOn?.AsUniversalTime()
    };

    if (source.User is not null)
    {
      destination.User = ToUser(source.User, realm);
    }
    else if (source.UserId.HasValue)
    {
      throw new ArgumentException("The user is required.", nameof(source));
    }

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Realm ToRealm(Entities.Realm source)
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

  public Role ToRole(Entities.Role source, Realm? realm)
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

  public Sender ToSender(Entities.Sender source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    Sender destination = new()
    {
      Id = source.Id,
      Realm = realm,
      Kind = source.Kind,
      IsDefault = source.IsDefault,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Provider = source.Provider
    };
    if (source.EmailAddress is not null)
    {
      destination.Email = new Email(source.EmailAddress);
    }
    if (source.PhoneNumber is not null && source.PhoneE164Formatted is not null)
    {
      destination.Phone = new Phone(source.PhoneCountryCode, source.PhoneNumber, extension: null, source.PhoneE164Formatted);
    }

    if (source.Settings is not null)
    {
      switch (source.Provider)
      {
        case SenderProvider.SendGrid:
          destination.SendGrid = JsonSerializer.Deserialize<SendGridSettings>(source.Settings);
          break;
        case SenderProvider.Twilio:
          destination.Twilio = JsonSerializer.Deserialize<TwilioSettings>(source.Settings);
          break;
        default:
          throw new SenderProviderNotSupportedException(source.Provider);
      }
    }

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

  public Template ToTemplate(Entities.Template source, Realm? realm)
  {
    if (source.RealmId is not null && realm is null)
    {
      throw new ArgumentNullException(nameof(realm));
    }

    Template destination = new()
    {
      Id = source.Id,
      Realm = realm,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Subject = source.Subject,
      Content = new Content(source.ContentType, source.ContentText)
    };

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
  private void MapAggregate(Entities.Aggregate source, Aggregate destination)
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
