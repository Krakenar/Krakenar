using Bogus;
using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class DisplayNameConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly DisplayName _displayName;

  public DisplayNameConverterTests()
  {
    _serializerOptions.Converters.Add(new DisplayNameConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _displayName = new(_faker.Person.FullName);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _displayName, '"');
    DisplayName? displayName = JsonSerializer.Deserialize<DisplayName?>(json, _serializerOptions);
    Assert.NotNull(displayName);
    Assert.Equal(_displayName, displayName);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    DisplayName? displayName = JsonSerializer.Deserialize<DisplayName?>("null", _serializerOptions);
    Assert.Null(displayName);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_displayName, _serializerOptions);
    Assert.Equal(string.Concat('"', _displayName, '"'), json);
  }
}
