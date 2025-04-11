using Bogus;
using Krakenar.Core;
using Krakenar.Core.Settings;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class UniqueNameConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly UniqueName _uniqueName;

  public UniqueNameConverterTests()
  {
    _serializerOptions.Converters.Add(new UniqueNameConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _uniqueName = new(new UniqueNameSettings(allowedCharacters: null), string.Concat(_faker.Person.UserName, "!?"));
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _uniqueName, '"');
    UniqueName? uniqueName = JsonSerializer.Deserialize<UniqueName?>(json, _serializerOptions);
    Assert.NotNull(uniqueName);
    Assert.Equal(_uniqueName, uniqueName);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    UniqueName? uniqueName = JsonSerializer.Deserialize<UniqueName?>("null", _serializerOptions);
    Assert.Null(uniqueName);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_uniqueName, _serializerOptions);
    Assert.Equal(string.Concat('"', _uniqueName, '"'), json);
  }
}
