using Krakenar.Contracts;
using Krakenar.Core.Localization;
using Logitar;

namespace Krakenar.Core.Contents;

public class ContentUniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified content unique name is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ContentTypeId
  {
    get => (Guid)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
  }
  public Guid ContentId
  {
    get => (Guid)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
  }
  public Guid? LanguageId
  {
    get => (Guid?)Data[nameof(LanguageId)];
    private set => Data[nameof(LanguageId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(ContentTypeId)] = ContentTypeId;
      error.Data[nameof(ContentId)] = ContentId;
      error.Data[nameof(LanguageId)] = LanguageId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentUniqueNameAlreadyUsedException(Content content, LanguageId? languageId, ContentId conflictId, UniqueName uniqueName)
    : base(BuildMessage(content, languageId, conflictId, uniqueName))
  {
    RealmId = content.RealmId?.ToGuid();
    ContentTypeId = content.ContentTypeId.EntityId;
    ContentId = content.EntityId;
    LanguageId = languageId?.EntityId;
    ConflictId = conflictId.EntityId;
    UniqueName = uniqueName.Value;
    PropertyName = nameof(ContentLocale.UniqueName);
  }

  private static string BuildMessage(Content content, LanguageId? languageId, ContentId conflictId, UniqueName uniqueName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), content.RealmId?.ToGuid())
    .AddData(nameof(ContentTypeId), content.ContentTypeId.EntityId)
    .AddData(nameof(ContentId), content.EntityId)
    .AddData(nameof(LanguageId), languageId?.EntityId)
    .AddData(nameof(ConflictId), conflictId.EntityId)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), nameof(ContentLocale.UniqueName))
    .Build();
}
