using FluentValidation;

namespace Krakenar.Core.Encryption;

public record EncryptedString
{
  public string Value { get; }

  public EncryptedString(string value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<EncryptedString>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty();
    }
  }
}
