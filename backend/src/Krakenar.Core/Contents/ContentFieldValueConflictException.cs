using FluentValidation.Results;
using Krakenar.Contracts;
using Krakenar.Core.Localization;
using Logitar;

namespace Krakenar.Core.Contents;

public class ContentFieldValueConflictException : ConflictException
{
  private const string ErrorMessage = "The specified field values are already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)]!;
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ContentId
  {
    get => (Guid)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
  }
  public Guid? LanguageId
  {
    get => (Guid?)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
  }
  public IReadOnlyCollection<ValidationFailure> Conflicts
  {
    get => (IReadOnlyCollection<ValidationFailure>)Data[nameof(Conflicts)]!;
    private set => Data[nameof(Conflicts)] = value;
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
      error.Data[nameof(Conflicts)] = Conflicts;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentFieldValueConflictException(ContentId contentId, LanguageId? languageId, IEnumerable<ValidationFailure> conflicts, string propertyName)
    : base(BuildMessage(contentId, languageId, conflicts, propertyName))
  {
    RealmId = contentId.RealmId?.ToGuid();
    ContentId = contentId.EntityId;
    LanguageId = languageId?.EntityId;
    Conflicts = conflicts.ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentId contentId, LanguageId? languageId, IEnumerable<ValidationFailure> conflicts, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(RealmId)).Append(": ").AppendLine(contentId.RealmId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(ContentId)).Append(": ").Append(contentId.EntityId).AppendLine();
    message.Append(nameof(LanguageId)).Append(": ").AppendLine(languageId?.EntityId.ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);

    message.Append(nameof(Conflicts)).Append(':').AppendLine();
    foreach (ValidationFailure conflict in conflicts)
    {
      string formatted = string.Join(" | ", conflict.PropertyName, conflict.ErrorCode, conflict.ErrorMessage, conflict.AttemptedValue);
      message.Append(" - ").AppendLine(formatted);
    }

    return message.ToString();
  }
}
