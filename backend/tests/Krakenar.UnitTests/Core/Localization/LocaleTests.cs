using FluentValidation;
using Logitar.Security.Cryptography;

namespace Krakenar.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class LocaleTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    CultureInfo culture = CultureInfo.GetCultureInfo("fr-CA");
    Locale locale = new($"  {culture}  ");
    Assert.Equal(culture.Name, locale.Code);
    Assert.Equal(culture, locale.Culture);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new Locale(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Code");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Code");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => new Locale(value));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Code");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Code");
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Locale_When_ToString_Then_CorrectString()
  {
    CultureInfo culture = CultureInfo.GetCultureInfo("fr-CA");
    Locale locale = new(culture);
    Assert.Equal(string.Format("{0} ({1})", culture.DisplayName, culture.Name), locale.ToString());
  }

  //[Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  //public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  //{
  //  string value = "  fr-CA  ";
  //  Locale? locale = Locale.TryCreate(value);
  //  Assert.NotNull(locale);
  //  Assert.Equal(value.Trim(), locale.Culture.Name);
  //}

  //[Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  //[InlineData(null)]
  //[InlineData("")]
  //[InlineData("  ")]
  //public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  //{
  //  Assert.Null(Locale.TryCreate(value));
  //}

  //[Fact(DisplayName = "TryCreate: it should throw ValidationException given an invalid value.")]
  //public void Given_InvalidValue_When_TryCreate_Then_ValidationException()
  //{
  //  string value = RandomStringGenerator.GetString(999);
  //  var exception = Assert.Throws<ValidationException>(() => Locale.TryCreate(value));

  //  Assert.Equal(2, exception.Errors.Count());
  //  Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Code");
  //  Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Code");
  //} // TODO(fpion): TryCreate
}
