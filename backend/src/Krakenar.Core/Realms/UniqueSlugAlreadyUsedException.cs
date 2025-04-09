using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Realms;

public class UniqueSlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique slug is already used.";

  public Guid RealmId
  {
    get => (Guid)Data[nameof(RealmId)]!;
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueSlug
  {
    get => (string)Data[nameof(UniqueSlug)]!;
    private set => Data[nameof(UniqueSlug)] = value;
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
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueSlug)] = UniqueSlug;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueSlugAlreadyUsedException(Realm realm, RealmId conflictId) : base(BuildMessage(realm, conflictId))
  {
    RealmId = realm.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    UniqueSlug = realm.UniqueSlug.ToString();
    PropertyName = nameof(realm.UniqueSlug);
  }

  private static string BuildMessage(Realm realm, RealmId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realm.Id.ToGuid())
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(UniqueSlug), realm.UniqueSlug)
    .AddData(nameof(PropertyName), nameof(realm.UniqueSlug))
    .Build();
}
