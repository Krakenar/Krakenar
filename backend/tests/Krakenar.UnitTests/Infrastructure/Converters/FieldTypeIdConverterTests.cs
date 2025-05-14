using Krakenar.Core.Fields;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class FieldTypeIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly FieldTypeId _fieldTypeId;

  public FieldTypeIdConverterTests()
  {
    _serializerOptions.Converters.Add(new FieldTypeIdConverter());

    _fieldTypeId = FieldTypeId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _fieldTypeId, '"');
    FieldTypeId? fieldTypeId = JsonSerializer.Deserialize<FieldTypeId?>(json, _serializerOptions);
    Assert.True(fieldTypeId.HasValue);
    Assert.Equal(_fieldTypeId, fieldTypeId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    FieldTypeId? fieldTypeId = JsonSerializer.Deserialize<FieldTypeId?>("null", _serializerOptions);
    Assert.Null(fieldTypeId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_fieldTypeId, _serializerOptions);
    Assert.Equal(string.Concat('"', _fieldTypeId, '"'), json);
  }
}
