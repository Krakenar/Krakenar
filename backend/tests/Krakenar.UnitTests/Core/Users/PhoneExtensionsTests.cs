namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class PhoneExtensionsTests
{
  [Fact(DisplayName = "FormatToE164: it should format the phone to E.164.")]
  public void FormatToE164_it_should_format_the_phone_to_E_164()
  {
    Phone phone = new("+1 (234) 567-8900", "CA", "1234567");
    Assert.Equal("+12345678900", phone.FormatToE164());
  }

  [Fact(DisplayName = "IsValid: it should return false when the phone is not valid.")]
  public void IsValid_it_should_return_false_when_the_phone_is_not_valid()
  {
    PhoneMock phone = new("ABCDEFGHIJ", "FR", extension: null);
    Assert.False(phone.IsValid());
  }

  [Fact(DisplayName = "IsValid: it should return true when the phone is valid.")]
  public void IsValid_it_should_return_true_when_the_phone_is_valid()
  {
    PhoneMock phone = new();
    Assert.True(phone.IsValid());
  }
}
