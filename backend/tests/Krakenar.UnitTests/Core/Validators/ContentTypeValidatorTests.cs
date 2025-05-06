using FluentValidation;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeValidatorTests
{
  private readonly ValidationContext<ContentTypeValidatorTests> _context;
  private readonly ContentTypeValidator<ContentTypeValidatorTests> _validator = new();

  public ContentTypeValidatorTests()
  {
    _context = new ValidationContext<ContentTypeValidatorTests>(this);
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
