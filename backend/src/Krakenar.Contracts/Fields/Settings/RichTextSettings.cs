using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Contracts.Fields.Settings;

public record RichTextSettings : IRichTextSettings
{
  public string ContentType { get; set; }
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }

  public RichTextSettings() : this(MediaTypeNames.Text.Plain, minimumLength: null, maximumLength: null)
  {
  }

  [JsonConstructor]
  public RichTextSettings(string contentType, int? minimumLength, int? maximumLength)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
  }

  public RichTextSettings(IRichTextSettings richText) : this(richText.ContentType, richText.MinimumLength, richText.MaximumLength)
  {
  }
}
