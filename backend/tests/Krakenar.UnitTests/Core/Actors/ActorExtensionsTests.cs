using Bogus;
using Krakenar.Contracts.Actors;
using Krakenar.Core.Realms;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core.Actors;

[Trait(Traits.Category, Categories.Unit)]
public class ActorExtensionsTests
{
  private readonly Faker _faker = new();

  [Theory(DisplayName = "GetActorId: it should return the correct API key actor ID.")]
  [InlineData(null)]
  [InlineData("47778e41-5c1e-4971-b4fd-d98b02299c1b")]
  public void Given_ApiKey_When_GetActorId_Then_CorrectActorId(string? realmId)
  {
    Actor actor = new()
    {
      Type = ActorType.ApiKey,
      Id = Guid.NewGuid(),
      DisplayName = _faker.Person.FullName,
      EmailAddress = _faker.Person.Email,
      PictureUrl = _faker.Person.Avatar
    };
    if (realmId is not null)
    {
      actor.RealmId = Guid.Parse(realmId);
    }

    ActorId actorId = actor.GetActorId();

    StringBuilder value = new();
    if (actor.RealmId.HasValue)
    {
      value.Append(new RealmId(actor.RealmId.Value)).Append('|');
    }
    value.Append(actor.Type).Append(':').Append(Convert.ToBase64String(actor.Id.ToByteArray()).ToUriSafeBase64());
    Assert.Equal(value.ToString(), actorId.Value);
  }

  [Theory(DisplayName = "GetActorId: it should return the correct User actor ID.")]
  [InlineData(null)]
  [InlineData("f4f5b92d-7df1-47f4-b760-f35ce0c796a0")]
  public void Given_User_When_GetActorId_Then_CorrectActorId(string? realmId)
  {
    Actor actor = new()
    {
      Type = ActorType.User,
      Id = Guid.NewGuid(),
      DisplayName = _faker.Person.FullName,
      EmailAddress = _faker.Person.Email,
      PictureUrl = _faker.Person.Avatar
    };
    if (realmId is not null)
    {
      actor.RealmId = Guid.Parse(realmId);
    }

    ActorId actorId = actor.GetActorId();

    StringBuilder value = new();
    if (actor.RealmId.HasValue)
    {
      value.Append(new RealmId(actor.RealmId.Value)).Append('|');
    }
    value.Append(actor.Type).Append(':').Append(Convert.ToBase64String(actor.Id.ToByteArray()).ToUriSafeBase64());
    Assert.Equal(value.ToString(), actorId.Value);
  }

  [Fact(DisplayName = "GetActorId: it should throw ArgumentException when the actor type is not supported.")]
  public void Given_TypeNotSupported_When_GetActorId_Then_ArgumentException()
  {
    Actor actor = new()
    {
      Type = (ActorType)(-1)
    };
    var exception = Assert.Throws<ArgumentException>(() => actor.GetActorId());
    Assert.StartsWith($"The actor type cannot be {actor.Type}.", exception.Message);
    Assert.Equal("actor", exception.ParamName);
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
