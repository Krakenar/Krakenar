using Krakenar.Contracts;
using Krakenar.Core.Contents;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Fields;

public class FieldUniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified field definition unique name is already used.";

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
  public Guid FieldId
  {
    get => (Guid)Data[nameof(FieldId)]!;
    private set => Data[nameof(FieldId)] = value;
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
      error.Data[nameof(FieldId)] = FieldId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public FieldUniqueNameAlreadyUsedException(ContentType contentType, FieldDefinition field, Guid conflictId)
    : base(BuildMessage(contentType.RealmId, contentType.EntityId, field.Id, conflictId, field.UniqueName.Value, nameof(field.UniqueName)))
  {
    RealmId = contentType.RealmId?.ToGuid();
    ContentTypeId = contentType.EntityId;
    FieldId = field.Id;
    ConflictId = conflictId;
    UniqueName = field.UniqueName.Value;
    PropertyName = nameof(field.UniqueName);
  }

  private static string BuildMessage(RealmId? realmId, Guid contentTypeId, Guid fieldId, Guid conflictId, string uniqueName, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(ContentTypeId), contentTypeId)
    .AddData(nameof(FieldId), fieldId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
