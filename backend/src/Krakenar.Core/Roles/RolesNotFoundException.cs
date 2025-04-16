using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Roles;

public class RolesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified roles could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    set => Data[nameof(RealmId)] = value;
  }
  public IReadOnlyCollection<string> Roles
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Roles)]!;
    set => Data[nameof(Roles)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(Roles)] = Roles;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RolesNotFoundException(RealmId? realmId, IEnumerable<string> roles, string propertyName) : base(BuildMessage(realmId, roles, propertyName))
  {
    RealmId = realmId?.ToGuid();
    Roles = roles.ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, IEnumerable<string> roles, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(RealmId)).Append(": ").AppendLine(realmId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(Roles)).Append(':').AppendLine();
    foreach (string role in roles)
    {
      message.Append(" - ").AppendLine(role);
    }
    return message.ToString();
  }
}
