using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Realms.Events;

public record RealmCreated(
  Slug UniqueSlug,
  Secret Secret,
  UniqueNameSettings UniqueNameSettings,
  PasswordSettings PasswordSettings,
  bool RequireUniqueEmail,
  bool RequireConfirmedAccount) : DomainEvent;
