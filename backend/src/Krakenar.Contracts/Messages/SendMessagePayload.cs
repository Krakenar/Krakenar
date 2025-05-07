namespace Krakenar.Contracts.Messages;

public record SendMessagePayload
{
  public string Sender { get; set; }
  public string Template { get; set; }

  // TODO(fpion): Recipients

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public List<Variable> Variables { get; set; } = [];

  public bool IsDemo { get; set; }

  public SendMessagePayload() : this(string.Empty, string.Empty)
  {
  }

  public SendMessagePayload(string sender, string template)
  {
    Sender = sender;
    Template = template;
  }
}
