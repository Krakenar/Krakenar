using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Senders.Validators;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

public record CreateOrReplaceSender(Guid? Id, CreateOrReplaceSenderPayload Payload, long? Version) : ICommand<CreateOrReplaceSenderResult>;

/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceSenderHandler : ICommandHandler<CreateOrReplaceSender, CreateOrReplaceSenderResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISenderQuerier SenderQuerier { get; }
  protected virtual ISenderRepository SenderRepository { get; }

  public CreateOrReplaceSenderHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    ApplicationContext = applicationContext;
    SenderQuerier = senderQuerier;
    SenderRepository = senderRepository;
  }

  public virtual async Task<CreateOrReplaceSenderResult> HandleAsync(CreateOrReplaceSender command, CancellationToken cancellationToken)
  {
    CreateOrReplaceSenderPayload payload = command.Payload;
    new CreateOrReplaceSenderValidator().ValidateAndThrow(payload);

    SenderId senderId = SenderId.NewId(ApplicationContext.RealmId);
    Sender? sender = null;
    if (command.Id.HasValue)
    {
      senderId = new(command.Id.Value, senderId.RealmId);
      sender = await SenderRepository.LoadAsync(senderId, cancellationToken);
    }

    Email? email = payload.Email is null ? null : new(payload.Email);
    Phone? phone = payload.Phone is null ? null : new(payload.Phone);
    SenderSettings settings = GetSettings(payload);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (sender is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceSenderResult();
      }

      if (email is not null)
      {
        int count = await SenderQuerier.CountAsync(SenderKind.Email, cancellationToken);
        sender = new(email, settings, isDefault: count == 0, actorId, senderId);
      }
      else if (phone is not null)
      {
        int count = await SenderQuerier.CountAsync(SenderKind.Phone, cancellationToken);
        sender = new(phone, settings, isDefault: count == 0, actorId, senderId);
      }
      else
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }
      created = true;
    }

    Sender reference = (command.Version.HasValue
      ? await SenderRepository.LoadAsync(senderId, command.Version, cancellationToken)
      : null) ?? sender;

    if (reference.Email != email)
    {
      sender.Email = email;
    }
    if (reference.Phone != phone)
    {
      sender.Phone = phone;
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      sender.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      sender.Description = description;
    }

    if (reference.Settings != settings)
    {
      switch (settings.Provider)
      {
        case SenderProvider.SendGrid:
          sender.SetSettings((SendGridSettings)settings);
          break;
        case SenderProvider.Twilio:
          sender.SetSettings((TwilioSettings)settings);
          break;
        default:
          throw new SenderProviderNotSupported(settings.Provider);
      }
    }

    sender.Update(actorId);
    await SenderRepository.SaveAsync(sender, cancellationToken);

    SenderDto dto = await SenderQuerier.ReadAsync(sender, cancellationToken);
    return new CreateOrReplaceSenderResult(dto, created);
  }

  protected virtual SenderSettings GetSettings(CreateOrReplaceSenderPayload payload)
  {
    List<SenderSettings> settings = new(capacity: 2);

    if (payload.SendGrid is not null)
    {
      settings.Add(new SendGridSettings(payload.SendGrid));
    }
    if (payload.Twilio is not null)
    {
      settings.Add(new TwilioSettings(payload.Twilio));
    }

    if (settings.Count > 1)
    {
      // TODO(fpion): implement
    }
    else if (settings.Count < 1)
    {
      // TODO(fpion): implement
    }

    return settings.Single();
  }
}
