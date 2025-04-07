using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class LocaleValidatorTests
{
  private readonly ValidationContext<LocaleValidatorTests> _context;
  private readonly LocaleValidator<LocaleValidatorTests> _validator = new();

  public LocaleValidatorTests()
  {
    _context = new ValidationContext<LocaleValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid locale.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("invalid")]
  [InlineData("es-CA")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid locale.")]
  [InlineData("fr")]
  [InlineData("fr-CA")]
  public void Given_Locale_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
