using FluentValidation;
using FluentValidation.Results;

namespace Krakenar.Core.Fields;

[Trait(Traits.Category, Categories.Unit)]
public class FieldValueTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  Hello World!  ";
    FieldValue fieldValue = new(value);
    Assert.Equal(value.Trim(), fieldValue.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty, or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new FieldValue(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_FieldValue_When_ToString_Then_CorrectString()
  {
    FieldValue fieldValue = new("Hello World!");
    Assert.Equal(fieldValue.Value, fieldValue.ToString());
  }
}
