using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Localization;

public class LanguageNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified language could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string Language
  {
    get => (string)Data[nameof(Language)]!;
    private set => Data[nameof(Language)] = value;
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
      error.Data[nameof(Language)] = Language;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LanguageNotFoundException(RealmId? realmId, string language, string propertyName) : base(BuildMessage(realmId, language, propertyName))
  {
    RealmId = realmId?.ToGuid();
    Language = language;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string language, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(Language), language)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
