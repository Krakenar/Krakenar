using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Passwords;
using Krakenar.Core.Passwords.Validators;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords.Commands;

public record CreateOneTimePassword(CreateOneTimePasswordPayload Payload) : ICommand<OneTimePasswordDto>;

/// <exception cref="IdAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOneTimePasswordHandler : ICommandHandler<CreateOneTimePassword, OneTimePasswordDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IOneTimePasswordQuerier OneTimePasswordQuerier { get; }
  protected virtual IOneTimePasswordRepository OneTimePasswordRepository { get; }
  protected virtual IPasswordManager PasswordManager { get; }
  protected virtual IUserManager UserManager { get; }

  public CreateOneTimePasswordHandler(
    IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier,
    IOneTimePasswordRepository oneTimePasswordRepository,
    IPasswordManager passwordManager,
    IUserManager userManager)
  {
    ApplicationContext = applicationContext;
    OneTimePasswordQuerier = oneTimePasswordQuerier;
    OneTimePasswordRepository = oneTimePasswordRepository;
    PasswordManager = passwordManager;
    UserManager = userManager;
  }

  public virtual async Task<OneTimePasswordDto> HandleAsync(CreateOneTimePassword command, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = command.Payload;
    new CreateOneTimePasswordValidator().ValidateAndThrow(payload);

    ActorId? actorId = ApplicationContext.ActorId;
    RealmId? realmId = ApplicationContext.RealmId;

    OneTimePasswordId oneTimePasswordId = OneTimePasswordId.NewId(realmId);
    OneTimePassword? oneTimePassword;
    if (payload.Id.HasValue)
    {
      oneTimePasswordId = new(payload.Id.Value, realmId);
      oneTimePassword = await OneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
      if (oneTimePassword != null)
      {
        throw new IdAlreadyUsedException<OneTimePassword>(realmId, payload.Id.Value, nameof(payload.Id));
      }
    }

    Password password = PasswordManager.Generate(payload.Characters, payload.Length, out string passwordString);
    User? user = null;
    if (!string.IsNullOrWhiteSpace(payload.User))
    {
      FoundUsers foundUsers = await UserManager.FindAsync(payload.User, cancellationToken);
      user = foundUsers.ById ?? foundUsers.ByUniqueName ?? foundUsers.ByEmailAddress ?? foundUsers.ByCustomIdentifier
        ?? throw new UserNotFoundException(realmId, payload.User, nameof(payload.User));
    }
    oneTimePassword = new(password, payload.ExpiresOn, payload.MaximumAttempts, user, actorId, oneTimePasswordId);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      oneTimePassword.SetCustomAttribute(key, customAttribute.Value);
    }

    oneTimePassword.Update(actorId);
    await OneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    OneTimePasswordDto result = await OneTimePasswordQuerier.ReadAsync(oneTimePassword, cancellationToken);
    result.Password = passwordString;
    return result;
  }
}
