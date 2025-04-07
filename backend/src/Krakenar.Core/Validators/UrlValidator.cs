using FluentValidation;
using FluentValidation.Validators;

namespace Krakenar.Core.Validators;

public class UrlValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _schemes = ["http", "https"];

  public string Name { get; } = "UrlValidator";
  public IReadOnlyCollection<string> Schemes => [.. _schemes];

  public UrlValidator(IEnumerable<string>? schemes = null)
  {
    if (schemes is not null)
    {
      _schemes.Clear();
      foreach (string scheme in schemes)
      {
        _schemes.Add(scheme.ToLowerInvariant());
      }
    }
  }

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be a valid absolute Uniform Resource Locator (URL) using one of the following schemes: {string.Join(", ", _schemes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      Uri uri = new(value, UriKind.Absolute);
      return _schemes.Contains(uri.Scheme.ToLowerInvariant());
    }
    catch (Exception)
    {
      return false;
    }
  }
}
