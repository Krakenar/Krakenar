using FluentValidation;

namespace Krakenar.Core.Tokens;

public record Secret
{
  public const int MinimumLength = 256 / 8;
  public const int MaximumLength = 512 / 8;

  public string Value { get; }

  public Secret(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Secret>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty().MinimumLength(MinimumLength); // NOTE(fpion): secrets should be encrypted. An encrypted secret cannot be shorter than the original.
    }
  }
}
