using FluentValidation;
using FluentValidation.Results;
using Logitar.Security.Cryptography;

namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class DisplayNameTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  Administrator  ";
    DisplayName displayName = new(value);
    Assert.Equal(value.Trim(), displayName.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty, or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new DisplayName(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => new DisplayName(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_DisplayName_When_ToString_Then_CorrectString()
  {
    DisplayName displayName = new("Administrator");
    Assert.Equal(displayName.Value, displayName.ToString());
  }

  [Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  {
    string value = "  Administrator  ";
    DisplayName? displayName = DisplayName.TryCreate(value);
    Assert.NotNull(displayName);
    Assert.Equal(value.Trim(), displayName.Value);
  }

  [Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  {
    Assert.Null(DisplayName.TryCreate(value));
  }

  [Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => DisplayName.TryCreate(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }
}
