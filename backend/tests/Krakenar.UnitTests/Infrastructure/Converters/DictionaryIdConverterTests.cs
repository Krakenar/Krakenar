using Krakenar.Core.Dictionaries;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class DictionaryIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly DictionaryId _dictionaryId;

  public DictionaryIdConverterTests()
  {
    _serializerOptions.Converters.Add(new DictionaryIdConverter());

    _dictionaryId = DictionaryId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _dictionaryId, '"');
    DictionaryId? dictionaryId = JsonSerializer.Deserialize<DictionaryId?>(json, _serializerOptions);
    Assert.True(dictionaryId.HasValue);
    Assert.Equal(_dictionaryId, dictionaryId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    DictionaryId? dictionaryId = JsonSerializer.Deserialize<DictionaryId?>("null", _serializerOptions);
    Assert.Null(dictionaryId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_dictionaryId, _serializerOptions);
    Assert.Equal(string.Concat('"', _dictionaryId, '"'), json);
  }
}
