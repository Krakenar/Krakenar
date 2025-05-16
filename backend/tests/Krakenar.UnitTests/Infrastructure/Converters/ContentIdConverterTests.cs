using Krakenar.Core.Contents;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class ContentIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly ContentId _contentId;

  public ContentIdConverterTests()
  {
    _serializerOptions.Converters.Add(new ContentIdConverter());

    _contentId = ContentId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _contentId, '"');
    ContentId? contentId = JsonSerializer.Deserialize<ContentId?>(json, _serializerOptions);
    Assert.True(contentId.HasValue);
    Assert.Equal(_contentId, contentId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    ContentId? contentId = JsonSerializer.Deserialize<ContentId?>("null", _serializerOptions);
    Assert.Null(contentId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_contentId, _serializerOptions);
    Assert.Equal(string.Concat('"', _contentId, '"'), json);
  }
}
