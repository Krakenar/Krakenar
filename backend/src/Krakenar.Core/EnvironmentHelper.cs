namespace Krakenar.Core;

public static class EnvironmentHelper
{
  public static string GetString(string variable, string defaultValue = "") => TryGetString(variable) ?? defaultValue;
  public static string? TryGetString(string variable, string? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
  }
}
