using Bogus;
using TimeZone = Krakenar.Core.Localization.TimeZone;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class TimeZoneConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly TimeZone _timeZone;

  public TimeZoneConverterTests()
  {
    _serializerOptions.Converters.Add(new TimeZoneConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _timeZone = new(_faker.Date.TimeZoneString());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _timeZone, '"');
    TimeZone? timeZone = JsonSerializer.Deserialize<TimeZone?>(json, _serializerOptions);
    Assert.NotNull(timeZone);
    Assert.Equal(_timeZone, timeZone);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    TimeZone? timeZone = JsonSerializer.Deserialize<TimeZone?>("null", _serializerOptions);
    Assert.Null(timeZone);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_timeZone, _serializerOptions);
    Assert.Equal(string.Concat('"', _timeZone, '"'), json);
  }
}
