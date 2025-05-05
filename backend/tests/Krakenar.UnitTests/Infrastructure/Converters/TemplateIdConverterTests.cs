using Krakenar.Core.Realms;
using Krakenar.Core.Templates;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly TemplateId _templateId;

  public TemplateIdConverterTests()
  {
    _serializerOptions.Converters.Add(new TemplateIdConverter());

    _templateId = TemplateId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _templateId, '"');
    TemplateId? templateId = JsonSerializer.Deserialize<TemplateId?>(json, _serializerOptions);
    Assert.True(templateId.HasValue);
    Assert.Equal(_templateId, templateId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    TemplateId? templateId = JsonSerializer.Deserialize<TemplateId?>("null", _serializerOptions);
    Assert.Null(templateId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_templateId, _serializerOptions);
    Assert.Equal(string.Concat('"', _templateId, '"'), json);
  }
}
