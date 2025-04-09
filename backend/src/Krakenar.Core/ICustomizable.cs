namespace Krakenar.Core;

public interface ICustomizable
{
  IReadOnlyDictionary<Identifier, string> CustomAttributes { get; }

  void RemoveCustomAttribute(Identifier key);
  void SetCustomAttribute(Identifier key, string value);
}
