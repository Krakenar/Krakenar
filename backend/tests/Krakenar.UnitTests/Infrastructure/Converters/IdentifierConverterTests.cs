using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class IdentifierConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Identifier _identifier;

  public IdentifierConverterTests()
  {
    _serializerOptions.Converters.Add(new IdentifierConverter());

    _identifier = new("HealthInsuranceNumber");
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _identifier, '"');
    Identifier? identifier = JsonSerializer.Deserialize<Identifier?>(json, _serializerOptions);
    Assert.NotNull(identifier);
    Assert.Equal(_identifier, identifier);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Identifier? identifier = JsonSerializer.Deserialize<Identifier?>("null", _serializerOptions);
    Assert.Null(identifier);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_identifier, _serializerOptions);
    Assert.Equal(string.Concat('"', _identifier, '"'), json);
  }
}
