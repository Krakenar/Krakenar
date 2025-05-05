namespace Krakenar.Contracts.Templates;

public record CreateOrReplaceTemplateResult
{
  public Template? Template { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceTemplateResult()
  {
  }

  public CreateOrReplaceTemplateResult(Template? role, bool created)
  {
    Template = role;
    Created = created;
  }
}
