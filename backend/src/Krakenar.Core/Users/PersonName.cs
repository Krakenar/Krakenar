using FluentValidation;
using Logitar;

namespace Krakenar.Core.Users;

public record PersonName
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public PersonName(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static string? BuildFullName(params PersonName?[] names) => BuildFullName(names.Select(name => name?.Value).ToArray());
  public static string? BuildFullName(params string?[] names) => string.Join(' ', names
    .SelectMany(name => name?.Split(' ') ?? [])
    .Where(name => !string.IsNullOrEmpty(name))).CleanTrim();

  public static PersonName? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<PersonName>
  {
    public Validator()
    {
      RuleFor(x => x.Value).PersonName();
    }
  }
}
