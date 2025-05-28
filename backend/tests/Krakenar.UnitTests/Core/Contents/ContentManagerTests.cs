using Krakenar.Contracts;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Fields.Validators;
using Krakenar.Core.Localization;
using Krakenar.Core.Settings;
using Moq;

namespace Krakenar.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly FieldValueValidatorFactory _fieldValueValidatorFactory;

  private readonly ContentManager _manager;

  public ContentManagerTests()
  {
    _fieldValueValidatorFactory = new FieldValueValidatorFactory(_contentQuerier.Object);

    _manager = new(
      _applicationContext.Object,
      _contentQuerier.Object,
      _contentRepository.Object,
      _contentTypeQuerier.Object,
      _contentTypeRepository.Object,
      _fieldTypeRepository.Object,
      _fieldValueValidatorFactory);
  }

  [Fact(DisplayName = "It should throw InvalidFieldValuesException when publishing contents with missing fields.")]
  public async Task Given_MissingFields_When_PublishingContent_Then_InvalidFieldValuesException()
  {
    Language language = new(new Locale("en"), isDefault: true);

    UniqueNameSettings uniqueNameSettings = new();
    FieldType boolean = new(new UniqueName(uniqueNameSettings, "Boolean"), new BooleanSettings());
    FieldType keywordsType = new(new UniqueName(uniqueNameSettings, "Keywords"), new TagsSettings());

    ContentType contentType = new(new Identifier("Product"));
    FieldDefinition isFeatured = new(Guid.NewGuid(), boolean.Id, true, true, true, false, new Identifier("IsFeatured"), null, null, null);
    contentType.SetField(isFeatured);
    FieldDefinition keywords = new(Guid.NewGuid(), keywordsType.Id, false, true, false, false, new Identifier("Keywords"), null, null, null);
    contentType.SetField(keywords);

    ContentLocale locale = new(new UniqueName(uniqueNameSettings, "shure-sm57"));
    Content content = new(contentType, locale);
    content.SetLocale(language, locale);

    content.ClearChanges();
    content.PublishLocale(language);

    _fieldTypeRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(new FieldTypeId[] { boolean.Id, keywordsType.Id })),
      _cancellationToken)).ReturnsAsync([boolean, keywordsType]);

    var exception = await Assert.ThrowsAsync<InvalidFieldValuesException>(async () => await _manager.SaveAsync(content, contentType, _cancellationToken));
    Assert.Equal(content.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(content.EntityId, exception.ContentId);
    Assert.Equal(language.EntityId, exception.LanguageId);
    Assert.Equal("FieldValues", exception.PropertyName);

    Error error = Assert.Single(exception.Errors);
    Assert.Equal("RequiredFieldValidator", error.Code);
    Assert.Equal("The specified field is missing.", error.Message);
    Assert.Equal(2, error.Data.Count);
    Assert.Contains(error.Data, d => d.Key == "Id" && d.Value is Guid id && id == keywords.Id);
    Assert.Contains(error.Data, d => d.Key == "Name" && d.Value is string name && name == keywords.UniqueName.Value);

    _contentTypeRepository.Verify(x => x.LoadAsync(It.IsAny<ContentTypeId>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentQuerier.Verify(x => x.FindConflictsAsync(
      It.IsAny<ContentTypeId>(),
      It.IsAny<LanguageId?>(),
      It.IsAny<ContentStatus>(),
      It.IsAny<IReadOnlyDictionary<Guid, FieldValue>>(),
      It.IsAny<ContentId>(),
      It.IsAny<CancellationToken>()), Times.Never);
    _contentRepository.Verify(x => x.SaveAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should throw InvalidFieldValuesException when saving contents with invalid field values.")]
  public async Task Given_InvalidFieldValues_When_SavingContent_Then_InvalidFieldValuesException()
  {
    UniqueNameSettings uniqueNameSettings = new();
    NumberSettings settings = new(0.01, 999999.99, 0.01);
    FieldType priceType = new(new UniqueName(uniqueNameSettings, "Price"), settings);

    ContentType contentType = new(new Identifier("Product"));
    FieldDefinition price = new(Guid.NewGuid(), priceType.Id, true, true, true, false, new Identifier("Price"), null, null, null);
    contentType.SetField(price);

    FieldValue fieldValue = new("-139");
    ContentLocale invariant = new(new UniqueName(uniqueNameSettings, "shure-sm57"), null, null, new Dictionary<Guid, FieldValue>
    {
      [price.Id] = fieldValue
    });
    Content content = new(contentType, invariant);

    _fieldTypeRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(new FieldTypeId[] { priceType.Id })),
      _cancellationToken)).ReturnsAsync([priceType]);

    var exception = await Assert.ThrowsAsync<InvalidFieldValuesException>(async () => await _manager.SaveAsync(content, contentType, _cancellationToken));
    Assert.Equal(content.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(content.EntityId, exception.ContentId);
    Assert.Null(exception.LanguageId);
    Assert.Equal("FieldValues", exception.PropertyName);

    Error error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanOrEqualValidator", error.Code);
    Assert.Equal("The value must be greater than or equal to 0.01.", error.Message);
    Assert.Equal(4, error.Data.Count);
    Assert.Contains(error.Data, d => d.Key == "Id" && d.Value is Guid id && id == price.Id);
    Assert.Contains(error.Data, d => d.Key == "Name" && d.Value is string name && name == price.UniqueName.Value);
    Assert.Contains(error.Data, d => d.Key == "Value" && d.Value is string value && value == fieldValue.Value);
    Assert.Contains(error.Data, d => d.Key == "MinimumValue" && d.Value is double minimumValue && minimumValue == settings.MinimumValue);

    _contentTypeRepository.Verify(x => x.LoadAsync(It.IsAny<ContentTypeId>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentQuerier.Verify(x => x.FindConflictsAsync(
      It.IsAny<ContentTypeId>(),
      It.IsAny<LanguageId?>(),
      It.IsAny<ContentStatus>(),
      It.IsAny<IReadOnlyDictionary<Guid, FieldValue>>(),
      It.IsAny<ContentId>(),
      It.IsAny<CancellationToken>()), Times.Never);
    _contentRepository.Verify(x => x.SaveAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should throw InvalidFieldValuesException when saving contents with invalid field variancy.")]
  public async Task Given_InvalidFieldVariancy_When_SavingContent_Then_InvalidFieldValuesException()
  {
    UniqueNameSettings uniqueNameSettings = new();
    FieldType keywordsType = new(new UniqueName(uniqueNameSettings, "Keywords"), new TagsSettings());

    ContentType contentType = new(new Identifier("Product"));
    FieldDefinition keywords = new(Guid.NewGuid(), keywordsType.Id, IsInvariant: false, true, false, false, new Identifier("Keywords"), null, null, null);
    contentType.SetField(keywords);

    FieldValue fieldValue = new(@"[""dynamic"",""mic""]");
    ContentLocale invariant = new(new UniqueName(uniqueNameSettings, "shure-sm57"), null, null, new Dictionary<Guid, FieldValue>
    {
      [keywords.Id] = fieldValue
    });
    Content content = new(contentType, invariant);

    _fieldTypeRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(new FieldTypeId[] { keywordsType.Id })),
      _cancellationToken)).ReturnsAsync([keywordsType]);

    var exception = await Assert.ThrowsAsync<InvalidFieldValuesException>(async () => await _manager.SaveAsync(content, contentType, _cancellationToken));
    Assert.Equal(content.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(content.EntityId, exception.ContentId);
    Assert.Null(exception.LanguageId);
    Assert.Equal("FieldValues", exception.PropertyName);

    Error error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantFieldValidator", error.Code);
    Assert.Equal("The field is defined as localized, but saved in an invariant content.", error.Message);
    Assert.Equal(3, error.Data.Count);
    Assert.Contains(error.Data, d => d.Key == "Id" && d.Value is Guid id && id == keywords.Id);
    Assert.Contains(error.Data, d => d.Key == "Name" && d.Value is string name && name == keywords.UniqueName.Value);
    Assert.Contains(error.Data, d => d.Key == "Value" && d.Value is string value && value == fieldValue.Value);

    _contentTypeRepository.Verify(x => x.LoadAsync(It.IsAny<ContentTypeId>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentQuerier.Verify(x => x.FindConflictsAsync(
      It.IsAny<ContentTypeId>(),
      It.IsAny<LanguageId?>(),
      It.IsAny<ContentStatus>(),
      It.IsAny<IReadOnlyDictionary<Guid, FieldValue>>(),
      It.IsAny<ContentId>(),
      It.IsAny<CancellationToken>()), Times.Never);
    _contentRepository.Verify(x => x.SaveAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should throw InvalidFieldValuesException when saving contents with unexpected fields.")]
  public async Task Given_UnexpectedFields_When_SavingContent_Then_InvalidFieldValuesException()
  {
    ContentType contentType = new(new Identifier("Product"));

    Guid unexpectedId = Guid.Empty;
    FieldValue fieldValue = new("Unexpected!");
    ContentLocale invariant = new(new UniqueName(new UniqueNameSettings(), "shure-sm57"), null, null, new Dictionary<Guid, FieldValue>
    {
      [unexpectedId] = fieldValue
    });
    Content content = new(contentType, invariant);

    var exception = await Assert.ThrowsAsync<InvalidFieldValuesException>(async () => await _manager.SaveAsync(content, contentType, _cancellationToken));
    Assert.Equal(content.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(content.EntityId, exception.ContentId);
    Assert.Null(exception.LanguageId);
    Assert.Equal("FieldValues", exception.PropertyName);

    Error error = Assert.Single(exception.Errors);
    Assert.Equal("FieldDefinitionValidator", error.Code);
    Assert.Equal("The field is not defined on content type 'Product'.", error.Message);
    Assert.Equal(2, error.Data.Count);
    Assert.Contains(error.Data, d => d.Key == "Id" && d.Value is Guid id && id == unexpectedId);
    Assert.Contains(error.Data, d => d.Key == "Value" && d.Value is string value && value == fieldValue.Value);

    _fieldTypeRepository.Verify(x => x.LoadAsync(It.IsAny<IEnumerable<FieldTypeId>>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentTypeRepository.Verify(x => x.LoadAsync(It.IsAny<ContentTypeId>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentQuerier.Verify(x => x.FindConflictsAsync(
      It.IsAny<ContentTypeId>(),
      It.IsAny<LanguageId?>(),
      It.IsAny<ContentStatus>(),
      It.IsAny<IReadOnlyDictionary<Guid, FieldValue>>(),
      It.IsAny<ContentId>(),
      It.IsAny<CancellationToken>()), Times.Never);
    _contentRepository.Verify(x => x.SaveAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
