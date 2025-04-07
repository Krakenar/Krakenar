using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class TimeZoneValidatorTests
{
  private readonly ValidationContext<TimeZoneValidatorTests> _context;
  private readonly TimeZoneValidator<TimeZoneValidatorTests> _validator = new();

  public TimeZoneValidatorTests()
  {
    _context = new ValidationContext<TimeZoneValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid time zone.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("invalid")]
  [InlineData("America/Singapore")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid time zone.")]
  [InlineData("America/New_York")]
  [InlineData("America/Toronto")]
  [InlineData("Asia/Singapore")]
  public void Given_TimeZone_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
