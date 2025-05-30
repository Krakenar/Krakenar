using Bogus;
using Krakenar.Core.Realms.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;

namespace Krakenar.Core.Realms;

[Trait(Traits.Category, Categories.Unit)]
public class RealmTests
{
  private readonly Faker _faker = new();
  private readonly Realm _realm = new(new Slug("my-realm"), new Secret(RandomStringGenerator.GetString(Secret.MinimumLength)));

  [Fact(DisplayName = "Delete: it should delete the realm.")]
  public void Given_Realm_When_Delete_Then_Deleted()
  {
    Assert.False(_realm.IsDeleted);

    _realm.Delete();
    Assert.True(_realm.IsDeleted);
    Assert.Contains(_realm.Changes, change => change is RealmDeleted);

    _realm.ClearChanges();
    _realm.Delete();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Theory(DisplayName = "It should construct a new realm from arguments.")]
  [InlineData(null, null)]
  [InlineData("e36778cf-458c-44d1-b82f-798d01d3bf76", "aa421611-96c4-476a-baf5-73f533cfe545")]
  public void Given_Arguments_When_ctor_Then_Realm(string? actorIdValue, string? realmIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));

    Realm realm = new(_realm.UniqueSlug, _realm.Secret, actorId, realmId);

    Assert.Equal(_realm.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(_realm.Secret, realm.Secret);
    Assert.Equal(actorId, realm.CreatedBy);
    Assert.Equal(actorId, realm.UpdatedBy);

    if (realmId.HasValue)
    {
      Assert.Equal(realmId, realm.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, realm.Id.ToGuid());
    }
  }

  [Fact(DisplayName = "It should handle Description change correctly.")]
  public void Given_Changes_When_Description_Then_Changed()
  {
    Description description = new("  This is my realm!  ");
    _realm.Description = description;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(description, _realm.Description);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.Description?.Value is not null && updated.Description.Value.Equals(description));

    _realm.ClearChanges();

    _realm.Description = description;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "It should handle DisplayName change correctly.")]
  public void Given_Changes_When_DisplayName_Then_Changed()
  {
    DisplayName displayName = new(" My Realm ");
    _realm.DisplayName = displayName;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(displayName, _realm.DisplayName);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.DisplayName?.Value is not null && updated.DisplayName.Value.Equals(displayName));

    _realm.ClearChanges();

    _realm.DisplayName = displayName;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "It should handle PasswordSettings change correctly.")]
  public void Given_Changes_When_PasswordSettings_Then_Changed()
  {
    PasswordSettings passwordSettings = new(6, 1, false, true, true, true, "PBKDF2");
    _realm.PasswordSettings = passwordSettings;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(passwordSettings, _realm.PasswordSettings);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.PasswordSettings is not null && updated.PasswordSettings.Equals(passwordSettings));

    _realm.ClearChanges();

    _realm.PasswordSettings = passwordSettings;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "It should handle RequireConfirmedAccount change correctly.")]
  public void Given_Changes_When_RequireConfirmedAccount_Then_Changed()
  {
    bool requireConfirmedAccount = !_realm.RequireConfirmedAccount;
    _realm.RequireConfirmedAccount = requireConfirmedAccount;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(requireConfirmedAccount, _realm.RequireConfirmedAccount);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.RequireConfirmedAccount == requireConfirmedAccount);

    _realm.ClearChanges();

    _realm.RequireConfirmedAccount = requireConfirmedAccount;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "It should handle RequireUniqueEmail change correctly.")]
  public void Given_Changes_When_RequireUniqueEmail_Then_Changed()
  {
    bool requireUniqueEmail = !_realm.RequireUniqueEmail;
    _realm.RequireUniqueEmail = requireUniqueEmail;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(requireUniqueEmail, _realm.RequireUniqueEmail);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.RequireUniqueEmail == requireUniqueEmail);

    _realm.ClearChanges();

    _realm.RequireUniqueEmail = requireUniqueEmail;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "It should handle UniqueNameSettings change correctly.")]
  public void Given_Changes_When_UniqueNameSettings_Then_Changed()
  {
    UniqueNameSettings uniqueNameSettings = new(allowedCharacters: null);
    _realm.UniqueNameSettings = uniqueNameSettings;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(uniqueNameSettings, _realm.UniqueNameSettings);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.UniqueNameSettings is not null && updated.UniqueNameSettings.Equals(uniqueNameSettings));

    _realm.ClearChanges();

    _realm.UniqueNameSettings = uniqueNameSettings;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "It should handle Url change correctly.")]
  public void Given_Changes_When_Url_Then_Changed()
  {
    Url url = new($"https://www.{_faker.Internet.DomainName()}");
    _realm.Url = url;
    _realm.Update(_realm.CreatedBy);
    Assert.Equal(url, _realm.Url);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.Url?.Value is not null && updated.Url.Value.Equals(url));

    _realm.ClearChanges();

    _realm.Url = url;
    _realm.Update();
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("CustomKey");
    _realm.SetCustomAttribute(key, "CustomValue");
    _realm.Update();

    _realm.RemoveCustomAttribute(key);
    _realm.Update();
    Assert.False(_realm.CustomAttributes.ContainsKey(key));
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.CustomAttributes[key] is null);

    _realm.ClearChanges();
    _realm.RemoveCustomAttribute(key);
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("CustomKey");
    _realm.SetCustomAttribute(key, "CustomValue");
    _realm.Update();

    _realm.SetCustomAttribute(key, value!);
    _realm.Update();
    Assert.False(_realm.CustomAttributes.ContainsKey(key));
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.CustomAttributes[key] is null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("CustomKey");
    string value = "  CustomValue  ";

    _realm.SetCustomAttribute(key, value);
    _realm.Update();
    Assert.Equal(_realm.CustomAttributes[key], value.Trim());
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _realm.ClearChanges();
    _realm.SetCustomAttribute(key, value.Trim());
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Fact(DisplayName = "SetSecret: it should handle Secret change correctly.")]
  public void Given_Changes_When_SetSecret_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _realm.SetSecret(secret, actorId);
    Assert.Equal(secret, _realm.Secret);
    Assert.Contains(_realm.Changes, change => change is RealmSecretChanged changed && changed.ActorId == actorId && changed.Secret.Equals(secret));

    _realm.ClearChanges();

    _realm.SetSecret(secret, actorId);
    Assert.False(_realm.HasChanges);
  }

  [Fact(DisplayName = "SetUniqueSlug: it should handle changes correctly.")]
  public void Given_Changes_When_SetUniqueSlug_Then_Changed()
  {
    _realm.ClearChanges();
    _realm.SetUniqueSlug(_realm.UniqueSlug);
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    Slug uniqueSlug = new("new-slug");
    _realm.SetUniqueSlug(uniqueSlug);
    Assert.Equal(uniqueSlug, _realm.UniqueSlug);
    Assert.Contains(_realm.Changes, change => change is RealmUniqueSlugChanged changed && changed.UniqueSlug.Equals(uniqueSlug));
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Realm_When_ToString_Then_CorrectString()
  {
    Assert.StartsWith(_realm.UniqueSlug.Value, _realm.ToString());

    _realm.DisplayName = new DisplayName("My Realm");
    Assert.StartsWith(_realm.DisplayName.Value, _realm.ToString());
  }
}
