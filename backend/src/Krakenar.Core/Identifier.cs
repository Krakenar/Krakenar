using FluentValidation;

namespace Krakenar.Core;

public record Identifier
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Identifier(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Identifier? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Identifier>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Identifier();
    }
  }
}
