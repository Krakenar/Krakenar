using FluentValidation;

namespace Krakenar.Core;

public record CustomIdentifier
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public CustomIdentifier(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static CustomIdentifier? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<CustomIdentifier>
  {
    public Validator()
    {
      RuleFor(x => x.Value).CustomIdentifier();
    }
  }
}
