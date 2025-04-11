using FluentValidation;
using Krakenar.Core.Settings;

namespace Krakenar.Infrastructure.Passwords;

[Trait(Traits.Category, Categories.Unit)]
public class PasswordInputTests
{
  private readonly PasswordSettings _settings = new();

  [Theory(DisplayName = "It should construct the correct instance from a valid password.")]
  [InlineData("P@s$W0rD")]
  public void Given_Valid_When_ctor_Then_Constructed(string passwordValue)
  {
    PasswordInput password = new(_settings, passwordValue);
    Assert.Equal(passwordValue, password.Password);
  }

  [Fact(DisplayName = "It should throw ValidationException when the password is not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new PasswordInput(_settings, string.Empty));
    Assert.Equal(7, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordTooShort"
      && e.ErrorMessage == "Passwords must be at least 8 characters."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUniqueChars"
      && e.ErrorMessage == "Passwords must use at least 8 different characters."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresNonAlphanumeric"
      && e.ErrorMessage == "Passwords must have at least one non alphanumeric character."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresLower"
      && e.ErrorMessage == "Passwords must have at least one lowercase ('a'-'z')."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUpper"
      && e.ErrorMessage == "Passwords must have at least one uppercase ('A'-'Z')."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresDigit"
      && e.ErrorMessage == "Passwords must have at least one digit ('0'-'9')."
      && e.PropertyName == "Password");
  }
}
