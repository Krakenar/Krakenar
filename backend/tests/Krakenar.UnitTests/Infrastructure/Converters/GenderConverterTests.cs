using Bogus;
using Krakenar.Core.Users;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class GenderConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Gender _gender;

  public GenderConverterTests()
  {
    _serializerOptions.Converters.Add(new GenderConverter());

    _gender = new(_faker.Person.Gender.ToString());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _gender, '"');
    Gender? gender = JsonSerializer.Deserialize<Gender?>(json, _serializerOptions);
    Assert.NotNull(gender);
    Assert.Equal(_gender, gender);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Gender? gender = JsonSerializer.Deserialize<Gender?>("null", _serializerOptions);
    Assert.Null(gender);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_gender, _serializerOptions);
    Assert.Equal(string.Concat('"', _gender, '"'), json);
  }
}
