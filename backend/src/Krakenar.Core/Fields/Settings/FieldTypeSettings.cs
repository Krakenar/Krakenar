using Krakenar.Contracts.Fields;

namespace Krakenar.Core.Fields.Settings;

public abstract record FieldTypeSettings
{
  public abstract DataType DataType { get; }
}
