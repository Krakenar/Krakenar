using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Logitar;
using Logitar.EventSourcing;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;
using ConfigurationAggregate = Krakenar.Core.Configurations.Configuration;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;

namespace Krakenar.EntityFrameworkCore.Relational;

public class Mapper
{
  protected virtual Dictionary<ActorId, Actor> Actors { get; } = [];
  protected virtual Actor System { get; } = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      Actors[actor.Key] = actor.Value;
    }
  }

  public virtual Actor ToActor(ActorEntity source) => new(source.DisplayName)
  {
    Type = source.Type,
    Id = source.Id,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public virtual Configuration ToConfiguration(ConfigurationAggregate source)
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

  public virtual Realm ToRealm(RealmEntity source)
  {
    Realm destination = new()
    {
      Id = source.Id,
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Secret = source.Secret,
      Url = source.Url,
      UniqueNameSettings = ToUniqueNameSettings(source),
      PasswordSettings = ToPasswordSettings(source),
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount
    };
    destination.CustomAttributes.AddRange(source.GetCustomAttributes().Select(customAttribute => new CustomAttribute(customAttribute)));

    MapAggregate(source, destination);

    return destination;
  }
  protected virtual PasswordSettings ToPasswordSettings(RealmEntity source) => new(
    source.RequiredPasswordLength,
    source.RequiredPasswordUniqueChars,
    source.PasswordsRequireNonAlphanumeric,
    source.PasswordsRequireLowercase,
    source.PasswordsRequireUppercase,
    source.PasswordsRequireDigit,
    source.PasswordHashingStrategy);
  protected virtual UniqueNameSettings ToUniqueNameSettings(RealmEntity source) => new(source.AllowedUniqueNameCharacters);

  public virtual Role ToRole(RoleEntity source)
  {
    Realm? realm = null;
    if (source.RealmId.HasValue)
    {
      if (source.Realm is null)
      {
        throw new ArgumentException($"The {nameof(source.Realm)} is required.", nameof(source));
      }
      realm = ToRealm(source.Realm);
    }
    return ToRole(source, realm);
  }
  public virtual Role ToRole(RoleEntity source, Realm? realm)
  {
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

  protected virtual void MapAggregate(AggregateRoot source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? System;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? System;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }
  protected virtual void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? System;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? System;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  protected virtual Actor? TryFindActor(string? id) => id is null ? null : TryFindActor(new ActorId(id));
  protected virtual Actor? TryFindActor(ActorId? id) => id.HasValue && Actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}
