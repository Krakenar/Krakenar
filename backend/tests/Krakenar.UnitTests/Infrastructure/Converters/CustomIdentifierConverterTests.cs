using Bogus;
using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class CustomIdentifierConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly CustomIdentifier _customIdentifier;

  public CustomIdentifierConverterTests()
  {
    _serializerOptions.Converters.Add(new CustomIdentifierConverter());

    _customIdentifier = new(_faker.Random.String(10, minChar: '0', maxChar: '9'));
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _customIdentifier, '"');
    CustomIdentifier? customIdentifier = JsonSerializer.Deserialize<CustomIdentifier?>(json, _serializerOptions);
    Assert.NotNull(customIdentifier);
    Assert.Equal(_customIdentifier, customIdentifier);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    CustomIdentifier? customIdentifier = JsonSerializer.Deserialize<CustomIdentifier?>("null", _serializerOptions);
    Assert.Null(customIdentifier);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_customIdentifier, _serializerOptions);
    Assert.Equal(string.Concat('"', _customIdentifier, '"'), json);
  }
}
