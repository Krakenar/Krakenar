using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Localization;

public class CannotDeleteDefaultLanguageException : BadRequestException
{
  private const string ErrorMessage = "The default language cannot be deleted.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(LanguageId)] = LanguageId;
      return error;
    }
  }

  public CannotDeleteDefaultLanguageException(Language language) : base(BuildMessage(language))
  {
    if (!language.IsDefault)
    {
      throw new ArgumentException("The language must be default.", nameof(language));
    }

    RealmId = language.RealmId?.ToGuid();
    LanguageId = language.EntityId;
  }

  private static string BuildMessage(Language language) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), language.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(LanguageId), language.EntityId)
    .Build();
}
