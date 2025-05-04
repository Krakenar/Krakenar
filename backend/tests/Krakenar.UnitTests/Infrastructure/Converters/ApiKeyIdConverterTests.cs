using Krakenar.Core.ApiKeys;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class ApiKeyIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly ApiKeyId _apiKeyId;

  public ApiKeyIdConverterTests()
  {
    _serializerOptions.Converters.Add(new ApiKeyIdConverter());

    _apiKeyId = ApiKeyId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _apiKeyId, '"');
    ApiKeyId? apiKeyId = JsonSerializer.Deserialize<ApiKeyId?>(json, _serializerOptions);
    Assert.True(apiKeyId.HasValue);
    Assert.Equal(_apiKeyId, apiKeyId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    ApiKeyId? apiKeyId = JsonSerializer.Deserialize<ApiKeyId?>("null", _serializerOptions);
    Assert.Null(apiKeyId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_apiKeyId, _serializerOptions);
    Assert.Equal(string.Concat('"', _apiKeyId, '"'), json);
  }
}
