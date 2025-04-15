using Krakenar.Contracts.Roles;

namespace Krakenar.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public ChangePasswordPayload? Password { get; set; }
  public bool? IsDisabled { get; set; }

  public Change<AddressPayload>? Address { get; set; }
  public Change<EmailPayload>? Email { get; set; }
  public Change<PhonePayload>? Phone { get; set; }

  public Change<string>? FirstName { get; set; }
  public Change<string>? MiddleName { get; set; }
  public Change<string>? LastName { get; set; }
  public Change<string>? Nickname { get; set; }

  public Change<DateTime?>? Birthdate { get; set; }
  public Change<string>? Gender { get; set; }
  public Change<string>? Locale { get; set; }
  public Change<string>? TimeZone { get; set; }

  public Change<string>? Picture { get; set; }
  public Change<string>? Profile { get; set; }
  public Change<string>? Website { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
  public List<RoleChange> Roles { get; set; } = [];
}
