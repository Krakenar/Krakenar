namespace Krakenar.Contracts.Senders;

public record CreateOrReplaceSenderResult
{
  public Sender? Sender { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceSenderResult()
  {
  }

  public CreateOrReplaceSenderResult(Sender? sender, bool created)
  {
    Sender = sender;
    Created = created;
  }
}
