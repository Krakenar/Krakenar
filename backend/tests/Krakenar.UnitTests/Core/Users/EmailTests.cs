using Bogus;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class EmailTests
{
  private readonly Faker _faker = new();

  [Theory(DisplayName = "ctor: it should construct a new email address.")]
  [InlineData("info@test.com", false)]
  [InlineData("  admin@test.com  ", true)]
  public void ctor_it_should_construct_a_new_email_address(string address, bool isVerified)
  {
    Email email = new(address, isVerified);
    Assert.Equal(address.Trim(), email.Address);
    Assert.Equal(isVerified, email.IsVerified);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException when the address is empty.")]
  [InlineData("")]
  [InlineData("  ")]
  public void ctor_it_should_throw_ValidationException_when_the_address_is_empty(string address)
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Email(address));
    Assert.All(exception.Errors, e =>
    {
      Assert.Equal("Address", e.PropertyName);
      Assert.True(e.ErrorCode == "EmailValidator" || e.ErrorCode == "NotEmptyValidator");
    });
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the address is not valid.")]
  public void ctor_it_should_throw_ValidationException_when_the_address_is_not_valid()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Email("aa@@bb..cc"));
    Assert.All(exception.Errors, e =>
    {
      Assert.Equal("Address", e.PropertyName);
      Assert.Equal("EmailValidator", e.ErrorCode);
    });
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the address is too long.")]
  public void ctor_it_should_throw_ValidationException_when_the_address_is_too_long()
  {
    var address = string.Concat(_faker.Random.String(Email.MaximumLength, minChar: 'a', maxChar: 'z'), '@', _faker.Internet.DomainName());

    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Email(address));
    Assert.All(exception.Errors, e =>
    {
      Assert.Equal("Address", e.PropertyName);
      Assert.Equal("MaximumLengthValidator", e.ErrorCode);
    });
  }
}
