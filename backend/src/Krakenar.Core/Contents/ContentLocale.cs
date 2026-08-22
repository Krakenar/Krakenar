using Krakenar.Core.Fields;

namespace Krakenar.Core.Contents;

public class ContentLocale
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

  public override bool Equals(object? obj) => obj is ContentLocale locale
    && locale.UniqueName == UniqueName
    && locale.DisplayName == DisplayName
    && locale.Description == Description
    && AreEqual(locale.FieldValues, FieldValues);
  public override int GetHashCode()
  {
    HashCode hash = new();
    hash.Add(UniqueName);
    hash.Add(DisplayName);
    hash.Add(Description);
    foreach (KeyValuePair<Guid, FieldValue> fieldValue in FieldValues)
    {
      hash.Add(fieldValue.Key);
      hash.Add(fieldValue.Value);
    }
    return hash.ToHashCode();
  }
  public override string ToString()
  {
    StringBuilder value = new();
    value.Append(GetType()).AppendLine();
    value.Append(nameof(UniqueName)).Append(": ").Append(UniqueName).AppendLine();
    if (DisplayName is not null)
    {
      value.Append(nameof(DisplayName)).Append(": ").Append(DisplayName).AppendLine();
    }
    if (Description is not null)
    {
      value.Append(nameof(Description)).Append(": ").Append(Description).AppendLine();
    }
    if (FieldValues.Count > 0)
    {
      value.Append(nameof(FieldValues)).Append(':').AppendLine();
      foreach (KeyValuePair<Guid, FieldValue> fieldValue in FieldValues)
      {
        value.Append(" - ").Append(fieldValue.Key).Append(": ").Append(fieldValue.Value).AppendLine();
      }
    }
    return value.ToString();
  }

  private static bool AreEqual(IReadOnlyDictionary<Guid, FieldValue> left, IReadOnlyDictionary<Guid, FieldValue> right)
  {
    if (left.Count != right.Count)
    {
      return false;
    }

    foreach (KeyValuePair<Guid, FieldValue> field in left)
    {
      if (!right.TryGetValue(field.Key, out FieldValue? value) || !value.Equals(field.Value))
      {
        return false;
      }
    }

    return true;
  }
}
