using Bogus;
using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class UrlValidatorTests
{
  private readonly Faker _faker = new();
  private readonly ValidationContext<UrlValidatorTests> _context;
  private readonly UrlValidator<UrlValidatorTests> _validator = new();

  public UrlValidatorTests()
  {
    _context = new ValidationContext<UrlValidatorTests>(this);
  }

  [Fact(DisplayName = "GetDefaultMessageTemplate: it should return the correct default message template.")]
  public void Given_Validator_When_GetDefaultMessageTemplate_Then_CorrectMessage()
  {
    string expected = "'{PropertyName}' must be a valid absolute Uniform Resource Locator (URL) using one of the following schemes: http, https.";
    Assert.Equal(expected, _validator.GetDefaultMessageTemplate(errorCode: string.Empty));
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid URL.")]
  [InlineData("invalid")]
  [InlineData("/test/123")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Fact(DisplayName = "IsValid: it should return true when the scheme is allowed.")]
  public void Given_SchemeAllowed_When_IsValid_Then_TrueReturned()
  {
    UrlValidator<UrlValidatorTests> _validator = new(["FTP", "FTPS"]);
    Assert.True(_validator.IsValid(_context, $"ftps://{_faker.Internet.Ip()}"));
  }

  [Fact(DisplayName = "IsValid: it should return true when the value is a valid URL.")]
  public void Given_Url_When_IsValid_Then_TrueReturned()
  {
    Assert.True(_validator.IsValid(_context, $"https://www.{_faker.Internet.DomainName()}"));
  }
}
