using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeIdTests
{
  [Theory(DisplayName = "ctor: it should construct the correct ID from a stream ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_StreamId_When_ctor_Then_CorrectIdConstructed(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();
    StreamId streamId = IdHelper.Construct("ContentType", entityId, realmId);

    ContentTypeId id = new(streamId);

    Assert.Equal(realmId, id.RealmId);
    Assert.Equal(entityId, id.EntityId);
  }

  [Theory(DisplayName = "ctor: it should construct the correct ID from a string.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_String_When_ctor_Then_CorrectIdConstructed(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();
    string value = IdHelper.Construct("ContentType", entityId, realmId).Value;

    ContentTypeId id = new(value);

    Assert.Equal(realmId, id.RealmId);
    Assert.Equal(entityId, id.EntityId);
  }

  [Theory(DisplayName = "ctor: it should construct the correct ID from a realm ID and an entity ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_RealmAndEntityId_When_ctor_Then_CorrectIdConstructed(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    ContentTypeId id = new(entityId, realmId);

    Assert.Equal(realmId, id.RealmId);
    Assert.Equal(entityId, id.EntityId);
  }

  [Fact(DisplayName = "Equals: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_Equals_Then_FalseReturned()
  {
    ContentTypeId id1 = new(Guid.NewGuid(), RealmId.NewId());
    ContentTypeId id2 = new(id1.EntityId);
    Assert.False(id1.Equals(id2));
  }

  [Theory(DisplayName = "Equals: it should return false when the object do not have the same types.")]
  [InlineData(null)]
  [InlineData(123)]
  public void Given_DifferentTypes_When_Equals_Then_FalseReturned(object? value)
  {
    ContentTypeId id = ContentTypeId.NewId(realmId: null);
    Assert.False(id.Equals(value));
  }

  [Fact(DisplayName = "Equals: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_Equals_Then_TrueReturned()
  {
    ContentTypeId id1 = new(Guid.NewGuid(), RealmId.NewId());
    ContentTypeId id2 = new(id1.StreamId);
    Assert.True(id1.Equals(id1));
    Assert.True(id1.Equals(id2));
  }

  [Fact(DisplayName = "EqualOperator: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_EqualOperator_Then_FalseReturned()
  {
    ContentTypeId id1 = new(Guid.NewGuid(), RealmId.NewId());
    ContentTypeId id2 = new(id1.EntityId);
    Assert.False(id1 == id2);
  }

  [Fact(DisplayName = "EqualOperator: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_EqualOperator_Then_TrueReturned()
  {
    ContentTypeId id1 = new(Guid.NewGuid(), RealmId.NewId());
    ContentTypeId id2 = new(id1.StreamId);
    Assert.True(id1 == id2);
  }

  [Theory(DisplayName = "NewId: it should generate a new random ID with or without a realm ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_RealmId_When_NewId_Then_NewRandomIdGenerated(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(realmIdValue);

    ContentTypeId id = ContentTypeId.NewId(realmId);

    Assert.Equal(realmId, id.RealmId);
    Assert.NotEqual(Guid.Empty, id.EntityId);
  }

  [Theory(DisplayName = "GetHashCode: it should return the correct hash code.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_Id_When_GetHashCode_Then_CorrectHashCodeReturned(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    ContentTypeId id = new(entityId, realmId);

    Assert.Equal(id.Value.GetHashCode(), id.GetHashCode());
  }

  [Fact(DisplayName = "NotEqualOperator: it should return false when the IDs are the same.")]
  public void Given_SameIds_When_NotEqualOperator_Then_TrueReturned()
  {
    ContentTypeId id1 = new(Guid.NewGuid(), RealmId.NewId());
    ContentTypeId id2 = new(id1.StreamId);
    Assert.False(id1 != id2);
  }

  [Fact(DisplayName = "NotEqualOperator: it should return true when the IDs are different.")]
  public void Given_DifferentIds_When_NotEqualOperator_Then_TrueReturned()
  {
    ContentTypeId id1 = new(Guid.NewGuid(), RealmId.NewId());
    ContentTypeId id2 = new(id1.EntityId);
    Assert.True(id1 != id2);
  }

  [Fact(DisplayName = "SwitchFields: it should throw ArgumentException when the source field could not be found.")]
  public void Given_SourceNotFound_When_SwitchFields_Then_ArgumentException()
  {
    UniqueNameSettings uniqueNameSettings = new();
    FieldType boolean = new(new UniqueName(uniqueNameSettings, "Boolean"), new BooleanSettings());
    FieldType priceType = new(new UniqueName(uniqueNameSettings, "Price"), new NumberSettings(minimumValue: 0.01, maximumValue: 999999.99, step: 0.01));
    FieldType skuType = new(new UniqueName(uniqueNameSettings, "StockKeepingUnit"), new StringSettings(minimumLength: 3, maximumLength: 32, pattern: null));

    ContentType product = new(new Identifier("Product"));

    FieldDefinition isFeatured = new(Guid.NewGuid(), boolean.Id, true, false, true, false, new Identifier("IsFeatured"), null, null, null);
    product.SetField(isFeatured);

    FieldDefinition price = new(Guid.NewGuid(), priceType.Id, true, true, true, false, new Identifier("Price"), null, null, null);
    product.SetField(price);

    FieldDefinition sku = new(Guid.NewGuid(), skuType.Id, true, true, true, true, new Identifier("Sku"), null, null, null);
    product.SetField(sku);

    Guid sourceId = Guid.NewGuid();
    var exception = Assert.Throws<ArgumentException>(() => product.SwitchFields(sourceId, sku.Id));
    Assert.StartsWith($"The field 'Id={sourceId}' was not found on content type '{product}'.", exception.Message);
    Assert.Equal("sourceId", exception.ParamName);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_Id_When_ToString_Then_CorrectStringReturned(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    ContentTypeId id = new(entityId, realmId);

    Assert.Equal(id.Value, id.ToString());
  }
}
