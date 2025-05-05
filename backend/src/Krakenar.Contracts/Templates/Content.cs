using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Contracts.Templates;

public record Content : IContent
{
  public string Type { get; set; }
  public string Text { get; set; }

  public Content() : this(string.Empty, string.Empty)
  {
  }

  public Content(IContent content) : this(content.Type, content.Text)
  {
  }

  public Content(string type, string text)
  {
    Type = type;
    Text = text;
  }

  public static Content Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static Content PlainText(string text) => new(MediaTypeNames.Text.Plain, text);
}
