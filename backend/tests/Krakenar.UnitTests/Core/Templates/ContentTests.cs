using FluentValidation;
using ContentDto = Krakenar.Contracts.Templates.Content;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Templates;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTests
{
  [Fact(DisplayName = "Html: it should construct the correct HTML template.")]
  public void Given_Text_When_Html_Then_HtmlContent()
  {
    string text = "<div>Hello World!</div>";
    Content content = Content.Html(text);
    Assert.Equal(MediaTypeNames.Text.Html, content.Type);
    Assert.Equal(text, content.Text);
  }

  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Content_When_ctor_Then_Instance()
  {
    ContentDto source = ContentDto.PlainText("Hello World!");
    Content content = new(source);
    Assert.Equal(source.Type, content.Type);
    Assert.Equal(source.Text, content.Text);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Instance()
  {
    string type = $" {MediaTypeNames.Text.Html} ".ToUpperInvariant();
    string text = "  <div>Hello World!</div>  ";
    Content content = new(type, text);
    Assert.Equal(type.Trim().ToLowerInvariant(), content.Type);
    Assert.Equal(text.Trim(), content.Text);
  }

  [Fact(DisplayName = "It should throw ValidationException when the text is not valid.")]
  public void Given_InvalidText_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new Content(MediaTypeNames.Text.Html, string.Empty));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Text");
  }

  [Fact(DisplayName = "It should throw ValidationException when the type is not valid.")]
  public void Given_InvalidType_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new Content(MediaTypeNames.Application.Json, @"{""message"":""Hello World!""}"));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.PropertyName == "Type");
  }

  [Fact(DisplayName = "PlainText: it should construct the correct plain text template.")]
  public void Given_Text_When_PlainText_Then_HtmlContent()
  {
    string text = "Hello World!";
    Content content = Content.PlainText(text);
    Assert.Equal(MediaTypeNames.Text.Plain, content.Type);
    Assert.Equal(text, content.Text);
  }
}
