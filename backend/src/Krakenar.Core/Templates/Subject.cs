using FluentValidation;

namespace Krakenar.Core.Templates;

public record Subject
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Subject(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Subject? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Subject>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Subject();
    }
  }
}
