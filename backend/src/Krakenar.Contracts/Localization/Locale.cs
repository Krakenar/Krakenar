namespace Krakenar.Contracts.Localization;

public record Locale
{
  public int LCID { get; set; }
  public string Code { get; set; }
  public string DisplayName { get; set; }
  public string EnglishName { get; set; }
  public string NativeName { get; set; }

  public Locale() : this(CultureInfo.InvariantCulture)
  {
  }

  public Locale(string code) : this(CultureInfo.GetCultureInfo(code))
  {
  }

  public Locale(CultureInfo culture)
  {
    LCID = culture.LCID;
    Code = culture.Name;
    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }

  public override string ToString() => $"{DisplayName} ({Code})";
}
