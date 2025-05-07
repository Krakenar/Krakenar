using Krakenar.Contracts.Messages;

namespace Krakenar.Core.Messages;

public record Recipients
{
  private readonly List<Recipient> _to;
  public IReadOnlyCollection<Recipient> To => _to.AsReadOnly();

  private readonly List<Recipient> _cc;
  public IReadOnlyCollection<Recipient> CC => _cc.AsReadOnly();

  private readonly List<Recipient> _bcc;
  public IReadOnlyCollection<Recipient> Bcc => _bcc.AsReadOnly();

  public Recipients(IEnumerable<Recipient> recipients) : this(recipients.Count())
  {
    foreach (Recipient recipient in recipients)
    {
      switch (recipient.Type)
      {
        case RecipientType.Bcc:
          _bcc.Add(recipient);
          break;
        case RecipientType.CC:
          _cc.Add(recipient);
          break;
        case RecipientType.To:
          _to.Add(recipient);
          break;
      }
    }
  }

  private Recipients(int capacity)
  {
    _to = new(capacity);
    _cc = new(capacity);
    _bcc = new(capacity);
  }
}
