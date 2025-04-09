using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class SlugValidatorTests
{
  private readonly ValidationContext<SlugValidatorTests> _context;
  private readonly SlugValidator<SlugValidatorTests> _validator = new();

  public SlugValidatorTests()
  {
    _context = new ValidationContext<SlugValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid slug.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("-")]
  [InlineData("test-")]
  [InlineData("hello--world")]
  [InlineData("test123!")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid slug.")]
  [InlineData("test")]
  [InlineData("hello-world-123")]
  public void Given_Slug_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
