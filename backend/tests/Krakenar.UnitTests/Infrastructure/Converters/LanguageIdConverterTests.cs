using Krakenar.Core.Localization;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly LanguageId _languageId;

  public LanguageIdConverterTests()
  {
    _serializerOptions.Converters.Add(new LanguageIdConverter());

    _languageId = LanguageId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _languageId, '"');
    LanguageId? languageId = JsonSerializer.Deserialize<LanguageId?>(json, _serializerOptions);
    Assert.True(languageId.HasValue);
    Assert.Equal(_languageId, languageId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    LanguageId? languageId = JsonSerializer.Deserialize<LanguageId?>("null", _serializerOptions);
    Assert.Null(languageId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_languageId, _serializerOptions);
    Assert.Equal(string.Concat('"', _languageId, '"'), json);
  }
}
