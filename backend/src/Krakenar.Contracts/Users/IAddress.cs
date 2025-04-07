namespace Krakenar.Contracts.Users;

public interface IAddress : IContact
{
  string Street { get; }
  string Locality { get; }
  string? PostalCode { get; }
  string? Region { get; }
  string Country { get; }
}
