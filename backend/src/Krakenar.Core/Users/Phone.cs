using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users.Validators;
using Logitar;

namespace Krakenar.Core.Users;

public record Phone : Contact, IPhone
{
  public const int CountryCodeMaximumLength = 2;
  public const int NumberMaximumLength = 20;
  public const int ExtensionMaximumLength = 10;

  public string? CountryCode { get; }
  public string Number { get; }
  public string? Extension { get; }

  [JsonConstructor]
  public Phone(string number, string? countryCode = null, string? extension = null, bool isVerified = false) : base(isVerified)
  {
    CountryCode = countryCode?.CleanTrim();
    Number = number.Trim();
    Extension = extension?.CleanTrim();
    new PhoneValidator().ValidateAndThrow(this);
  }

  public Phone(IPhone phone, bool isVerified = false) : this(phone.Number, phone.CountryCode, phone.Extension, isVerified)
  {
  }

  public override string ToString() => this.FormatToE164();
}
