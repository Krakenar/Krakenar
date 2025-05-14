using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Templates;

namespace Krakenar.Contracts.Messages;

public class Message : Aggregate
{
  public Realm? Realm { get; set; }

  public string Subject { get; set; }
  public Content Body { get; set; } = new();

  public int RecipientCount { get; set; }
  public List<Recipient> Recipients { get; set; } = [];

  public Sender Sender { get; set; } = new();
  public Template Template { get; set; } = new();

  public bool IgnoreUserLocale { get; set; }
  public Locale? Locale { get; set; }

  public List<Variable> Variables { get; set; } = [];

  public bool IsDemo { get; set; }

  public MessageStatus Status { get; set; }
  public List<ResultData> Results { get; set; } = [];

  public Message() : this(string.Empty)
  {
  }

  public Message(string subject)
  {
    Subject = subject;
  }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
