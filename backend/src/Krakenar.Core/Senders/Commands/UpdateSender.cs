using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Senders.Validators;
using Krakenar.Core.Users;
using Logitar;
using Logitar.EventSourcing;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

public record UpdateSender(Guid Id, UpdateSenderPayload Payload) : ICommand<SenderDto?>, ISensitiveActivity
{
  public IActivity Anonymize()
  {
    UpdateSender clone = this.DeepClone();
    if (clone.Payload.SendGrid is not null)
    {
      clone.Payload.SendGrid.ApiKey = clone.Payload.SendGrid.ApiKey.Mask();
    }
    if (clone.Payload.Twilio is not null)
    {
      clone.Payload.Twilio.AccountSid = clone.Payload.Twilio.AccountSid.Mask();
      clone.Payload.Twilio.AuthenticationToken = clone.Payload.Twilio.AuthenticationToken.Mask();
    }
    return clone;
  }
}

/// <exception cref="ValidationException"></exception>
public class UpdateSenderHandler : ICommandHandler<UpdateSender, SenderDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISenderQuerier SenderQuerier { get; }
  protected virtual ISenderRepository SenderRepository { get; }

  public UpdateSenderHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    ApplicationContext = applicationContext;
    SenderQuerier = senderQuerier;
    SenderRepository = senderRepository;
  }

  public virtual async Task<SenderDto?> HandleAsync(UpdateSender command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(command.Id, ApplicationContext.RealmId);
    Sender? sender = await SenderRepository.LoadAsync(senderId, cancellationToken);
    if (sender is null)
    {
      return null;
    }

    UpdateSenderPayload payload = command.Payload;
    new UpdateSenderValidator(sender.Provider).ValidateAndThrow(payload);

    ActorId? actorId = ApplicationContext.ActorId;

    if (payload.Email is not null)
    {
      sender.Email = new Email(payload.Email);
    }
    if (payload.Phone is not null)
    {
      sender.Phone = new Phone(payload.Phone);
    }
    if (payload.DisplayName is not null)
    {
      sender.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      sender.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.SendGrid is not null)
    {
      SendGridSettings settings = new(payload.SendGrid);
      sender.SetSettings(settings, actorId);
    }
    if (payload.Twilio is not null)
    {
      TwilioSettings settings = new(payload.Twilio);
      sender.SetSettings(settings, actorId);
    }

    sender.Update(actorId);
    await SenderRepository.SaveAsync(sender, cancellationToken);

    return await SenderQuerier.ReadAsync(sender, cancellationToken);
  }
}
