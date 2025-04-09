using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Logitar;
using Logitar.EventSourcing;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;
using ConfigurationAggregate = Krakenar.Core.Configurations.Configuration;
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
