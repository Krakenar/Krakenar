using Krakenar.Core.Fields;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class PlaceholderConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Placeholder _placeholder;

  public PlaceholderConverterTests()
  {
    _serializerOptions.Converters.Add(new PlaceholderConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _placeholder = new("Enter your name.");
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _placeholder, '"');
    Placeholder? placeholder = JsonSerializer.Deserialize<Placeholder?>(json, _serializerOptions);
    Assert.NotNull(placeholder);
    Assert.Equal(_placeholder, placeholder);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Placeholder? placeholder = JsonSerializer.Deserialize<Placeholder?>("null", _serializerOptions);
    Assert.Null(placeholder);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_placeholder, _serializerOptions);
    Assert.Equal(string.Concat('"', _placeholder, '"'), json);
  }
}
