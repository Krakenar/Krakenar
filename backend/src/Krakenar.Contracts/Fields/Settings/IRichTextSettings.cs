namespace Krakenar.Contracts.Fields.Settings;

public interface IRichTextSettings
{
  string ContentType { get; }
  int? MinimumLength { get; }
  int? MaximumLength { get; }
}
