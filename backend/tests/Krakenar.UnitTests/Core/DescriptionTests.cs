using FluentValidation;
using FluentValidation.Results;

namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class DescriptionTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  Hello World!  ";
    Description description = new(value);
    Assert.Equal(value.Trim(), description.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new Description(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Description_When_ToString_Then_CorrectString()
  {
    Description description = new("Hello World!");
    Assert.Equal(description.Value, description.ToString());
  }

  [Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  {
    string value = "  Hello World!  ";
    Description? description = Description.TryCreate(value);
    Assert.NotNull(description);
    Assert.Equal(value.Trim(), description.Value);
  }

  [Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  {
    Assert.Null(Description.TryCreate(value));
  }
}
