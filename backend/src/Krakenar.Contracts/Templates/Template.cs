using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Templates;

public class Template : Aggregate
{
  public Realm? Realm { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public Template() : this(string.Empty, string.Empty, new Content())
  {
  }

  public Template(string uniqueName, string subject, Content content)
  {
    UniqueName = uniqueName;

    Subject = subject;
    Content = content;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
