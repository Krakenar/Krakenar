using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Dictionaries;

public class LanguageAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified language is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid DictionaryId
  {
    get => (Guid)Data[nameof(DictionaryId)]!;
    private set => Data[nameof(DictionaryId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
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
      error.Data[nameof(DictionaryId)] = DictionaryId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(LanguageId)] = LanguageId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LanguageAlreadyUsedException(Dictionary dictionary, DictionaryId conflictId) : base(BuildMessage(dictionary, conflictId))
  {
    RealmId = dictionary.RealmId?.ToGuid();
    DictionaryId = dictionary.EntityId;
    ConflictId = conflictId.EntityId;
    LanguageId = dictionary.LanguageId.EntityId;
    PropertyName = nameof(dictionary.LanguageId);
  }

  private static string BuildMessage(Dictionary dictionary, DictionaryId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), dictionary.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(LanguageId), dictionary.EntityId)
    .AddData(nameof(ConflictId), conflictId.EntityId)
    .AddData(nameof(LanguageId), dictionary.LanguageId.EntityId)
    .AddData(nameof(PropertyName), nameof(dictionary.LanguageId))
    .Build();
}
