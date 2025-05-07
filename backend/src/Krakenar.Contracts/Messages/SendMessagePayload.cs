namespace Krakenar.Contracts.Messages;

public record SendMessagePayload
{
  public string Sender { get; set; }
  public string Template { get; set; }

  // TODO(fpion): Recipients

  // TODO(fpion): IgnoreUserLocale
  // TODO(fpion): Locale

  // TODO(fpion): Variables

  // TODO(fpion): IsDemo

  public SendMessagePayload() : this(string.Empty, string.Empty)
  {
  }

  public SendMessagePayload(string sender, string template)
  {
    Sender = sender;
    Template = template;
  }
}
