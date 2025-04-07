using Logitar.EventSourcing;

namespace Krakenar.Core.Realms;

[Trait(Traits.Category, Categories.Unit)]
public class RealmIdTests
{
  [Theory(DisplayName = "ctor: it should construct the correct ID from a stream ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_StreamId_When_ctor_Then_CorrectIdConstructed(string? idValue)
  {
    StreamId streamId = idValue == null ? StreamId.NewId() : new StreamId(idValue);
    RealmId id = new(streamId);
    Assert.Equal(streamId, id.StreamId);
  }

  [Fact(DisplayName = "Equals: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_Equals_Then_FalseReturned()
  {
    RealmId id1 = RealmId.NewId();
    RealmId id2 = RealmId.NewId();
    Assert.False(id1.Equals(id2));
  }

  [Theory(DisplayName = "Equals: it should return false when the object do not have the same types.")]
  [InlineData(null)]
  [InlineData(123)]
  public void Given_DifferentTypes_When_Equals_Then_FalseReturned(object? value)
  {
    RealmId id = RealmId.NewId();
    Assert.False(id.Equals(value));
  }

  [Fact(DisplayName = "Equals: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_Equals_Then_TrueReturned()
  {
    RealmId id1 = RealmId.NewId();
    RealmId id2 = new(id1.StreamId);
    Assert.True(id1.Equals(id1));
    Assert.True(id1.Equals(id2));
  }

  [Fact(DisplayName = "EqualOperator: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_EqualOperator_Then_FalseReturned()
  {
    RealmId id1 = RealmId.NewId();
    RealmId id2 = RealmId.NewId();
    Assert.False(id1 == id2);
  }

  [Fact(DisplayName = "EqualOperator: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_EqualOperator_Then_TrueReturned()
  {
    RealmId id1 = RealmId.NewId();
    RealmId id2 = new(id1.StreamId);
    Assert.True(id1 == id2);
  }

  [Fact(DisplayName = "NewId: it should generate a new random ID.")]
  public void Given_RealmId_When_NewId_Then_NewRandomIdGenerated()
  {
    RealmId id = RealmId.NewId();
    Assert.NotEqual(Guid.Empty, id.ToGuid());
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_Id_When_GetHashCode_Then_CorrectHashCodeReturned()
  {
    RealmId id = RealmId.NewId();
    Assert.Equal(id.Value.GetHashCode(), id.GetHashCode());
  }

  [Fact(DisplayName = "NotEqualOperator: it should return false when the IDs are the same.")]
  public void Given_SameIds_When_NotEqualOperator_Then_TrueReturned()
  {
    RealmId id1 = RealmId.NewId();
    RealmId id2 = new(id1.StreamId);
    Assert.False(id1 != id2);
  }

  [Fact(DisplayName = "NotEqualOperator: it should return true when the IDs are different.")]
  public void Given_DifferentIds_When_NotEqualOperator_Then_TrueReturned()
  {
    RealmId id1 = RealmId.NewId();
    RealmId id2 = RealmId.NewId();
    Assert.True(id1 != id2);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Id_When_ToString_Then_CorrectStringReturned()
  {
    RealmId id = RealmId.NewId();
    Assert.Equal(id.Value, id.ToString());
  }
}
