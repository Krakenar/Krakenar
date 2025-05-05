namespace Krakenar.Contracts.Templates;

public record CreateOrReplaceTemplatePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public CreateOrReplaceTemplatePayload() : this(string.Empty, string.Empty, new Content())
  {
  }

  public CreateOrReplaceTemplatePayload(string uniqueName, string subject, Content content)
  {
    UniqueName = uniqueName;

    Subject = subject;
    Content = content;
  }
}
