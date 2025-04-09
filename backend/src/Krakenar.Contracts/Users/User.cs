using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Sessions;

namespace Krakenar.Contracts.Users;

public class User : Aggregate
{
  public Realm? Realm { get; set; }

  public string UniqueName { get; set; } = string.Empty;

  public bool HasPassword { get; set; }
  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public Address? Address { get; set; }
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public Locale? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
  public List<CustomIdentifier> CustomIdentifiers { get; set; } = [];
  public List<Role> Roles { get; set; } = [];
  public List<Session> Sessions { get; set; } = [];

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
