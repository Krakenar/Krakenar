using Krakenar.Core.Configurations;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class ConfigurationIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly ConfigurationId _configurationId = new();

  public ConfigurationIdConverterTests()
  {
    _serializerOptions.Converters.Add(new ConfigurationIdConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _configurationId, '"');
    ConfigurationId? configurationId = JsonSerializer.Deserialize<ConfigurationId?>(json, _serializerOptions);
    Assert.True(configurationId.HasValue);
    Assert.Equal(_configurationId, configurationId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    ConfigurationId? configurationId = JsonSerializer.Deserialize<ConfigurationId?>("null", _serializerOptions);
    Assert.Null(configurationId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_configurationId, _serializerOptions);
    Assert.Equal(string.Concat('"', _configurationId, '"'), json);
  }
}
