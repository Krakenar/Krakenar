using Bogus;
using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class UrlConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Url _url;

  public UrlConverterTests()
  {
    _serializerOptions.Converters.Add(new UrlConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _url = new($"https://www.{_faker.Internet.DomainName()}");
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _url, '"');
    Url? url = JsonSerializer.Deserialize<Url?>(json, _serializerOptions);
    Assert.NotNull(url);
    Assert.Equal(_url, url);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Url? url = JsonSerializer.Deserialize<Url?>("null", _serializerOptions);
    Assert.Null(url);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_url, _serializerOptions);
    Assert.Equal(string.Concat('"', _url, '"'), json);
  }
}
