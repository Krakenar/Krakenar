using Krakenar.Core.Fields;

namespace Krakenar.Core.Contents;

public record ContentLocale
{
  public UniqueName UniqueName { get; }
  public DisplayName? DisplayName { get; }
  public Description? Description { get; }
  public IReadOnlyDictionary<Guid, FieldValue> FieldValues { get; }

  public ContentLocale(UniqueName uniqueName, DisplayName? displayName = null, Description? description = null, IReadOnlyDictionary<Guid, FieldValue>? fieldValues = null)
  {
    UniqueName = uniqueName;
    DisplayName = displayName;
    Description = description;
    FieldValues = fieldValues ?? new Dictionary<Guid, FieldValue>().AsReadOnly();
  }
}
