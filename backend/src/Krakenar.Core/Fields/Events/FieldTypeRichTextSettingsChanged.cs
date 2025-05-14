using Krakenar.Core.Fields.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Fields.Events;

public record FieldTypeRichTextSettingsChanged(RichTextSettings Settings) : DomainEvent;
