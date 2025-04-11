using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class RealmIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();

  public RealmIdConverterTests()
  {
    _serializerOptions.Converters.Add(new RealmIdConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _realmId, '"');
    RealmId? realmId = JsonSerializer.Deserialize<RealmId?>(json, _serializerOptions);
    Assert.True(realmId.HasValue);
    Assert.Equal(_realmId, realmId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    RealmId? realmId = JsonSerializer.Deserialize<RealmId?>("null", _serializerOptions);
    Assert.Null(realmId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_realmId, _serializerOptions);
    Assert.Equal(string.Concat('"', _realmId, '"'), json);
  }
}
