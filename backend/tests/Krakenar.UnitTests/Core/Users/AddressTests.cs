using Bogus;
using Logitar;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class AddressTests
{
  private readonly Faker _faker = new();
  private readonly AddressHelper _addressHelper = new();

  [Theory(DisplayName = "ctor: it should construct a new postal address.")]
  [InlineData("150 Saint-Catherine St W", "Montreal", "CA", "QC", "H2X 3Y2", false)]
  [InlineData(" 150 Saint-Catherine St W", " Montreal ", " CA ", " QC ", " H2X 3Y2 ", true)]
  public void ctor_it_should_construct_a_new_address_address(string street, string locality, string country, string? region, string? postalCode, bool isVerified)
  {
    Address address = new(_addressHelper, street, locality, country, region, postalCode, isVerified);
    Assert.Equal(street.Trim(), address.Street);
    Assert.Equal(locality.Trim(), address.Locality);
    Assert.Equal(postalCode?.CleanTrim(), address.PostalCode);
    Assert.Equal(region?.CleanTrim(), address.Region);
    Assert.Equal(country.Trim(), address.Country);
    Assert.Equal(isVerified, address.IsVerified);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException when a component is empty.")]
  [InlineData("")]
  [InlineData("  ")]
  public void ctor_it_should_throw_ValidationException_when_a_component_is_empty(string value)
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Address(_addressHelper, value, value, value));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Street");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Locality");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Country");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when a component is too long.")]
  public void ctor_it_should_throw_ValidationException_when_a_component_is_too_long()
  {
    var value = _faker.Random.String(Address.MaximumLength + 1, minChar: 'A', maxChar: 'Z');

    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Address(_addressHelper, value, value, value, value, value));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Street");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Locality");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Country");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Region");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "PostalCode");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the country is not supported.")]
  public void ctor_it_should_throw_ValidationException_when_the_country_is_not_supported()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Address(_addressHelper, "150 Saint-Catherine St W", "Montreal", "QC"));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CountryValidator" && e.PropertyName == "Country");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the postal code does not match the regular expression.")]
  public void ctor_it_should_throw_ValidationException_when_the_postal_code_does_not_match_the_regular_expression()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Address(_addressHelper, "150 Saint-Catherine St W", "Montreal", "CA", "QC", "D0L7A9"));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PostalCodeValidator" && e.PropertyName == "PostalCode");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the region is not valid.")]
  public void ctor_it_should_throw_ValidationException_when_the_region_is_not_valid()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Address(_addressHelper, "150 Saint-Catherine St W", "Montreal", "CA", "ZZ", "H2X 3Y2"));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RegionValidator" && e.PropertyName == "Region");
  }

  [Fact(DisplayName = "Format: it should format a postal address.")]
  public void Format_it_should_format_a_postal_address()
  {
    Address address = new(_addressHelper, " Jean Du Pays\r\n \r\n150 Saint-Catherine St W ", " Montreal ", " CA ", " QC ", " H2X 3Y2 ");
    var expected = string.Join(Environment.NewLine, ["Jean Du Pays", "150 Saint-Catherine St W", "Montreal QC H2X 3Y2", "CA"]);
    Assert.Equal(expected, address.ToString());
  }
}
