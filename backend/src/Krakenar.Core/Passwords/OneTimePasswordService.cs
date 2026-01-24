using Krakenar.Contracts.Passwords;
using Krakenar.Core.Passwords.Commands;
using Krakenar.Core.Passwords.Queries;
using Logitar.CQRS;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords;

public class OneTimePasswordService : IOneTimePasswordService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public OneTimePasswordService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<OneTimePasswordDto> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    CreateOneTimePassword command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<OneTimePasswordDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadOneTimePassword query = new(id);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<OneTimePasswordDto?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    ValidateOneTimePassword command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
