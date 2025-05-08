namespace Krakenar.Contracts.Messages;

public record SentMessages
{
  public List<Guid> Ids { get; set; } = [];

  public SentMessages(IEnumerable<Guid>? ids = null)
  {
    if (ids is not null)
    {
      Ids.AddRange(ids);
    }
  }
}
