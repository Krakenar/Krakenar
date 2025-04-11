using Bogus;
using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class DescriptionConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Description _description;

  public DescriptionConverterTests()
  {
    _serializerOptions.Converters.Add(new DescriptionConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _description = new(_faker.Lorem.Paragraph(1));
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _description, '"');
    Description? description = JsonSerializer.Deserialize<Description?>(json, _serializerOptions);
    Assert.NotNull(description);
    Assert.Equal(_description, description);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Description? description = JsonSerializer.Deserialize<Description?>("null", _serializerOptions);
    Assert.Null(description);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_description, _serializerOptions);
    Assert.Equal(string.Concat('"', _description, '"'), json);
  }
}
