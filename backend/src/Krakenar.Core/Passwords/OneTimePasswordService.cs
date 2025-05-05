using Krakenar.Contracts.Passwords;
using Krakenar.Core.Passwords.Commands;
using Krakenar.Core.Passwords.Queries;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords;

public class OneTimePasswordService : IOneTimePasswordService
{
  protected virtual ICommandHandler<CreateOneTimePassword, OneTimePasswordDto> CreateOneTimePassword { get; }
  protected virtual IQueryHandler<ReadOneTimePassword, OneTimePasswordDto?> ReadOneTimePassword { get; }
  protected virtual ICommandHandler<ValidateOneTimePassword, OneTimePasswordDto?> ValidateOneTimePassword { get; }

  public OneTimePasswordService(
    ICommandHandler<CreateOneTimePassword, OneTimePasswordDto> createOneTimePassword,
    IQueryHandler<ReadOneTimePassword, OneTimePasswordDto?> readOneTimePassword,
    ICommandHandler<ValidateOneTimePassword, OneTimePasswordDto?> validateOneTimePassword)
  {
    CreateOneTimePassword = createOneTimePassword;
    ReadOneTimePassword = readOneTimePassword;
    ValidateOneTimePassword = validateOneTimePassword;
  }

  public virtual async Task<OneTimePasswordDto> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    CreateOneTimePassword command = new(payload);
    return await CreateOneTimePassword.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<OneTimePasswordDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadOneTimePassword query = new(id);
    return await ReadOneTimePassword.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<OneTimePasswordDto?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    ValidateOneTimePassword command = new(id, payload);
    return await ValidateOneTimePassword.HandleAsync(command, cancellationToken);
  }
}
