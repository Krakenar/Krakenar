using Krakenar.Core.Messages;
using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class MessageIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly MessageId _messageId;

  public MessageIdConverterTests()
  {
    _serializerOptions.Converters.Add(new MessageIdConverter());

    _messageId = MessageId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _messageId, '"');
    MessageId? messageId = JsonSerializer.Deserialize<MessageId?>(json, _serializerOptions);
    Assert.True(messageId.HasValue);
    Assert.Equal(_messageId, messageId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    MessageId? messageId = JsonSerializer.Deserialize<MessageId?>("null", _serializerOptions);
    Assert.Null(messageId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_messageId, _serializerOptions);
    Assert.Equal(string.Concat('"', _messageId, '"'), json);
  }
}
