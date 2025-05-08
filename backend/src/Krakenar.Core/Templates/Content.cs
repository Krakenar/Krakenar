using FluentValidation;
using Krakenar.Contracts.Templates;
using Krakenar.Core.Templates.Validators;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Templates;

public record Content : IContent
{
  public string Type { get; }
  public string Text { get; }

  [JsonConstructor]
  public Content(string type, string text)
  {
    Type = type.Trim().ToLowerInvariant();
    Text = text.Trim();
    new ContentValidator().ValidateAndThrow(this);
  }

  public Content(IContent content) : this(content.Type, content.Text)
  {
  }

  public static Content Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static Content PlainText(string text) => new(MediaTypeNames.Text.Plain, text);

  public Content Create(string text) => new(Type, text);
}
