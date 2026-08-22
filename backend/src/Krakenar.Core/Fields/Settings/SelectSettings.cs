using FluentValidation;
using Krakenar.Contracts.Fields;
using SelectSettingsDto = Krakenar.Contracts.Fields.Settings.SelectSettings;

namespace Krakenar.Core.Fields.Settings;

public sealed record SelectSettings : FieldTypeSettings
{
  public override DataType DataType => DataType.Select;

  public IReadOnlyCollection<SelectOption> Options { get; }
  public bool IsMultiple { get; }

  public SelectSettings() : this(options: [])
  {
  }

  [JsonConstructor]
  public SelectSettings(IReadOnlyCollection<SelectOption> options, bool isMultiple = false)
  {
    Options = options.ToList().AsReadOnly();
    IsMultiple = isMultiple;
    new Validator().ValidateAndThrow(this);
  }

  public SelectSettings(SelectSettingsDto select)
    : this([.. select.Options.Select(option => new SelectOption(option))], select.IsMultiple)
  {
  }

  public bool Equals(SelectSettings? other) => other is SelectSettings settings
    && settings.Options.SequenceEqual(Options)
    && settings.IsMultiple == IsMultiple;
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (SelectOption option in Options)
    {
      hash.Add(option);
    }
    hash.Add(IsMultiple);
    return hash.ToHashCode();
  }
  public override string ToString()
  {
    StringBuilder value = new();
    value.Append(GetType()).AppendLine();
    if (Options.Count > 0)
    {
      value.Append(nameof(Options)).Append(':').AppendLine();
      foreach (SelectOption option in Options)
      {
        value.Append(" - ").Append(option).AppendLine();
      }
    }
    value.Append(nameof(IsMultiple)).Append(": ").Append(IsMultiple).AppendLine();
    return value.ToString();
  }

  private class Validator : AbstractValidator<SelectSettings>;
}
