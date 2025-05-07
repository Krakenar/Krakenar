using FluentValidation;
using Krakenar.Contracts.Messages;
using Krakenar.Core.Messages.Validators;

namespace Krakenar.Core.Messages.Commands;

public record SendMessage(SendMessagePayload Payload) : ICommand<SentMessages>;

public class SendMessageHandler : ICommandHandler<SendMessage, SentMessages>
{
  protected virtual IMessageQuerier MessageQuerier { get; }

  public SendMessageHandler(IMessageQuerier messageQuerier)
  {
    MessageQuerier = messageQuerier;
  }

  public virtual async Task<SentMessages> HandleAsync(SendMessage command, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    new SendMessageValidator().ValidateAndThrow(payload);

    // TODO(fpion): resolve Sender

    // TODO(fpion): resolve Template

    await Task.Delay(1000, cancellationToken);

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
