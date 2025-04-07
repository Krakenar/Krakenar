using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class IdentifierValidatorTests
{
  private readonly ValidationContext<IdentifierValidatorTests> _context;
  private readonly IdentifierValidator<IdentifierValidatorTests> _validator = new();

  public IdentifierValidatorTests()
  {
    _context = new ValidationContext<IdentifierValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid identifier.")]
  [InlineData("    ")]
  [InlineData("123_test")]
  [InlineData("test123!")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid identifier.")]
  [InlineData("")]
  [InlineData("_test")]
  [InlineData("test_123")]
  public void Given_Identifier_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
