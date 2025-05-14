namespace Krakenar.Contracts.Fields.Settings;

public interface IRelatedContentSettings
{
  Guid ContentTypeId { get; }
  bool IsMultiple { get; }
}
