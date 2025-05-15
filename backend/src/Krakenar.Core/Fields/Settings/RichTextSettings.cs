using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Fields.Settings;

public record RichTextSettings : FieldTypeSettings, IRichTextSettings
{
  public override DataType DataType => DataType.RichText;

  public string ContentType { get; }
  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  public RichTextSettings() : this(MediaTypeNames.Text.Plain, minimumLength: null, maximumLength: null)
  {
  }

  [JsonConstructor]
  public RichTextSettings(string contentType, int? minimumLength, int? maximumLength)
  {
    ContentType = contentType.Trim();
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    new RichTextSettingsValidator().ValidateAndThrow(this);
  }

  public RichTextSettings(IRichTextSettings richText) : this(richText.ContentType, richText.MinimumLength, richText.MaximumLength)
  {
  }
}
