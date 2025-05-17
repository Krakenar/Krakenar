using Bogus;
using Krakenar.Core.Fields;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class FieldValueConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly FieldValue _fieldValue;

  public FieldValueConverterTests()
  {
    _serializerOptions.Converters.Add(new FieldValueConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _fieldValue = new(_faker.Person.FullName);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _fieldValue, '"');
    FieldValue? fieldValue = JsonSerializer.Deserialize<FieldValue?>(json, _serializerOptions);
    Assert.NotNull(fieldValue);
    Assert.Equal(_fieldValue, fieldValue);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    FieldValue? fieldValue = JsonSerializer.Deserialize<FieldValue?>("null", _serializerOptions);
    Assert.Null(fieldValue);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_fieldValue, _serializerOptions);
    Assert.Equal(string.Concat('"', _fieldValue, '"'), json);
  }
}
