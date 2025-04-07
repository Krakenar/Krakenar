using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations;

[Trait(Traits.Category, Categories.Unit)]
public class ConfigurationIdTests
{
  [Theory(DisplayName = "ctor: it should construct the correct ID from a stream ID.")]
  [InlineData(null)]
  [InlineData("ConfigurationId")]
  public void Given_StreamId_When_ctor_Then_CorrectIdConstructed(string? idValue)
  {
    StreamId streamId = idValue is null ? StreamId.NewId() : new StreamId(idValue);
    ConfigurationId id = new(streamId);
    Assert.Equal(streamId, id.StreamId);
  }

  [Fact(DisplayName = "Equals: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_Equals_Then_FalseReturned()
  {
    ConfigurationId id1 = new();
    ConfigurationId id2 = new(StreamId.NewId());
    Assert.False(id1.Equals(id2));
  }

  [Theory(DisplayName = "Equals: it should return false when the object do not have the same types.")]
  [InlineData(null)]
  [InlineData(123)]
  public void Given_DifferentTypes_When_Equals_Then_FalseReturned(object? value)
  {
    ConfigurationId id = new();
    Assert.False(id.Equals(value));
  }

  [Fact(DisplayName = "Equals: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_Equals_Then_TrueReturned()
  {
    ConfigurationId id1 = new();
    ConfigurationId id2 = new(id1.StreamId);
    Assert.True(id1.Equals(id1));
    Assert.True(id1.Equals(id2));
  }

  [Fact(DisplayName = "EqualOperator: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_EqualOperator_Then_FalseReturned()
  {
    ConfigurationId id1 = new();
    ConfigurationId id2 = new(StreamId.NewId());
    Assert.False(id1 == id2);
  }

  [Fact(DisplayName = "EqualOperator: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_EqualOperator_Then_TrueReturned()
  {
    ConfigurationId id1 = new(StreamId.NewId());
    ConfigurationId id2 = new(id1.StreamId);
    Assert.True(id1 == id2);
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_Id_When_GetHashCode_Then_CorrectHashCodeReturned()
  {
    ConfigurationId id = new();
    Assert.Equal(id.Value.GetHashCode(), id.GetHashCode());
  }

  [Fact(DisplayName = "NotEqualOperator: it should return false when the IDs are the same.")]
  public void Given_SameIds_When_NotEqualOperator_Then_TrueReturned()
  {
    ConfigurationId id1 = new();
    ConfigurationId id2 = new(id1.StreamId);
    Assert.False(id1 != id2);
  }

  [Fact(DisplayName = "NotEqualOperator: it should return true when the IDs are different.")]
  public void Given_DifferentIds_When_NotEqualOperator_Then_TrueReturned()
  {
    ConfigurationId id1 = new();
    ConfigurationId id2 = new(StreamId.NewId());
    Assert.True(id1 != id2);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Id_When_ToString_Then_CorrectStringReturned()
  {
    ConfigurationId id = new();
    Assert.Equal(id.Value, id.ToString());
  }
}
