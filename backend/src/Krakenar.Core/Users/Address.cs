using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users.Validators;
using Logitar;
using System.Text;

namespace Krakenar.Core.Users;

public record Address : Contact, IAddress
{
  public const int MaximumLength = byte.MaxValue;

  public string Street { get; }
  public string Locality { get; }
  public string? PostalCode { get; }
  public string? Region { get; }
  public string Country { get; }

  [JsonConstructor]
  public Address(string street, string locality, string country, string? region = null, string? postalCode = null, bool isVerified = false)
    : base(isVerified)
  {
    Street = street.Trim();
    Locality = locality.Trim();
    Region = region?.CleanTrim();
    PostalCode = postalCode?.CleanTrim();
    Country = country.Trim();
  }

  public Address(IAddressHelper helper, string street, string locality, string country, string? region = null, string? postalCode = null, bool isVerified = false)
    : this(street, locality, country, region, postalCode, isVerified)
  {
    new AddressValidator(helper).ValidateAndThrow(this);
  }

  public Address(IAddress address, bool isVerified = false)
    : this(address.Street, address.Locality, address.Country, address.Region, address.PostalCode, isVerified)
  {
  }

  public Address(IAddressHelper helper, IAddress address, bool isVerified = false)
    : this(helper, address.Street, address.Locality, address.Country, address.Region, address.PostalCode, isVerified)
  {
  }

  public override string ToString()
  {
    StringBuilder formatted = new();
    string[] lines = Street.Remove("\r").Split('\n');
    foreach (string line in lines)
    {
      if (!string.IsNullOrWhiteSpace(line))
      {
        formatted.AppendLine(line.Trim());
      }
    }
    formatted.Append(Locality);
    if (Region != null)
    {
      formatted.Append(' ').Append(Region);
    }
    if (PostalCode != null)
    {
      formatted.Append(' ').Append(PostalCode);
    }
    formatted.AppendLine();
    formatted.Append(Country);
    return formatted.ToString();
  }
}
