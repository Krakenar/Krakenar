using FluentValidation;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class MediaTypeValidatorTests
{
  private readonly ValidationContext<MediaTypeValidatorTests> _context;
  private readonly MediaTypeValidator<MediaTypeValidatorTests> _validator = new();

  public MediaTypeValidatorTests()
  {
    _context = new ValidationContext<MediaTypeValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid content type.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData(MediaTypeNames.Application.Json)]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid content type.")]
  [InlineData(MediaTypeNames.Text.Html)]
  [InlineData(MediaTypeNames.Text.Plain)]
  public void Given_Secret_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
