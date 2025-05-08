namespace Krakenar.Contracts.Messages;

public record SentMessages
{
  public List<Guid> Ids { get; set; } = [];

  public SentMessages()
  {
  }

  public SentMessages(IEnumerable<Guid> ids)
  {
    Ids.AddRange(ids);
  }
}
