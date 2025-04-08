namespace Krakenar.Contracts.Users;

public record Phone : Contact, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public string E164Formatted { get; set; }

  public Phone() : this(null, string.Empty, null, string.Empty)
  {
  }

  public Phone(IPhone phone, string e164Formatted) : this(phone.CountryCode, phone.Number, phone.Extension, e164Formatted, phone.IsVerified)
  {
    IsVerified = phone.IsVerified;
  }

  public Phone(string? countryCode, string number, string? extension, string e164Formatted, bool isVerified = false) : base(isVerified)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    E164Formatted = e164Formatted;
  }
}
