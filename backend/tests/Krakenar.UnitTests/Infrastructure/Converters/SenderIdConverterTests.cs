using Krakenar.Core.Realms;
using Krakenar.Core.Senders;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class SenderIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly SenderId _senderId;

  public SenderIdConverterTests()
  {
    _serializerOptions.Converters.Add(new SenderIdConverter());

    _senderId = SenderId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _senderId, '"');
    SenderId? senderId = JsonSerializer.Deserialize<SenderId?>(json, _serializerOptions);
    Assert.True(senderId.HasValue);
    Assert.Equal(_senderId, senderId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    SenderId? senderId = JsonSerializer.Deserialize<SenderId?>("null", _serializerOptions);
    Assert.Null(senderId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_senderId, _serializerOptions);
    Assert.Equal(string.Concat('"', _senderId, '"'), json);
  }
}
