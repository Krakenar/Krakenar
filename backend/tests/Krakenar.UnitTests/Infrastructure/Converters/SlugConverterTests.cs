using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class SlugConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Slug _slug = new("the-new-world");

  public SlugConverterTests()
  {
    _serializerOptions.Converters.Add(new SlugConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _slug, '"');
    Slug? slug = JsonSerializer.Deserialize<Slug?>(json, _serializerOptions);
    Assert.NotNull(slug);
    Assert.Equal(_slug, slug);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Slug? slug = JsonSerializer.Deserialize<Slug?>("null", _serializerOptions);
    Assert.Null(slug);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_slug, _serializerOptions);
    Assert.Equal(string.Concat('"', _slug, '"'), json);
  }
}
