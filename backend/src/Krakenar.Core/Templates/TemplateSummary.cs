namespace Krakenar.Core.Templates;

public record TemplateSummary
{
  public TemplateId Id { get; }
  public UniqueName UniqueName { get; }
  public DisplayName? DisplayName { get; }

  [JsonConstructor]
  public TemplateSummary(TemplateId id, UniqueName uniqueName, DisplayName? displayName)
  {
    Id = id;
    UniqueName = uniqueName;
    DisplayName = displayName;
  }

  public TemplateSummary(Template template) : this(template.Id, template.UniqueName, template.DisplayName)
  {
  }
}
