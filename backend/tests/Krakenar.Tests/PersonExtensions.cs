using Bogus;

namespace Krakenar;

public static class PersonExtensions
{
  private static readonly Random _random = new();

  public static string BuildHealthInsuranceNumber(this Person person, bool withSpaces = false)
  {
    StringBuilder healthInsuranceNumber = new();
    healthInsuranceNumber.Append(person.LastName[..3].ToUpperInvariant());
    healthInsuranceNumber.Append(person.FirstName[..1].ToUpperInvariant());
    if (withSpaces)
    {
      healthInsuranceNumber.Append(' ');
    }
    healthInsuranceNumber.Append((person.DateOfBirth.Year % 100).ToString("D2"));
    switch (person.Gender)
    {
      case Bogus.DataSets.Name.Gender.Female:
        healthInsuranceNumber.Append((person.DateOfBirth.Month + 50).ToString("D2"));
        break;
      default:
        healthInsuranceNumber.Append(person.DateOfBirth.Month.ToString("D2"));
        break;
    }
    if (withSpaces)
    {
      healthInsuranceNumber.Append(' ');
    }
    healthInsuranceNumber.Append(person.DateOfBirth.Day.ToString("D2"));
    healthInsuranceNumber.Append(_random.Next(0, 99 + 1).ToString("D2"));
    return healthInsuranceNumber.ToString();
  }
}
