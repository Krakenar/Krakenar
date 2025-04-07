using Bogus;
using FluentValidation.Results;
using Krakenar.Core.Settings;
using Logitar.Security.Cryptography;

namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class UniqueNameTests
{
  private readonly Faker _faker = new();
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = $"  {_faker.Person.UserName}  ";
    UniqueName uniqueName = new(_uniqueNameSettings, value);
    Assert.Equal(value.Trim(), uniqueName.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty, or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new UniqueName(_uniqueNameSettings, value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new UniqueName(_uniqueNameSettings, value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Value");
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_UniqueName_When_ToString_Then_CorrectString()
  {
    UniqueName uniqueName = new(_uniqueNameSettings, _faker.Person.UserName);
    Assert.Equal(uniqueName.Value, uniqueName.ToString());
  }

  //[Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  //public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  //{
  //  string value = $"  {_faker.Person.UserName}  ";
  //  UniqueName? uniqueName = UniqueName.TryCreate(_uniqueNameSettings, value);
  //  Assert.NotNull(uniqueName);
  //  Assert.Equal(value.Trim(), uniqueName.Value);
  //}

  //[Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  //[InlineData(null)]
  //[InlineData("")]
  //[InlineData("  ")]
  //public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  //{
  //  Assert.Null(UniqueName.TryCreate(_uniqueNameSettings, value));
  //}

  //[Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  //public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  //{
  //  string value = RandomStringGenerator.GetString(999);
  //  var exception = Assert.Throws<FluentValidation.ValidationException>(() => UniqueName.TryCreate(_uniqueNameSettings, value));

  //  Assert.Equal(2, exception.Errors.Count());
  //  Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "Value");
  //  Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Value");
  //} // TODO(fpion): TryCreate
}
