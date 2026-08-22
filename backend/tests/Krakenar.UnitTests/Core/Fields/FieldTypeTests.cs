using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Settings;

namespace Krakenar.Core.Fields;

[Trait(Traits.Category, Categories.Unit)]
public class FieldTypeTests
{
  [Fact(DisplayName = "SetSettings: it should not raise an event when the select settings are unchanged.")]
  public void Given_EquivalentSelectSettings_When_SetSettings_Then_DoNothing()
  {
    UniqueNameSettings uniqueNameSettings = new();
    SelectSettings settings = new(
    [
      new SelectOption("linux_sysadmin"),
      new SelectOption("Software Architecture", value: "software-architecture")
    ], isMultiple: true);
    FieldType fieldType = new(new UniqueName(uniqueNameSettings, "Skills"), settings);
    fieldType.ClearChanges();

    SelectSettings equivalent = new(
    [
      new SelectOption("linux_sysadmin"),
      new SelectOption("Software Architecture", value: "software-architecture")
    ], isMultiple: true);
    fieldType.SetSettings(equivalent);

    Assert.False(fieldType.HasChanges);
    Assert.Empty(fieldType.Changes);
  }
}
