namespace Krakenar.Core.Fields;

public record FieldDefinition(
  Guid Id,
  FieldTypeId FieldTypeId,
  bool IsInvariant,
  bool IsRequired,
  bool IsIndexed,
  bool IsUnique,
  Identifier UniqueName,
  DisplayName? DisplayName,
  Description? Description,
  Placeholder? Placeholder);
