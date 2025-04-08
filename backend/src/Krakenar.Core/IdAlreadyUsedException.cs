using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core;

public class IdAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified ID is already used.";

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
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public IdAlreadyUsedException(RealmId? realmId, string entityType, Guid entityId, string propertyName)
    : base(BuildMessage(realmId, entityType, entityId, propertyName))
  {
    RealmId = realmId?.ToGuid();
    EntityType = entityType;
    EntityId = entityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string entityType, Guid entityId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(EntityType), entityType)
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

public class IdAlreadyUsedException<T> : IdAlreadyUsedException
{
  public IdAlreadyUsedException(RealmId? realmId, Guid entityId, string propertyName)
    : base(realmId, typeof(T).Name, entityId, propertyName)
  {
  }
}
