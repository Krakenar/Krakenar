using Krakenar.Core.Fields;

namespace Krakenar.Core.Contents;

public record ContentLocale(UniqueName UniqueName, DisplayName? DisplayName, Description? Description, IReadOnlyDictionary<Guid, FieldValue> FieldValues);
