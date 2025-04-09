using FluentValidation;
using NodaTime;

namespace Krakenar.Core.Localization;

public record TimeZone
{
  public const int MaximumLength = 32;

  public DateTimeZone DateTimeZone { get; }
  public string Id { get; }

  public TimeZone(DateTimeZone dateTimeZone)
  {
    Id = dateTimeZone.Id;
    new Validator().ValidateAndThrow(this);

    DateTimeZone = dateTimeZone;
  }

  public TimeZone(string id)
  {
    Id = id.Trim();
    new Validator().ValidateAndThrow(this);

    DateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(Id) ?? throw new InvalidOperationException($"The date time zone 'Id={Id}' could not be resolved.");
  }

  public static TimeZone? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Id;

  private class Validator : AbstractValidator<TimeZone>
  {
    public Validator()
    {
      RuleFor(x => x.Id).TimeZone();
    }
  }
}
