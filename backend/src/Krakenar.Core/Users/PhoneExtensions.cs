using Krakenar.Contracts.Users;
using PhoneNumbers;

namespace Krakenar.Core.Users;

public static class PhoneExtensions
{
  public static string FormatToE164(this IPhone phone, string defaultRegion = "US")
  {
    PhoneNumber instance = phone.Parse(defaultRegion);
    return PhoneNumberUtil.GetInstance().Format(instance, PhoneNumberFormat.E164);
  }

  public static bool IsValid(this IPhone phone, string defaultRegion = "US")
  {
    try
    {
      _ = phone.Parse(defaultRegion);
      return true;
    }
    catch (NumberParseException)
    {
      return false;
    }
  }

  private static PhoneNumber Parse(this IPhone phone, string defaultRegion)
  {
    string formatted = string.IsNullOrWhiteSpace(phone.Extension)
      ? phone.Number : $"{phone.Number} x{phone.Extension}";

    return PhoneNumberUtil.GetInstance().Parse(formatted, phone.CountryCode ?? defaultRegion);
  }
}
