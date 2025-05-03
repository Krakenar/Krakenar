using Krakenar.Core.Passwords;
using Moq;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class PasswordConverterTests
{
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  public PasswordConverterTests()
  {
    _serializerOptions.Converters.Add(new PasswordConverter(_passwordManager.Object));
  }

  [Fact(DisplayName = "It should deserialize the correct value from a non-null value.")]
  public void Given_Value_When_Deserialize_Then_CorrectValue()
  {
    Base64Password password = new("P@s$W0rD");
    _passwordManager.Setup(x => x.Decode(password.Encode())).Returns(password);

    Password? deserialized = JsonSerializer.Deserialize<Password>(string.Concat('"', password.Encode(), '"'), _serializerOptions);
    Assert.NotNull(deserialized);
    Assert.Same(password, deserialized);
  }

  [Fact(DisplayName = "It should deserialize null from a null value.")]
  public void Given_NullValue_When_Deserialize_Then_NullValue()
  {
    Password? password = JsonSerializer.Deserialize<Password>("null", _serializerOptions);
    Assert.Null(password);
  }

  [Fact(DisplayName = "It should serialize correctly the value.")]
  public void Given_Value_When_Serialize_Then_SerializedCorrectly()
  {
    Base64Password password = new("P@s$W0rD");
    string json = JsonSerializer.Serialize<Password>(password, _serializerOptions);
    Assert.Equal(string.Concat('"', password.Encode(), '"'), json);
  }
}
