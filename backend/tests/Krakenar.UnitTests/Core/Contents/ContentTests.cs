using Krakenar.Core.Fields;
using Krakenar.Core.Localization;
using Krakenar.Core.Settings;

namespace Krakenar.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTests
{
  [Fact(DisplayName = "SetInvariant: it should not raise an event when the invariant is unchanged.")]
  public void Given_EquivalentInvariant_When_SetInvariant_Then_DoNothing()
  {
    UniqueNameSettings uniqueNameSettings = new();
    ContentType contentType = new(new Identifier("Product"));
    Guid fieldId = Guid.NewGuid();
    ContentLocale invariant = new(
      new UniqueName(uniqueNameSettings, "shure-sm57"),
      new DisplayName("Shure SM57"),
      new Description("Dynamic instrument microphone"),
      new Dictionary<Guid, FieldValue> { [fieldId] = new("20834") });
    Content content = new(contentType, invariant);
    content.ClearChanges();

    ContentLocale equivalent = new(
      invariant.UniqueName,
      invariant.DisplayName,
      invariant.Description,
      new Dictionary<Guid, FieldValue> { [fieldId] = new("20834") });
    content.SetInvariant(equivalent);

    Assert.False(content.HasChanges);
    Assert.Empty(content.Changes);
  }

  [Fact(DisplayName = "SetLocale: it should not raise an event when the locale is unchanged.")]
  public void Given_EquivalentLocale_When_SetLocale_Then_DoNothing()
  {
    UniqueNameSettings uniqueNameSettings = new();
    ContentType contentType = new(new Identifier("Product"), isInvariant: false);
    Content content = new(contentType, new ContentLocale(new UniqueName(uniqueNameSettings, "shure-sm57")));

    Language language = new(new Locale("en"), isDefault: true);
    Guid fieldId = Guid.NewGuid();
    ContentLocale locale = new(
      new UniqueName(uniqueNameSettings, "shure-sm57"),
      new DisplayName("Shure SM57"),
      new Description("Dynamic instrument microphone"),
      new Dictionary<Guid, FieldValue> { [fieldId] = new("20834") });
    content.SetLocale(language, locale);
    content.ClearChanges();

    ContentLocale equivalent = new(
      locale.UniqueName,
      locale.DisplayName,
      locale.Description,
      new Dictionary<Guid, FieldValue> { [fieldId] = new("20834") });
    content.SetLocale(language, equivalent);

    Assert.False(content.HasChanges);
    Assert.Empty(content.Changes);
  }
}
