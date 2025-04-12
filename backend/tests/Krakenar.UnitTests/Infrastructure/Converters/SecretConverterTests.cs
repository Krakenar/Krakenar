using Krakenar.Core.Tokens;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class SecretConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Secret _secret = new("agwWZrQ9vEfYtncb8xXAMRJUH57m2Kzy");

  public SecretConverterTests()
  {
    _serializerOptions.Converters.Add(new SecretConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _secret, '"');
    Secret? secret = JsonSerializer.Deserialize<Secret?>(json, _serializerOptions);
    Assert.NotNull(secret);
    Assert.Equal(_secret, secret);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Secret? secret = JsonSerializer.Deserialize<Secret?>("null", _serializerOptions);
    Assert.Null(secret);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_secret, _serializerOptions);
    Assert.Equal(string.Concat('"', _secret, '"'), json);
  }
}
