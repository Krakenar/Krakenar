using FluentValidation;

namespace Krakenar.Core.Localization;

public record Locale
{
  public const int MaximumLength = 16;

  public string Code { get; }
  public CultureInfo Culture { get; }

  public Locale(CultureInfo culture)
  {
    Code = culture.Name;
    new Validator().ValidateAndThrow(this);

    Culture = culture;
  }
  public Locale(string code)
  {
    Code = code.Trim();
    new Validator().ValidateAndThrow(this);

    Culture = new CultureInfo(code);
  }

  public override string ToString() => $"{Culture.DisplayName} ({Code})";

  internal class Validator : AbstractValidator<Locale>
  {
    public Validator()
    {
      RuleFor(x => x.Code).Locale();
    }
  }
}
