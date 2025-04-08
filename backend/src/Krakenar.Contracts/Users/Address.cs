namespace Krakenar.Contracts.Users;

public record Address : Contact, IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }
  public string Formatted { get; set; }

  public Address()
    : this(string.Empty, string.Empty, null, null, string.Empty, string.Empty)
  {
  }

  public Address(IAddress address, string formatted)
    : this(address.Street, address.Locality, address.PostalCode, address.Region, address.Country, formatted, address.IsVerified)
  {
    IsVerified = address.IsVerified;
  }

  public Address(string street, string locality, string? postalCode, string? region, string country, string formatted, bool isVerified = false)
    : base(isVerified)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
    Formatted = formatted;
  }
}
