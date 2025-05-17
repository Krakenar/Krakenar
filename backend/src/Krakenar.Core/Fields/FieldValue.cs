using FluentValidation;

namespace Krakenar.Core.Fields;

public record FieldValue
{
  public string Value { get; }

  public FieldValue(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<FieldValue>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty();
    }
  }
}
