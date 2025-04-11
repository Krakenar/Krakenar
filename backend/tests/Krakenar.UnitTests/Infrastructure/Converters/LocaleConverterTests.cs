using Krakenar.Core.Localization;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class LocaleConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Locale _locale = new("fr-CA");

  public LocaleConverterTests()
  {
    _serializerOptions.Converters.Add(new LocaleConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _locale.Code, '"');
    Locale? locale = JsonSerializer.Deserialize<Locale?>(json, _serializerOptions);
    Assert.NotNull(locale);
    Assert.Equal(_locale, locale);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Locale? locale = JsonSerializer.Deserialize<Locale?>("null", _serializerOptions);
    Assert.Null(locale);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_locale, _serializerOptions);
    Assert.Equal(string.Concat('"', _locale.Code, '"'), json);
  }
}
