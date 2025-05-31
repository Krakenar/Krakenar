using Krakenar.Core.Contents.Events;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeTests
{
  [Fact(DisplayName = "SwitchFields: it should not do anything when the field IDs are the same.")]
  public void Given_SameFieldIds_When_SwitchFields_Then_DoNothing()
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

    product.ClearChanges();
    product.SwitchFields(price, price);
    Assert.False(product.HasChanges);
    Assert.Empty(product.Changes);
  }

  [Fact(DisplayName = "SwitchFields: it should switch the two field definition orders.")]
  public void Given_TwoFieldDefinitions_When_SwitchFields_Then_OrderSwitched()
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

    ActorId actorId = ActorId.NewId();
    product.SwitchFields(isFeatured, sku, actorId);
    Assert.Contains(product.Changes, change => change is ContentTypeFieldSwitched switched && switched.ActorId == actorId
      && switched.SourceId == isFeatured.Id && switched.DestinationId == sku.Id);

    Assert.Equal(sku.Id, product.Fields.ElementAt(0).Id);
    Assert.Equal(price.Id, product.Fields.ElementAt(1).Id);
    Assert.Equal(isFeatured.Id, product.Fields.ElementAt(2).Id);

    Assert.Same(sku, product.FindField(sku.Id));
    Assert.Same(price, product.FindField(price.Id));
    Assert.Same(isFeatured, product.FindField(isFeatured.Id));

    Assert.Same(sku, product.FindField(sku.UniqueName));
    Assert.Same(price, product.FindField(price.UniqueName));
    Assert.Same(isFeatured, product.FindField(isFeatured.UniqueName));
  }

  [Fact(DisplayName = "SwitchFields: it should throw ArgumentException when the destination field could not be found.")]
  public void Given_DestinationNotFound_When_SwitchFields_Then_ArgumentException()
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

    Guid destinationId = Guid.NewGuid();
    var exception = Assert.Throws<ArgumentException>(() => product.SwitchFields(isFeatured.Id, destinationId));
    Assert.StartsWith($"The field 'Id={destinationId}' was not found on content type '{product}'.", exception.Message);
    Assert.Equal("destinationId", exception.ParamName);
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
}
