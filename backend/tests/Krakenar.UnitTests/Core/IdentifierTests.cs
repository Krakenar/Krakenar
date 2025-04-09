using FluentValidation;
using FluentValidation.Results;
using Logitar.Security.Cryptography;

namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class IdentifierTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  HealthInsuranceNumber  ";
    Identifier identifier = new(value);
    Assert.Equal(value.Trim(), identifier.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new Identifier(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => new Identifier(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Value");
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Identifier_When_ToString_Then_CorrectString()
  {
    Identifier identifier = new("HealthInsuranceNumber");
    Assert.Equal(identifier.Value, identifier.ToString());
  }

  [Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  {
    string value = "  HealthInsuranceNumber  ";
    Identifier? identifier = Identifier.TryCreate(value);
    Assert.NotNull(identifier);
    Assert.Equal(value.Trim(), identifier.Value);
  }

  [Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  {
    Assert.Null(Identifier.TryCreate(value));
  }

  [Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => Identifier.TryCreate(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Value");
  }
}
