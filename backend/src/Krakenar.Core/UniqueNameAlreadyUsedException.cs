using Krakenar.Contracts;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Logitar;

namespace Krakenar.Core;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string EntityType
  {
    get => (string)Data[nameof(EntityType)]!;
    private set => Data[nameof(EntityType)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
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
      error.Data[nameof(EntityType)] = EntityType;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(ContentType contentType, ContentTypeId conflictId)
    : this(contentType.RealmId, "ContentType", contentType.EntityId, conflictId.EntityId, contentType.UniqueName.Value, nameof(contentType.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(FieldType fieldType, FieldTypeId conflictId)
    : this(fieldType.RealmId, "FieldType", fieldType.EntityId, conflictId.EntityId, fieldType.UniqueName.Value, nameof(fieldType.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Role role, RoleId conflictId)
    : this(role.RealmId, "Role", role.EntityId, conflictId.EntityId, role.UniqueName.Value, nameof(role.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Template template, TemplateId conflictId)
    : this(template.RealmId, "Template", template.EntityId, conflictId.EntityId, template.UniqueName.Value, nameof(template.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(User user, UserId conflictId)
    : this(user.RealmId, "User", user.EntityId, conflictId.EntityId, user.UniqueName.Value, nameof(user.UniqueName))
  {
  }
  private UniqueNameAlreadyUsedException(RealmId? realmId, string entityType, Guid entityId, Guid conflictId, string uniqueName, string propertyName)
    : base(BuildMessage(realmId, entityType, entityId, conflictId, uniqueName, propertyName))
  {
    RealmId = realmId?.ToGuid();
    EntityType = entityType;
    EntityId = entityId;
    ConflictId = conflictId;
    UniqueName = uniqueName;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string entityType, Guid entityId, Guid conflictId, string uniqueName, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(EntityType), entityType)
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
