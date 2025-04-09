using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core.Actors;

[Trait(Traits.Category, Categories.Unit)]
public class ActorExtensionsTests
{
  [Theory(DisplayName = "GetActorId: it should return the correct actor ID.")]
  [InlineData(ActorType.ApiKey, "6c1dd96e-e287-4158-9f86-5b27cdf67b85")]
  [InlineData(ActorType.User, "139670b3-1707-4bc6-a21a-8dfd376d4dc2")]
  public void Given_Actor_When_GetActorId_Then_CorrectActorId(ActorType type, string idValue)
  {
    Actor actor = new()
    {
      Type = type,
      Id = Guid.Parse(idValue)
    };
    ActorId id = actor.GetActorId();
    Assert.Equal(string.Concat(actor.Type, ':', Convert.ToBase64String(actor.Id.ToByteArray()).ToUriSafeBase64()), id.Value);
  }

  [Fact(DisplayName = "GetActorId: it should throw ArgumentException when the actor type is System.")]
  public void Given_SystemType_When_GetActorId_Then_ArgumentException()
  {
    Actor system = new();
    var exception = Assert.Throws<ArgumentException>(() => system.GetActorId());
    Assert.StartsWith("The actor type cannot be System.", exception.Message);
    Assert.Equal("actor", exception.ParamName);
  }
}
