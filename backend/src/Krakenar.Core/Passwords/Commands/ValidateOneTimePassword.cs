using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Passwords;
using Krakenar.Core.Passwords.Validators;
using Logitar.CQRS;
using Logitar.EventSourcing;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords.Commands;

public record ValidateOneTimePassword(Guid Id, ValidateOneTimePasswordPayload Payload) : ICommand<OneTimePasswordDto?>;

/// <exception cref="IncorrectOneTimePasswordPasswordException"></exception>
/// <exception cref="MaximumAttemptsReachedException"></exception>
/// <exception cref="OneTimePasswordAlreadyUsedException"></exception>
/// <exception cref="OneTimePasswordIsExpiredException"></exception>
/// <exception cref="ValidationException"></exception>
public class ValidateOneTimePasswordHandler : ICommandHandler<ValidateOneTimePassword, OneTimePasswordDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IOneTimePasswordQuerier OneTimePasswordQuerier { get; }
  protected virtual IOneTimePasswordRepository OneTimePasswordRepository { get; }

  public ValidateOneTimePasswordHandler(
    IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier,
    IOneTimePasswordRepository oneTimePasswordRepository)
  {
    ApplicationContext = applicationContext;
    OneTimePasswordQuerier = oneTimePasswordQuerier;
    OneTimePasswordRepository = oneTimePasswordRepository;
  }

  public virtual async Task<OneTimePasswordDto?> HandleAsync(ValidateOneTimePassword command, CancellationToken cancellationToken)
  {
    ValidateOneTimePasswordPayload payload = command.Payload;
    new ValidateOneTimePasswordValidator().ValidateAndThrow(payload);

    OneTimePasswordId oneTimePasswordId = new(command.Id, ApplicationContext.RealmId);
    OneTimePassword? oneTimePassword = await OneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
    if (oneTimePassword is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    bool isValid = oneTimePassword.Validate(payload.Password, actorId);
    if (isValid)
    {
      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        oneTimePassword.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
      }
      oneTimePassword.Update(actorId);
    }

    await OneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    if (!isValid)
    {
      throw new IncorrectOneTimePasswordPasswordException(oneTimePassword, payload.Password);
    }

    return await OneTimePasswordQuerier.ReadAsync(oneTimePassword, cancellationToken);
  }
}
