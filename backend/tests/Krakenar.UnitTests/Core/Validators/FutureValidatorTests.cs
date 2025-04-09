using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class FutureValidatorTests
{
  private readonly ValidationContext<FutureValidatorTests> _context;
  private readonly FutureValidator<FutureValidatorTests> _validator = new();

  public FutureValidatorTests()
  {
    _context = new ValidationContext<FutureValidatorTests>(this);
  }

  [Fact(DisplayName = "IsValid: it should return false when the value is in the past or the present.")]
  public void Given_PastOrPresent_When_IsValid_Then_FalseReturned()
  {
    Assert.False(_validator.IsValid(_context, DateTime.Now.AddMinutes(-1)));

    FutureValidator<FutureValidatorTests> validator = new(DateTime.Now.AddHours(1));
    Assert.False(validator.IsValid(_context, DateTime.Now.AddMinutes(15)));
  }

  [Fact(DisplayName = "IsValid: it should return true when the value is in the future.")]
  public void Given_Future_When_IsValid_Then_TrueReturned()
  {
    Assert.True(_validator.IsValid(_context, DateTime.Now.AddDays(1)));
  }
}
