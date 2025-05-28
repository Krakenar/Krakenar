using Krakenar.Contracts;
using Krakenar.Core.Localization;
using Logitar;

namespace Krakenar.Core.Contents;

public class InvalidFieldValuesException : BadRequestException
{
  private const string ErrorMessage = "The content locale has invalid field values.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
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
  public IReadOnlyCollection<Error> Errors
  {
    get => (IReadOnlyCollection<Error>)Data[nameof(Errors)]!;
    private set => Data[nameof(Errors)] = value;
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
      error.Data[nameof(ContentId)] = ContentId;
      error.Data[nameof(LanguageId)] = LanguageId;
      error.Data[nameof(Errors)] = Errors;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidFieldValuesException(ContentId contentId, LanguageId? languageId, IEnumerable<Error> errors, string propertyName)
    : base(BuildMessage(contentId, languageId, errors, propertyName))
  {
    RealmId = contentId.RealmId?.ToGuid();
    ContentId = contentId.EntityId;
    LanguageId = languageId?.EntityId;
    Errors = errors.ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentId contentId, LanguageId? languageId, IEnumerable<Error> errors, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(RealmId)).Append(": ").AppendLine(contentId.RealmId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(ContentId)).Append(": ").Append(contentId.EntityId).AppendLine();
    message.Append(nameof(LanguageId)).Append(": ").AppendLine(languageId?.EntityId.ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);

    message.Append(nameof(Errors)).Append(':').AppendLine();
    foreach (Error error in errors)
    {
      message.Append(" - ").AppendLine(JsonSerializer.Serialize(error));
    }

    return message.ToString();
  }
}
