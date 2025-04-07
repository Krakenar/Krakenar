using FluentValidation;
using Logitar.Security.Cryptography;
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

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    UniqueNameSettings settings = new();
    Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+", settings.AllowedCharacters);

    string allowedCharacters = "0123456789";
    settings = new(allowedCharacters);
    Assert.Equal(allowedCharacters, settings.AllowedCharacters);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new UniqueNameSettings(RandomStringGenerator.GetString(999)));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "AllowedCharacters");
  }
}
