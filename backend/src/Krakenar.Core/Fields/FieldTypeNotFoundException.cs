using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Fields;

public class FieldTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified field type could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string FieldType
  {
    get => (string)Data[nameof(FieldType)]!;
    private set => Data[nameof(FieldType)] = value;
  }
  public string PropertyName
  {
    get => (string?)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(FieldType)] = FieldType;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public FieldTypeNotFoundException(RealmId? realmId, string fieldType, string propertyName) : base(BuildMessage(realmId, fieldType, propertyName))
  {
    RealmId = realmId?.ToGuid();
    FieldType = fieldType;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string fieldType, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(FieldType), fieldType)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
