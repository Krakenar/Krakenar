using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Localization;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Helper
{
  public static string Normalize(IEmail email) => Normalize(email.Address);
  public static string Normalize(Identifier identifier) => Normalize(identifier.Value);
  public static string Normalize(Locale locale) => Normalize(locale.Code);
  public static string Normalize(Slug slug) => Normalize(slug.Value);
  public static string Normalize(UniqueName uniqueName) => Normalize(uniqueName.Value);
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
