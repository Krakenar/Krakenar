using Logitar.EventSourcing;

namespace Krakenar.Core.Realms.Events;

public record RealmUniqueSlugChanged(Slug UniqueSlug) : DomainEvent;
