using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class OneTimePasswordIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly OneTimePasswordId _oneTimePasswordId;

  public OneTimePasswordIdConverterTests()
  {
    _serializerOptions.Converters.Add(new OneTimePasswordIdConverter());

    _oneTimePasswordId = OneTimePasswordId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _oneTimePasswordId, '"');
    OneTimePasswordId? oneTimePasswordId = JsonSerializer.Deserialize<OneTimePasswordId?>(json, _serializerOptions);
    Assert.True(oneTimePasswordId.HasValue);
    Assert.Equal(_oneTimePasswordId, oneTimePasswordId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    OneTimePasswordId? oneTimePasswordId = JsonSerializer.Deserialize<OneTimePasswordId?>("null", _serializerOptions);
    Assert.Null(oneTimePasswordId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_oneTimePasswordId, _serializerOptions);
    Assert.Equal(string.Concat('"', _oneTimePasswordId, '"'), json);
  }
}
