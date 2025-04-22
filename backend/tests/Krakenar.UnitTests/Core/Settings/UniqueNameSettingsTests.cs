using FluentValidation;
using UniqueNameSettingsDto = Krakenar.Contracts.Settings.UniqueNameSettings;

namespace Krakenar.Core.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class UniqueNameSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    UniqueNameSettingsDto instance = new()
    {
      AllowedCharacters = "0123456789"
    };

    UniqueNameSettings settings = new(instance);

    Assert.Equal(instance.AllowedCharacters, settings.AllowedCharacters);
  }

  [Theory(DisplayName = "It should construct the correct instance from arguments.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  [InlineData("012345067890")]
  [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+")]
  public void Given_Arguments_When_ctor_Then_Constructed(string? allowedCharacters)
  {
    UniqueNameSettings settings = new(allowedCharacters);

    if (string.IsNullOrWhiteSpace(allowedCharacters))
    {
      Assert.Null(settings.AllowedCharacters);
    }
    else
    {
      Assert.Equal(new string([.. new HashSet<char>(allowedCharacters)]), settings.AllowedCharacters);
    }
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    StringBuilder characters = new(capacity: 256);
    HashSet<int> skip = [127, 129, 141, 143, 144, 157];
    for (int i = 32; i <= 300; i++)
    {
      if (!skip.Contains(i))
      {
        characters.Append((char)i);
      }
    }

    var exception = Assert.Throws<ValidationException>(() => new UniqueNameSettings(characters.ToString()));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "AllowedCharacters");
  }
}
