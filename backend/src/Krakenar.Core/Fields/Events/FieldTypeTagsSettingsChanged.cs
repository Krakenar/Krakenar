using Krakenar.Core.Fields.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Fields.Events;

public record FieldTypeTagsSettingsChanged(TagsSettings Settings) : DomainEvent;
