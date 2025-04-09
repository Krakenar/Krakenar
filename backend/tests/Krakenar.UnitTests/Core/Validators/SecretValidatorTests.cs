using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class SecretValidatorTests
{
  private readonly ValidationContext<SecretValidatorTests> _context;
  private readonly SecretValidator<SecretValidatorTests> _validator = new();

  public SecretValidatorTests()
  {
    _context = new ValidationContext<SecretValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid secret.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("WPgG9JHw4v5MXbdk")]
  [InlineData("BkHVPzjq8aUregfwN2D7cMmCsQyGAdF4LZYWbgC29WYXDayGRNtpPuF8EBT3v64xKA5kLhqz")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid secret.")]
  [InlineData("pJ9UdMCTHXb8RWAVhtw6cQvKZg5sLxNP")]
  [InlineData("  PuEwVjpMsZrqmUunaJ3sbTSKw59gGLf2    jZcsxhqyRLaxH46z3tWxJUfQyvbXNDnK  ")]
  public void Given_Secret_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
