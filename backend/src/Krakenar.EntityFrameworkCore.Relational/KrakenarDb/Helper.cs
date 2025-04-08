using Krakenar.Core;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Helper
{
  public static string Normalize(Slug slug) => Normalize(slug.Value);
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
