using FluentValidation;

namespace Krakenar.Core;

public record Url
{
  public const int MaximumLength = 2048;

  public Uri Uri { get; }
  public string Value { get; }

  public Url(Uri uri)
  {
    Value = uri.ToString();
    new Validator().ValidateAndThrow(this);

    Uri = uri;
  }
  public Url(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);

    Uri = new(Value, UriKind.Absolute);
  }

  public static Url? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Url>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Url();
    }
  }
}
