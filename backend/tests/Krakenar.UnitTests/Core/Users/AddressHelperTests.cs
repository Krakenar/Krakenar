namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class AddressHelperTests
{
  private readonly AddressHelper _addressHelper = new();

  [Fact(DisplayName = "GetCountry: it should return null when the country is not supported.")]
  public void GetCountry_it_should_return_null_when_the_country_is_not_supported()
  {
    Assert.Null(_addressHelper.GetCountry("QC"));
  }

  [Fact(DisplayName = "GetCountry: it should return the country settings when it is supported.")]
  public void GetCountry_it_should_return_the_country_settings_when_it_is_supported()
  {
    Assert.NotNull(_addressHelper.GetCountry("CA"));
  }

  [Fact(DisplayName = "IsSupported: it should return false when the country is not supported.")]
  public void IsSupported_it_should_return_false_when_the_country_is_not_supported()
  {
    Assert.False(_addressHelper.IsSupported("QC"));
  }

  [Fact(DisplayName = "IsSupported: it should return true when the country is supported.")]
  public void IsSupported_it_should_return_true_when_the_country_is_supported()
  {
    Assert.True(_addressHelper.IsSupported("CA"));
  }

  [Fact(DisplayName = "SupportedCountries: it should return the list of supported countries.")]
  public void SupportedCountries_it_should_return_the_list_of_supported_countries()
  {
    string[] expected = ["CA"];
    Assert.Equal(expected, _addressHelper.SupportedCountries);
  }
}
