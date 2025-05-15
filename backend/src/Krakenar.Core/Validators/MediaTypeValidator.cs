using FluentValidation;
using FluentValidation.Validators;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Validators;

public class MediaTypeValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _mediaTypes = new([MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain]);

  public IReadOnlyCollection<string> MediaTypes => [.. _mediaTypes];
  public string Name { get; } = "MediaTypeValidator";

  public MediaTypeValidator(IEnumerable<string>? mediaTypes = null)
  {
    if (mediaTypes is not null)
    {
      _mediaTypes.Clear();
      foreach (string contentType in mediaTypes)
      {
        _mediaTypes.Add(contentType.Trim().ToLowerInvariant());
      }
    }
  }

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be one of the following: {string.Join(", ", _mediaTypes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return _mediaTypes.Contains(value);
  }
}
