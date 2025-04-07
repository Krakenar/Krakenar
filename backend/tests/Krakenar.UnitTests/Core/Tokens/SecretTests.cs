using FluentValidation;
using FluentValidation.Results;

namespace Krakenar.Core.Tokens;

[Trait(Traits.Category, Categories.Unit)]
public class SecretTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  bnXkytd^:PfcB;xL4,hW+TJZg5G#{u`@  ";
    Secret secret = new(value);
    Assert.Equal(value.Trim(), secret.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty, or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new Secret(value));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MinimumLengthValidator" && e.PropertyName == "Value");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    var exception = Assert.Throws<ValidationException>(() => new Secret(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("MinimumLengthValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Secret_When_ToString_Then_CorrectString()
  {
    Secret secret = new("sGFbu9kMr7LwEz3jAYBt6CUHKcQpyd5T");
    Assert.Equal(secret.Value, secret.ToString());
  }

  //[Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  //public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  //{
  //  string value = $"  {RandomStringGenerator.GetString()}  ";
  //  Secret? secret = Secret.TryCreate(value);
  //  Assert.NotNull(secret);
  //  Assert.Equal(value.Trim(), secret.Value);
  //}

  //[Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  //[InlineData(null)]
  //[InlineData("")]
  //[InlineData("  ")]
  //public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  //{
  //  Assert.Null(Secret.TryCreate(value));
  //}

  //[Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  //public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  //{
  //  string value = RandomStringGenerator.GetString(999);
  //  var exception = Assert.Throws<ValidationException>(() => Secret.TryCreate(value));

  //  ValidationFailure failure = Assert.Single(exception.Errors);
  //  Assert.Equal("MaximumLengthValidator", failure.ErrorCode);
  //  Assert.Equal("Value", failure.PropertyName);
  //} // TODO(fpion): TryCreate
}
