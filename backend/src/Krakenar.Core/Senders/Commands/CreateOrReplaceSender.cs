using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Senders.Validators;
using Krakenar.Core.Users;
using Logitar;
using Logitar.EventSourcing;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

public record CreateOrReplaceSender(Guid? Id, CreateOrReplaceSenderPayload Payload, long? Version) : ICommand<CreateOrReplaceSenderResult>, ISensitiveActivity
{
  public IActivity Anonymize()
  {
    CreateOrReplaceSender clone = this.DeepClone();
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
    SenderId senderId = SenderId.NewId(ApplicationContext.RealmId);
    Sender? sender = null;
    if (command.Id.HasValue)
    {
      senderId = new(command.Id.Value, senderId.RealmId);
      sender = await SenderRepository.LoadAsync(senderId, cancellationToken);
    }

    CreateOrReplaceSenderPayload payload = command.Payload;
    ValidationResult validation = new CreateOrReplaceSenderValidator(sender?.Provider).Validate(payload);
    if (!validation.IsValid)
    {
      throw new ValidationException(validation.Errors);
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
        SenderId? defaultId = await SenderQuerier.FindDefaultIdAsync(SenderKind.Email, cancellationToken);
        sender = new(email, settings, isDefault: !defaultId.HasValue, actorId, senderId);
      }
      else if (phone is not null)
      {
        SenderId? defaultId = await SenderQuerier.FindDefaultIdAsync(SenderKind.Phone, cancellationToken);
        sender = new(phone, settings, isDefault: !defaultId.HasValue, actorId, senderId);
      }
      else
      {
        throw new ArgumentException($"Exactly one of the following payload properties must be set: {nameof(payload.Email)}, {nameof(payload.Phone)}.", nameof(command));
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
          throw new SenderProviderNotSupportedException(settings.Provider);
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
      throw new ArgumentException($"Exactly one setting property must be set; {settings.Count} were set.", nameof(payload));
    }
    else if (settings.Count < 1)
    {
      throw new ArgumentException("Exactly one setting property must be set; none were set.", nameof(payload));
    }

    return settings.Single();
  }
}
