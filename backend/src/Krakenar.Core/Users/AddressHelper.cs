namespace Krakenar.Core.Users;

public interface IAddressHelper
{
  IReadOnlyCollection<string> SupportedCountries { get; }

  CountrySettings? GetCountry(string country);

  bool IsSupported(string country);
}

public class AddressHelper : IAddressHelper
{
  protected virtual Dictionary<string, CountrySettings> Countries { get; } = new()
  {
    ["CA"] = new()
    {
      PostalCode = "[ABCEGHJ-NPRSTVXY]\\d[ABCEGHJ-NPRSTV-Z][ -]?\\d[ABCEGHJ-NPRSTV-Z]\\d$",
      Regions = ["AB", "BC", "MB", "NB", "NL", "NT", "NS", "NU", "ON", "PE", "QC", "SK", "YT"]
    }
  };

  public AddressHelper(IEnumerable<KeyValuePair<string, CountrySettings>>? countries = null)
  {
    if (countries != null)
    {
      foreach (KeyValuePair<string, CountrySettings> country in countries)
      {
        Countries[country.Key] = country.Value;
      }
    }
  }

  public virtual IReadOnlyCollection<string> SupportedCountries => Countries.Keys;

  public virtual CountrySettings? GetCountry(string country) => Countries.TryGetValue(country, out CountrySettings? settings) ? settings : null;

  public virtual bool IsSupported(string country) => Countries.ContainsKey(country);
}
