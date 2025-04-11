using FluentValidation;
using FluentValidation.Results;
using Logitar.Security.Cryptography;

namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class SlugTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  hello-world  ";
    Slug slug = new(value);
    Assert.Equal(value.Trim(), slug.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty, or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new Slug(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SlugValidator" && e.PropertyName == "Value");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new Slug("test--not_valid!"));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("SlugValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Slug_When_ToString_Then_CorrectString()
  {
    Slug slug = new("Administrator");
    Assert.Equal(slug.Value, slug.ToString());
  }

  [Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  {
    string value = "  the-new-world  ";
    Slug? slug = Slug.TryCreate(value);
    Assert.NotNull(slug);
    Assert.Equal(value.Trim(), slug.Value);
  }

  [Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  {
    Assert.Null(Slug.TryCreate(value));
  }

  [Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => Slug.TryCreate(value));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SlugValidator" && e.PropertyName == "Value");
  }
}
