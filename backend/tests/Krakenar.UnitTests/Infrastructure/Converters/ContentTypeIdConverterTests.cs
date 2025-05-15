using Krakenar.Core.Contents;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly ContentTypeId _contentTypeId;

  public ContentTypeIdConverterTests()
  {
    _serializerOptions.Converters.Add(new ContentTypeIdConverter());

    _contentTypeId = ContentTypeId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _contentTypeId, '"');
    ContentTypeId? contentTypeId = JsonSerializer.Deserialize<ContentTypeId?>(json, _serializerOptions);
    Assert.True(contentTypeId.HasValue);
    Assert.Equal(_contentTypeId, contentTypeId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    ContentTypeId? contentTypeId = JsonSerializer.Deserialize<ContentTypeId?>("null", _serializerOptions);
    Assert.Null(contentTypeId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_contentTypeId, _serializerOptions);
    Assert.Equal(string.Concat('"', _contentTypeId, '"'), json);
  }
}
