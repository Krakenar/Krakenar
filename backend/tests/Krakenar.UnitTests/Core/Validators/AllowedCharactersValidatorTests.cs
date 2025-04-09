using Bogus;
using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class AllowedCharactersValidatorTests
{
  private readonly Faker _faker = new();
  private readonly ValidationContext<AllowedCharactersValidatorTests> _context;
  private readonly AllowedCharactersValidator<AllowedCharactersValidatorTests> _validator = new("0123456789");

  public AllowedCharactersValidatorTests()
  {
    _context = new ValidationContext<AllowedCharactersValidatorTests>(this);
  }

  [Fact(DisplayName = "GetDefaultMessageTemplate: it should return the correct default message template.")]
  public void Given_Validator_When_GetDefaultMessageTemplate_Then_CorrectMessage()
  {
    string expected = string.Format("'{{PropertyName}}' may only include the following characters: {0}", _validator.AllowedCharacters);
    Assert.Equal(expected, _validator.GetDefaultMessageTemplate(errorCode: string.Empty));
  }

  [Fact(DisplayName = "IsValid: it should return false when there are characters that are not allowed.")]
  public void Given_CharactersNotAllowed_When_IsValid_Then_FalseReturned()
  {
    string value = "invalid";
    Assert.False(_validator.IsValid(_context, value));
  }

  [Fact(DisplayName = "IsValid: it should return true when the allowed characters are null.")]
  public void Given_NullAllowedCharacters_When_IsValid_Then_TrueReturned()
  {
    string value = "Test123!";

    AllowedCharactersValidator<AllowedCharactersValidatorTests> validator = new(allowedCharacters: null);
    Assert.True(validator.IsValid(_context, value));
  }

  [Fact(DisplayName = "IsValid: it should return true when all characters are allowed.")]
  public void Given_OnlyAllowedCharacters_When_IsValid_Then_TrueReturned()
  {
    string value = _faker.Random.String(6, minChar: '0', maxChar: '9');
    Assert.True(_validator.IsValid(_context, value));
  }
}
