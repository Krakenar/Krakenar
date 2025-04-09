using Bogus;
using Logitar;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class PhoneTests
{
  private readonly Faker _faker = new();

  [Theory(DisplayName = "ctor: it should construct a new phone number.")]
  [InlineData("+15148454636", "CA", "  ", false)]
  [InlineData("  (514) 845-4636  ", "CA", "12345", true)]
  public void ctor_it_should_construct_a_new_phone_number(string number, string? countryCode, string? extension, bool isVerified)
  {
    Phone phone = new(number, countryCode, extension, isVerified);
    Assert.Equal(countryCode?.CleanTrim(), phone.CountryCode);
    Assert.Equal(number.Trim(), phone.Number);
    Assert.Equal(extension?.CleanTrim(), phone.Extension);
    Assert.Equal(isVerified, phone.IsVerified);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException when the number is empty.")]
  [InlineData("")]
  [InlineData("  ")]
  public void ctor_it_should_throw_ValidationException_when_the_number_is_empty(string number)
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Phone(number));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Number");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the number is not valid.")]
  public void ctor_it_should_throw_ValidationException_when_the_number_is_not_valid()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Phone("+15148454636+12345", "CA", "12345"));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PhoneValidator" && e.PropertyName == "");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when a value is too long.")]
  public void ctor_it_should_throw_ValidationException_when_a_value_is_too_long()
  {
    var countryCode = "CAD";
    var number = _faker.Random.String(Phone.NumberMaximumLength + 1, minChar: '0', maxChar: '9');
    var extension = _faker.Random.String(Phone.ExtensionMaximumLength + 1, minChar: '0', maxChar: '9');

    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Phone(number, countryCode, extension));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ExactLengthValidator" && e.PropertyName == "CountryCode");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Number");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Extension");
  }
}
