using FluentValidation;
using FluentValidation.Validators;
using Logitar;

namespace Krakenar.Core.Validators;

public class FutureValidator<T> : IPropertyValidator<T, DateTime>
{
  public DateTime Moment { get; }
  public string Name { get; } = "FutureValidator";

  public FutureValidator(DateTime? moment = null)
  {
    Moment = moment ?? DateTime.Now;
  }

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be a date and time set in the future.";
  }

  public bool IsValid(ValidationContext<T> context, DateTime value)
  {
    return value.AsUniversalTime() > Moment.AsUniversalTime();
  }
}
