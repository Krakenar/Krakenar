using Bogus;
using Krakenar.Core.Users;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class PersonNameConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly PersonName _personName;

  public PersonNameConverterTests()
  {
    _serializerOptions.Converters.Add(new PersonNameConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    _personName = new(_faker.Person.FirstName);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _personName, '"');
    PersonName? personName = JsonSerializer.Deserialize<PersonName?>(json, _serializerOptions);
    Assert.NotNull(personName);
    Assert.Equal(_personName, personName);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    PersonName? personName = JsonSerializer.Deserialize<PersonName?>("null", _serializerOptions);
    Assert.Null(personName);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_personName, _serializerOptions);
    Assert.Equal(string.Concat('"', _personName, '"'), json);
  }
}
