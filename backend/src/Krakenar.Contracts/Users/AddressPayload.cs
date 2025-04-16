namespace Krakenar.Contracts.Users;

public record AddressPayload : ContactPayload, IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }

  public AddressPayload() : this(string.Empty, string.Empty, string.Empty)
  {
  }

  public AddressPayload(string street, string locality, string country, string? postalCode = null, string? region = null, bool isVerified = false) : base(isVerified)
  {
    Street = street;
    Locality = locality;
    Country = country;
    PostalCode = postalCode;
    Region = region;
  }
}
