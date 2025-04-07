using FluentValidation;
using Logitar.Security.Claims;

namespace Krakenar.Core.Users;

public record Gender
{
  public const int MaximumLength = byte.MaxValue;

  private static readonly HashSet<string> _knownValues = new([Genders.Female, Genders.Male]);
  public static IReadOnlyCollection<string> KnownValues => _knownValues.ToList().AsReadOnly();

  public string Value { get; }

  public Gender(string value)
  {
    Value = Format(value);
    new Validator().ValidateAndThrow(this);
  }

  public static bool IsKnown(string value) => _knownValues.Contains(value.Trim().ToLower());

  public static Gender? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  private static string Format(string value)
  {
    value = value.Trim();
    return IsKnown(value) ? value.ToLower() : value;
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Gender>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Gender();
    }
  }
}
