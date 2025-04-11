using Krakenar.Core.Realms;
using Krakenar.Core.Roles;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class RoleIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly RoleId _roleId;

  public RoleIdConverterTests()
  {
    _serializerOptions.Converters.Add(new RoleIdConverter());

    _roleId = RoleId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _roleId, '"');
    RoleId? roleId = JsonSerializer.Deserialize<RoleId?>(json, _serializerOptions);
    Assert.True(roleId.HasValue);
    Assert.Equal(_roleId, roleId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    RoleId? roleId = JsonSerializer.Deserialize<RoleId?>("null", _serializerOptions);
    Assert.Null(roleId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_roleId, _serializerOptions);
    Assert.Equal(string.Concat('"', _roleId, '"'), json);
  }
}
