using FluentValidation;
using Logitar.Security.Cryptography;

namespace Krakenar.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class TimeZoneTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string id = "  America/Montreal  ";
    TimeZone timeZone = new(id);
    Assert.Equal(id.Trim(), timeZone.Id);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new TimeZone(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "TimeZoneValidator" && e.PropertyName == "Id");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => new TimeZone(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Id");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "TimeZoneValidator" && e.PropertyName == "Id");
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_TimeZone_When_ToString_Then_CorrectString()
  {
    TimeZone timeZone = new("Asia/Singapore");
    Assert.Equal(timeZone.Id, timeZone.ToString());
  }

  [Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  {
    string id = "  Europe/Paris  ";
    TimeZone? timeZone = TimeZone.TryCreate(id);
    Assert.NotNull(timeZone);
    Assert.Equal(id.Trim(), timeZone.Id);
  }

  [Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  {
    Assert.Null(TimeZone.TryCreate(value));
  }

  [Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => TimeZone.TryCreate(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Id");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "TimeZoneValidator" && e.PropertyName == "Id");
  }
}
