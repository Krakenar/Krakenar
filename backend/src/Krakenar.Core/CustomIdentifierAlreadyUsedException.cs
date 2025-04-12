using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Logitar;

namespace Krakenar.Core;

public class CustomIdentifierAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified custom identifier is already used.";

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
  public string Key
  {
    get => (string)Data[nameof(Key)]!;
    private set => Data[nameof(Key)] = value;
  }
  public string Value
  {
    get => (string)Data[nameof(Value)]!;
    private set => Data[nameof(Value)] = value;
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
      error.Data[nameof(Key)] = Key;
      error.Data[nameof(Value)] = Value;
      return error;
    }
  }

  public CustomIdentifierAlreadyUsedException(User user, Identifier key, CustomIdentifier value, UserId conflictId)
    : this(user.RealmId, "User", user.EntityId, conflictId.EntityId, key, value, nameof(user.UniqueName))
  {
  }
  private CustomIdentifierAlreadyUsedException(RealmId? realmId, string entityType, Guid entityId, Guid conflictId, Identifier key, CustomIdentifier value, string propertyName)
    : base(BuildMessage(realmId, entityType, entityId, conflictId, key, value, propertyName))
  {
    RealmId = realmId?.ToGuid();
    EntityType = entityType;
    EntityId = entityId;
    ConflictId = conflictId;
    Key = key.Value;
    Value = value.Value;
  }

  private static string BuildMessage(RealmId? realmId, string entityType, Guid entityId, Guid conflictId, Identifier key, CustomIdentifier value, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(EntityType), entityType)
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(Key), key)
    .AddData(nameof(Value), value)
    .Build();
}
