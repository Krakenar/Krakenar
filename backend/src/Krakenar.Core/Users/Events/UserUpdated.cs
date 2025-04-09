using Krakenar.Core.Localization;
using Logitar.EventSourcing;
using TimeZone = Krakenar.Core.Localization.TimeZone;

namespace Krakenar.Core.Users.Events;

public record UserUpdated : DomainEvent
{
  public Change<PersonName>? FirstName { get; set; }
  public Change<PersonName>? MiddleName { get; set; }
  public Change<PersonName>? LastName { get; set; }
  public Change<string>? FullName { get; set; }
  public Change<PersonName>? Nickname { get; set; }

  public Change<DateTime?>? Birthdate { get; set; }
  public Change<Gender>? Gender { get; set; }
  public Change<Locale>? Locale { get; set; }
  public Change<TimeZone>? TimeZone { get; set; }

  public Change<Url>? Picture { get; set; }
  public Change<Url>? Profile { get; set; }
  public Change<Url>? Website { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];
}
