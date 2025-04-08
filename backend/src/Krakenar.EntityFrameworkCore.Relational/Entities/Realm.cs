﻿using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public class Realm : Aggregate
{
  public int RealmId { get; private set; }
  public Guid Id { get; private set; }

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => Helper.Normalize(UniqueSlug);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Secret { get; private set; } = string.Empty;
  public string? Url { get; private set; }

  public string? AllowedUniqueNameCharacters { get; private set; }
  public int RequiredPasswordLength { get; private set; }
  public int RequiredPasswordUniqueChars { get; private set; }
  public bool PasswordsRequireNonAlphanumeric { get; private set; }
  public bool PasswordsRequireLowercase { get; private set; }
  public bool PasswordsRequireUppercase { get; private set; }
  public bool PasswordsRequireDigit { get; private set; }
  public string PasswordHashingStrategy { get; private set; } = string.Empty;
  public bool RequireUniqueEmail { get; private set; }
  public bool RequireConfirmedAccount { get; private set; }

  public string? CustomAttributes { get; private set; }

  public List<Actor> Actors { get; private set; } = [];
  public List<Role> Roles { get; private set; } = [];

  public Realm(RealmCreated @event) : base(@event)
  {
    Id = new RealmId(@event.StreamId).ToGuid();

    UniqueSlug = @event.UniqueSlug.Value;

    Secret = @event.Secret.Value;

    SetUniqueNameSettings(@event.UniqueNameSettings);
    SetPasswordSettings(@event.PasswordSettings);
    RequireUniqueEmail = @event.RequireUniqueEmail;
    RequireConfirmedAccount = @event.RequireConfirmedAccount;
  }

  private Realm() : base()
  {
  }

  public void SetUniqueSlug(RealmUniqueSlugChanged @event)
  {
    base.Update(@event);

    UniqueSlug = @event.UniqueSlug.Value;
  }

  public void Update(RealmUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Secret is not null)
    {
      Secret = @event.Secret.Value;
    }
    if (@event.Url is not null)
    {
      Url = @event.Url.Value?.Value;
    }

    if (@event.UniqueNameSettings is not null)
    {
      SetUniqueNameSettings(@event.UniqueNameSettings);
    }
    if (@event.PasswordSettings is not null)
    {
      SetPasswordSettings(@event.PasswordSettings);
    }
    if (@event.RequireUniqueEmail.HasValue)
    {
      RequireUniqueEmail = @event.RequireUniqueEmail.Value;
    }
    if (@event.RequireConfirmedAccount.HasValue)
    {
      RequireConfirmedAccount = @event.RequireConfirmedAccount.Value;
    }

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value is null)
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
    return (CustomAttributes is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }

  private void SetPasswordSettings(IPasswordSettings passwordSettings)
  {
    RequiredPasswordLength = passwordSettings.RequiredLength;
    RequiredPasswordUniqueChars = passwordSettings.RequiredUniqueChars;
    PasswordsRequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
    PasswordsRequireLowercase = passwordSettings.RequireLowercase;
    PasswordsRequireUppercase = passwordSettings.RequireUppercase;
    PasswordsRequireDigit = passwordSettings.RequireDigit;
    PasswordHashingStrategy = passwordSettings.HashingStrategy;
  }
  private void SetUniqueNameSettings(IUniqueNameSettings uniqueNameSettings)
  {
    AllowedUniqueNameCharacters = uniqueNameSettings.AllowedCharacters;
  }

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
