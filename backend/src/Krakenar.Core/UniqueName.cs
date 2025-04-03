using FluentValidation;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core;

public record UniqueName
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public UniqueName(IUniqueNameSettings uniqueNameSettings, string value)
  {
    Value = value.Trim();
    new Validator(uniqueNameSettings).ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<UniqueName>
  {
    public Validator(IUniqueNameSettings uniqueNameSettings)
    {
      RuleFor(x => x.Value).UniqueName(uniqueNameSettings);
    }
  }
}
