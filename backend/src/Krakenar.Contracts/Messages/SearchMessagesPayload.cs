using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Messages;

public record SearchMessagesPayload : SearchPayload
{
  public Guid? TemplateId { get; set; }
  public bool? IsDemo { get; set; }
  public MessageStatus? Status { get; set; }

  public new List<MessageSortOption> Sort { get; set; } = [];
}
