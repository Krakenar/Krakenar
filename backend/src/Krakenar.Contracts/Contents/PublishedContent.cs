namespace Krakenar.Contracts.Contents;

public class PublishedContent
{
  public Guid Id { get; set; }

  public ContentType ContentType { get; set; } = new();

  public PublishedContentLocale Invariant { get; set; } = new();
  public List<PublishedContentLocale> Locales { get; set; } = [];

  public PublishedContentLocale FindDefaultLocale()
  {
    return TryGetDefaultLocale() ?? throw new InvalidOperationException($"The published content (Id={Id}) does not have a default locale.");
  }
  public PublishedContentLocale FindLocale(Guid id)
  {
    return TryGetLocale(id) ?? throw new InvalidOperationException($"The published content (Id={Id}) does not have a locale for language 'Id={id}'.");
  }
  public PublishedContentLocale FindLocale(string code)
  {
    return TryGetLocale(code) ?? throw new InvalidOperationException($"The published content (Id={Id}) does not have a locale for language 'Locale={code}'.");
  }

  public bool HasDefaultLocale() => TryGetDefaultLocale() is not null;
  public bool HasLocale(Guid id) => TryGetLocale(id) is not null;
  public bool HasLocale(string code) => TryGetLocale(code) is not null;

  public PublishedContentLocale? TryGetDefaultLocale()
  {
    PublishedContentLocale[] locales = [.. Locales.Where(x => x.Language is not null && x.Language.IsDefault)];
    if (locales.Length > 1)
    {
      throw new InvalidOperationException($"The published content (Id={Id}) cannot have multiple ({locales.Length}) default locales.");
    }
    return locales.SingleOrDefault();
  }
  public PublishedContentLocale? TryGetLocale(Guid id)
  {
    PublishedContentLocale[] locales = [.. Locales.Where(x => x.Language is not null && x.Language.Id == id)];
    if (locales.Length > 1)
    {
      throw new InvalidOperationException($"The published content (Id={Id}) cannot have multiple ({locales.Length}) locales for language 'Id={id}'.");
    }
    return locales.SingleOrDefault();
  }
  public PublishedContentLocale? TryGetLocale(string code)
  {
    PublishedContentLocale[] locales = [.. Locales.Where(x => x.Language is not null && x.Language.Locale.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase))];
    if (locales.Length > 1)
    {
      throw new InvalidOperationException($"The published content (Id={Id}) cannot have multiple ({locales.Length}) locales for language 'Locale={code}'.");
    }
    return locales.SingleOrDefault();
  }

  public override bool Equals(object? obj) => obj is PublishedContent content && content.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Invariant} | {GetType()} (Id={Id})";
}
