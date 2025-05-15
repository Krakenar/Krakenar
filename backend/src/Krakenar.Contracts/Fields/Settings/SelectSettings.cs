namespace Krakenar.Contracts.Fields.Settings;

public record SelectSettings
{
  public List<SelectOption> Options { get; set; } = [];
  public bool IsMultiple { get; set; }

  public SelectSettings()
  {
  }

  [JsonConstructor]
  public SelectSettings(List<SelectOption> options, bool isMultiple = false)
  {
    Options.AddRange(options);
    IsMultiple = isMultiple;
  }
}
