using FluentValidation;

namespace Krakenar.Core.Fields;

public record Placeholder
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Placeholder(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Placeholder? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Placeholder>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Placeholder();
    }
  }
}
