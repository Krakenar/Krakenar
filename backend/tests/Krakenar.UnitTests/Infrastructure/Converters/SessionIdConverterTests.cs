using Krakenar.Core.Realms;
using Krakenar.Core.Sessions;

namespace Krakenar.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class SessionIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly RealmId _realmId = RealmId.NewId();
  private readonly SessionId _sessionId;

  public SessionIdConverterTests()
  {
    _serializerOptions.Converters.Add(new SessionIdConverter());

    _sessionId = SessionId.NewId(_realmId);
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _sessionId, '"');
    SessionId? sessionId = JsonSerializer.Deserialize<SessionId?>(json, _serializerOptions);
    Assert.True(sessionId.HasValue);
    Assert.Equal(_sessionId, sessionId.Value);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    SessionId? sessionId = JsonSerializer.Deserialize<SessionId?>("null", _serializerOptions);
    Assert.Null(sessionId);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_sessionId, _serializerOptions);
    Assert.Equal(string.Concat('"', _sessionId, '"'), json);
  }
}
