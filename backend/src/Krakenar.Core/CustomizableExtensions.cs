using Krakenar.Contracts;

namespace Krakenar.Core;

public static class CustomizableExtensions
{
  public static int SetCustomAttributes(this ICustomizable customizable, IEnumerable<CustomAttribute> customAttributes, ICustomizable? reference = null)
  {
    int changes = 0;

    reference ??= customizable;

    HashSet<Identifier> keys = customAttributes.Select(customAttribute => new Identifier(customAttribute.Key)).ToHashSet();
    foreach (Identifier key in reference.CustomAttributes.Keys)
    {
      if (!keys.Contains(key))
      {
        customizable.RemoveCustomAttribute(key);
        changes++;
      }
    }

    foreach (CustomAttribute customAttribute in customAttributes)
    {
      Identifier key = new(customAttribute.Key);
      if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || existingValue != customAttribute.Value)
      {
        customizable.SetCustomAttribute(key, customAttribute.Value);
        changes++;
      }
    }

    return changes;
  }
}
