namespace Krakenar.Contracts.Fields.Settings;

public record TagsSettings : ITagsSettings
{
  [JsonConstructor]
  public TagsSettings()
  {
  }

  public TagsSettings(ITagsSettings _) : this()
  {
  }
}
