using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Users.Validators;

namespace Krakenar.Core.Senders.Validators;

public class CreateOrReplaceSenderValidator : AbstractValidator<CreateOrReplaceSenderPayload>
{
  protected virtual bool IsCreate { get; }

  public CreateOrReplaceSenderValidator(SenderProvider? provider)
  {
    if (provider.HasValue)
    {
      ConfigureReplace(provider.Value);
    }
    else
    {
      IsCreate = true;
      ConfigureCreate();
    }

    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }

  public override ValidationResult Validate(ValidationContext<CreateOrReplaceSenderPayload> context)
  {
    if (IsCreate)
    {
      CreateOrReplaceSenderPayload payload = context.InstanceToValidate;
      SenderKind? kind = GetSenderKind(payload);
      SenderProvider? provider = GetSenderProvider(payload);

      List<ValidationFailure> failures = new(capacity: 2);
      if (kind.HasValue && provider.HasValue)
      {
        SenderKind expectedKind = provider.Value.GetSenderKind();
        if (expectedKind != kind.Value)
        {
          ValidationFailure failure = new()
          {
            ErrorCode = nameof(CreateOrReplaceSenderValidator),
            ErrorMessage = "The combination of the sender kind and provider is not compatible."
          };
          failures.Add(failure);
        }
      }
      else
      {
        if (!kind.HasValue)
        {
          ValidationFailure failure = new()
          {
            ErrorCode = nameof(CreateOrReplaceSenderValidator),
            ErrorMessage = $"Exactly one of the following properties must be provided: {nameof(payload.Email)}, {nameof(payload.Phone)}."
          };
          failures.Add(failure);
        }
        if (!provider.HasValue)
        {
          ValidationFailure failure = new()
          {
            ErrorCode = nameof(CreateOrReplaceSenderValidator),
            ErrorMessage = $"Exactly one of the following properties must be provided: {nameof(payload.SendGrid)}, {nameof(payload.Twilio)}."
          };
          failures.Add(failure);
        }
      }
      if (failures.Count > 1)
      {
        return new ValidationResult(failures);
      }
    }

    return base.Validate(context);
  }

  protected virtual void ConfigureCreate()
  {
    When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone is not null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => x.SendGrid is not null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
    When(x => x.Twilio is not null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
  }

  protected virtual void ConfigureReplace(SenderProvider provider)
  {
    SenderKind kind = provider.GetSenderKind();
    switch (kind)
    {
      case SenderKind.Email:
        When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()))
          .Otherwise(() => RuleFor(x => x.Email).NotNull());
        RuleFor(x => x.Phone).Null();
        break;
      case SenderKind.Phone:
        RuleFor(x => x.Email).Null();
        When(x => x.Phone is not null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()))
          .Otherwise(() => RuleFor(x => x.Phone).NotNull());
        break;
    }

    switch (provider)
    {
      case SenderProvider.SendGrid:
        When(x => x.SendGrid is not null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.SendGrid).NotNull());
        RuleFor(x => x.Twilio).Null();
        break;
      case SenderProvider.Twilio:
        RuleFor(x => x.SendGrid).Null();
        When(x => x.Twilio is not null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.Twilio).NotNull());
        break;
      default:
        throw new SenderProviderNotSupportedException(provider);
    }
  }

  protected virtual SenderKind? GetSenderKind(CreateOrReplaceSenderPayload payload)
  {
    List<SenderKind> kinds = new(capacity: 2);
    if (payload.Email is not null)
    {
      kinds.Add(SenderKind.Email);
    }
    if (payload.Phone is not null)
    {
      kinds.Add(SenderKind.Phone);
    }
    return kinds.Count == 1 ? kinds.Single() : null;
  }

  protected virtual SenderProvider? GetSenderProvider(CreateOrReplaceSenderPayload payload)
  {
    List<SenderProvider> providers = new(capacity: 2);
    if (payload.SendGrid is not null)
    {
      providers.Add(SenderProvider.SendGrid);
    }
    if (payload.Twilio is not null)
    {
      providers.Add(SenderProvider.Twilio);
    }
    return providers.Count == 1 ? providers.Single() : null;
  }
}
