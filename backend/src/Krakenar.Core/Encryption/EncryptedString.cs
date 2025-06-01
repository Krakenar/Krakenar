using FluentValidation;
using Krakenar.Core.Realms;

namespace Krakenar.Core.Encryption;

public record EncryptedString // TODO(fpion): tests
{
  public string Value { get; }

  public EncryptedString(string value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public static EncryptedString Encrypt(string decrypted, IEncryptionManager manager, RealmId? realmId = null) => manager.Encrypt(decrypted, realmId);

  public string Decrypt(IEncryptionManager manager, RealmId? realmId = null) => manager.Decrypt(this, realmId);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<EncryptedString>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty();
    }
  }
}
