using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users.Validators;

namespace Krakenar.Core.Users;

public record Email : Contact, IEmail
{
  public const int MaximumLength = byte.MaxValue;

  public string Address { get; }

  [JsonConstructor]
  public Email(string address, bool isVerified = false) : base(isVerified)
  {
    Address = address.Trim();
    new EmailValidator().ValidateAndThrow(this);
  }

  public Email(IEmail email, bool isVerified = false) : this(email.Address, isVerified)
  {
  }

  public override string ToString() => Address;
}
