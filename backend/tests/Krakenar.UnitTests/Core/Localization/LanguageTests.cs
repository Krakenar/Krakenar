using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageTests
{
  private readonly Language _language = new(new Locale("en"), isDefault: true);

  [Fact(DisplayName = "Delete: it should delete the language.")]
  public void Given_Language_When_Delete_Then_Deleted()
  {
    Assert.False(_language.IsDeleted);

    _language.Delete();
    Assert.True(_language.IsDeleted);
    Assert.Contains(_language.Changes, change => change is LanguageDeleted);

    _language.ClearChanges();
    _language.Delete();
    Assert.False(_language.HasChanges);
    Assert.Empty(_language.Changes);
  }

  [Theory(DisplayName = "It should construct a new language from arguments.")]
  [InlineData(null, null, null, false)]
  [InlineData("85f7a0c1-18f5-48ba-a94d-6343a3711932", "d9c8b704-919d-477e-94f8-360e6b64f315", "10a6cf37-0918-47a3-8580-1f30f30bb8de", true)]
  public void Given_Arguments_When_ctor_Then_Session(string? actorIdValue, string? realmIdValue, string? languageIdValue, bool isDefault)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    LanguageId? languageId = languageIdValue is null ? null : new(Guid.Parse(languageIdValue), realmId);

    Locale locale = new("en");
    Language language = new(locale, isDefault, actorId, languageId);

    Assert.Equal(locale, language.Locale);
    Assert.Equal(isDefault, language.IsDefault);
    Assert.Equal(actorId, language.CreatedBy);
    Assert.Equal(actorId, language.UpdatedBy);

    Assert.Equal(realmId, language.RealmId);
    if (languageId.HasValue)
    {
      Assert.Equal(languageId, language.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, language.EntityId);
    }
  }

  [Fact(DisplayName = "SetDefault: it should handle changes correctly.")]
  public void Given_Changes_When_SetDefault_Then_Changed()
  {
    _language.ClearChanges();
    _language.SetDefault(_language.IsDefault);
    Assert.False(_language.HasChanges);
    Assert.Empty(_language.Changes);

    bool isDefault = !_language.IsDefault;
    _language.SetDefault(isDefault);
    Assert.Equal(isDefault, _language.IsDefault);
    Assert.Contains(_language.Changes, change => change is LanguageSetDefault changed && changed.IsDefault == isDefault);
  }

  [Fact(DisplayName = "SetLocale: it should handle changes correctly.")]
  public void Given_Changes_When_SetLocale_Then_Changed()
  {
    _language.ClearChanges();
    _language.SetLocale(_language.Locale);
    Assert.False(_language.HasChanges);
    Assert.Empty(_language.Changes);

    Locale locale = new("fr");
    _language.SetLocale(locale);
    Assert.Equal(locale, _language.Locale);
    Assert.Contains(_language.Changes, change => change is LanguageLocaleChanged changed && changed.Locale.Equals(locale));
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Language_When_ToString_Then_CorrectString()
  {
    Assert.StartsWith(_language.Locale.ToString(), _language.ToString());
  }
}
