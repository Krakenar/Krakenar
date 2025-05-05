using FluentValidation;
using FluentValidation.Validators;
using System.Net.Mime;

namespace Krakenar.Core.Validators;

public class ContentTypeValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _contentTypes = new([MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain]);

  public IReadOnlyCollection<string> ContentTypes => [.. _contentTypes];
  public string Name { get; } = "ContentTypeValidator";

  public ContentTypeValidator(IEnumerable<string>? contentTypes = null)
  {
    if (contentTypes is not null)
    {
      _contentTypes.Clear();
      foreach (string contentType in contentTypes)
      {
        _contentTypes.Add(contentType.ToLowerInvariant());
      }
    }
  }

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be one of the following: {string.Join(", ", _contentTypes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return _contentTypes.Contains(value);
  }
}
