using Krakenar.Core.Realms;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class IdHelperTests
{
  [Theory(DisplayName = "Construct: it should construct the correct stream identifier from components.")]
  [InlineData(null)]
  [InlineData("34b7a8f4-540b-46cb-849e-eb19605e1a17")]
  public void Given_Components_When_Construct_Then_StreamId(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    string entityType = "User";
    Guid entityId = Guid.NewGuid();

    StreamId streamId = IdHelper.Construct(entityType, entityId, realmId);
    string[] components = streamId.Value.Split('|');
    string entity;
    if (realmId.HasValue)
    {
      Assert.Equal(2, components.Length);
      Assert.Equal(realmId.Value.Value, components.First());
      entity = components.Last();
    }
    else
    {
      entity = Assert.Single(components);
    }
    string[] parts = entity.Split(':');
    Assert.Equal(2, parts.Length);
    Assert.Equal(entityType, parts.First());
    Assert.Equal(Convert.ToBase64String(entityId.ToByteArray()).ToUriSafeBase64(), parts.Last());
  }

  [Theory(DisplayName = "Deconstruct: it should deconstruct the stream identifier into components.")]
  [InlineData(null)]
  [InlineData("a90d67b8-3b0f-48b8-a15d-d2007a5302e6")]
  public void Given_StreamId_When_Deconstruct_Then_Components(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    string entityType = "User";
    Guid entityId = Guid.NewGuid();
    string entity = $"{entityType}:{Convert.ToBase64String(entityId.ToByteArray()).ToUriSafeBase64()}";
    StreamId streamId = new(realmId.HasValue ? $"{realmId}|{entity}" : entity);

    Tuple<Guid, RealmId?> components = IdHelper.Deconstruct(streamId, entityType);
    Assert.Equal(entityId, components.Item1);
    if (realmId.HasValue)
    {
      Assert.NotNull(components.Item2);
      Assert.Equal(realmId.Value, components.Item2);
    }
    else
    {
      Assert.Null(components.Item2);
    }
  }

  [Fact(DisplayName = "Deconstruct: it should throw ArgumentException when the entity is not valid.")]
  public void Given_InvalidEntity_When_Deconstruct_Then_ArgumentException()
  {
    StreamId streamId = new("invalid");
    var exception = Assert.Throws<ArgumentException>(() => IdHelper.Deconstruct(streamId, "User"));
    Assert.StartsWith("The value 'invalid' is not a valid entity.", exception.Message);
    Assert.Equal("streamId", exception.ParamName);
  }

  [Fact(DisplayName = "Deconstruct: it should throw ArgumentException when the entity type is not expected.")]
  public void Given_EntityTypeNotExpected_When_Deconstruct_Then_ArgumentException()
  {
    StreamId streamId = new($"Language:{Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToUriSafeBase64()}");
    var exception = Assert.Throws<ArgumentException>(() => IdHelper.Deconstruct(streamId, "User"));
    Assert.StartsWith("The entity type 'Language' was not expected (User).", exception.Message);
    Assert.Equal("streamId", exception.ParamName);
  }

  [Fact(DisplayName = "Deconstruct: it should throw ArgumentException when the value is not a valid stream identifier.")]
  public void Given_InvalidStreamId_When_Deconstruct_Then_ArgumentException()
  {
    StreamId streamId = new("in|va|lid");
    var exception = Assert.Throws<ArgumentException>(() => IdHelper.Deconstruct(streamId, "User"));
    Assert.StartsWith($"The value '{streamId}' is not a valid aggregate identifier.", exception.Message);
    Assert.Equal("streamId", exception.ParamName);
  }
}
