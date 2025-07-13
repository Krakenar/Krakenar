namespace Krakenar.Core;

public static class EnvironmentHelper
{
  public static bool GetBoolean(string variable, bool defaultValue = false) => TryGetBoolean(variable) ?? defaultValue;
  public static bool? TryGetBoolean(string variable, bool? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    if (!string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out bool boolValue))
    {
      return boolValue;
    }
    return defaultValue;
  }

  public static int GetInt32(string variable, int defaultValue = 0) => TryGetInt32(variable) ?? defaultValue;
  public static int? TryGetInt32(string variable, int? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int intValue))
    {
      return intValue;
    }
    return defaultValue;
  }

  public static string GetString(string variable, string defaultValue = "") => TryGetString(variable) ?? defaultValue;
  public static string? TryGetString(string variable, string? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
  }
}
