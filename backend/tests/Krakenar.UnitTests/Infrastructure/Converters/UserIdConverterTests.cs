using Krakenar.Core.Realms;
using Krakenar.Core.Users;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class UserIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly UserId _userId;

  public UserIdConverterTests()
  {
    _serializerOptions.Converters.Add(new UserIdConverter());

    _userId = UserId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _userId, '"');
    UserId? userId = JsonSerializer.Deserialize<UserId?>(json, _serializerOptions);
    Assert.True(userId.HasValue);
    Assert.Equal(_userId, userId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    UserId? userId = JsonSerializer.Deserialize<UserId?>("null", _serializerOptions);
    Assert.Null(userId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_userId, _serializerOptions);
    Assert.Equal(string.Concat('"', _userId, '"'), json);
  }
}
