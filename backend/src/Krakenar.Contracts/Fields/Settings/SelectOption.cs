namespace Krakenar.Contracts.Fields.Settings;

public record SelectOption : ISelectOption
{
  public string Text { get; set; }
  public string? Value { get; set; }
  public string? Label { get; set; }
  public bool IsDisabled { get; set; }

  public SelectOption() : this(string.Empty)
  {
  }

  [JsonConstructor]
  public SelectOption(string text, string? value = null, string? label = null, bool isDisabled = false)
  {
    Text = text;
    Value = value;
    Label = label;
    IsDisabled = isDisabled;
  }

  public SelectOption(ISelectOption option) : this(option.Text, option.Value, option.Label, option.IsDisabled)
  {
  }
}
