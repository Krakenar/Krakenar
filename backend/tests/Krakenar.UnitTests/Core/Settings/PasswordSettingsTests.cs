using FluentValidation;
using PasswordSettingsDto = Krakenar.Contracts.Settings.PasswordSettings;

namespace Krakenar.Core.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class PasswordSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    PasswordSettingsDto instance = new();

    PasswordSettings settings = new(instance);

    Assert.Equal(instance.RequiredLength, settings.RequiredLength);
    Assert.Equal(instance.RequiredUniqueChars, settings.RequiredUniqueChars);
    Assert.Equal(instance.RequireNonAlphanumeric, settings.RequireNonAlphanumeric);
    Assert.Equal(instance.RequireLowercase, settings.RequireLowercase);
    Assert.Equal(instance.RequireUppercase, settings.RequireUppercase);
    Assert.Equal(instance.RequireDigit, settings.RequireDigit);
    Assert.Equal(instance.HashingStrategy, settings.HashingStrategy);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    PasswordSettings settings = new();
    Assert.Equal(8, settings.RequiredLength);
    Assert.Equal(8, settings.RequiredUniqueChars);
    Assert.True(settings.RequireNonAlphanumeric);
    Assert.True(settings.RequireLowercase);
    Assert.True(settings.RequireUppercase);
    Assert.True(settings.RequireDigit);
    Assert.Equal("PBKDF2", settings.HashingStrategy);

    settings = new(requiredLength: 6, requiredUniqueChars: 3, requireNonAlphanumeric: false, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "MD5");
    Assert.Equal(6, settings.RequiredLength);
    Assert.Equal(3, settings.RequiredUniqueChars);
    Assert.False(settings.RequireNonAlphanumeric);
    Assert.True(settings.RequireLowercase);
    Assert.True(settings.RequireUppercase);
    Assert.True(settings.RequireDigit);
    Assert.Equal("MD5", settings.HashingStrategy);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new PasswordSettings(
      requiredLength: 2,
      requiredUniqueChars: 3,
      requireNonAlphanumeric: false,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "    "));
    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "RequiredLength");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "RequiredUniqueChars");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "HashingStrategy");
  }
}
