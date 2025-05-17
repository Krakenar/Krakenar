using Krakenar.Contracts.Fields;

namespace Krakenar.Contracts.Contents;

public record UpdateContentLocalePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public List<FieldValuePayload> FieldValues { get; set; } = [];
}
