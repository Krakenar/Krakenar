namespace Krakenar.Core;

public static class EnvironmentHelper
{
  public static string GetString(string variable, string defaultValue = "")
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
  }
}
