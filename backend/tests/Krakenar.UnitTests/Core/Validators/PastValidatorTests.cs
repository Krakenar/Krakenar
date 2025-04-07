using FluentValidation;

namespace Krakenar.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class PastValidatorTests
{
  private readonly ValidationContext<PastValidatorTests> _context;
  private readonly PastValidator<PastValidatorTests> _validator = new();

  public PastValidatorTests()
  {
    _context = new ValidationContext<PastValidatorTests>(this);
  }

  [Fact(DisplayName = "IsValid: it should return false when the value is in the future or the present.")]
  public void Given_FutureOrPresent_When_IsValid_Then_FalseReturned()
  {
    Assert.False(_validator.IsValid(_context, DateTime.Now.AddMinutes(1)));

    PastValidator<PastValidatorTests> validator = new(DateTime.Now.AddHours(-1));
    Assert.False(validator.IsValid(_context, DateTime.Now.AddMinutes(-15)));
  }

  [Fact(DisplayName = "IsValid: it should return true when the value is in the past.")]
  public void Given_Past_When_IsValid_Then_TrueReturned()
  {
    Assert.True(_validator.IsValid(_context, DateTime.Now.AddYears(-1)));
  }
}
