using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users;

internal record PhoneMock : IPhone
{
  public string? CountryCode { get; }
  public string Number { get; }
  public string? Extension { get; }
  public bool IsVerified { get; }

  public PhoneMock(string number = "+15148454636", string? countryCode = "CA", string? extension = "12345", bool isVerified = false)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    IsVerified = isVerified;
  }
}
