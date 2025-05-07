using FluentValidation;
using Krakenar.Contracts.Messages;
using Krakenar.Core.Messages.Validators;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Messages.Commands;

public record SendMessage(SendMessagePayload Payload) : ICommand<SentMessages>;

/// <exception cref="InvalidSmsMessageContentTypeException"></exception>
/// <exception cref="SenderNotFoundException"></exception>
/// <exception cref="TemplateNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class SendMessageHandler : ICommandHandler<SendMessage, SentMessages>
{
  protected virtual IMessageQuerier MessageQuerier { get; }
  protected virtual ISenderManager SenderManager { get; }
  protected virtual ITemplateManager TemplateManager { get; }

  public SendMessageHandler(
    IMessageQuerier messageQuerier,
    ISenderManager senderManager,
    ITemplateManager templateManager)
  {
    MessageQuerier = messageQuerier;
    SenderManager = senderManager;
    TemplateManager = templateManager;
  }

  public virtual async Task<SentMessages> HandleAsync(SendMessage command, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    new SendMessageValidator().ValidateAndThrow(payload);

    Sender sender = await SenderManager.FindAsync(payload.Sender, nameof(payload.Sender), cancellationToken);
    Template template = await TemplateManager.FindAsync(payload.Template, nameof(payload.Template), cancellationToken);
    if (sender.Kind == Contracts.Senders.SenderKind.Phone && template.Content.Type != MediaTypeNames.Text.Plain)
    {
      throw new InvalidSmsMessageContentTypeException(template.Content.Type, nameof(payload.Template));
    }

    Variables variables = new(payload.Variables);
    IReadOnlyDictionary<string, string> variableDictionary = variables.AsDictionary();

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
