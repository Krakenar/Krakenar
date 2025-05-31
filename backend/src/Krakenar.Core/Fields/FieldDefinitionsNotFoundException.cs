using Krakenar.Contracts;
using Krakenar.Core.Contents;
using Logitar;

namespace Krakenar.Core.Fields;

public class FieldDefinitionsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified fields were not defined on the content type.";

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
  public IReadOnlyCollection<string> Fields
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Fields)]!;
    private set => Data[nameof(Fields)] = value;
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
      error.Data[nameof(Fields)] = Fields;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public FieldDefinitionsNotFoundException(ContentType contentType, IEnumerable<string> fields, string propertyName)
    : base(BuildMessage(contentType, fields, propertyName))
  {
    RealmId = contentType.RealmId?.ToGuid();
    ContentTypeId = contentType.EntityId;
    Fields = fields.ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentType contentType, IEnumerable<string> fields, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(RealmId)).Append(": ").AppendLine(contentType.RealmId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(ContentTypeId)).Append(": ").Append(contentType.EntityId).AppendLine();
    message.Append(nameof(ContentTypeId)).Append(": ").AppendLine(propertyName);

    message.Append(nameof(Fields)).Append(':').AppendLine();
    foreach (string field in fields)
    {
      message.Append(" - ").AppendLine(field);
    }

    return message.ToString();
  }
}
