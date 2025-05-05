using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords.Queries;

public record ReadOneTimePassword(Guid Id) : IQuery<OneTimePasswordDto?>;

public class ReadOneTimePasswordHandler : IQueryHandler<ReadOneTimePassword, OneTimePasswordDto?>
{
  protected virtual IOneTimePasswordQuerier OneTimePasswordQuerier { get; }

  public ReadOneTimePasswordHandler(IOneTimePasswordQuerier oneTimePasswordQuerier)
  {
    OneTimePasswordQuerier = oneTimePasswordQuerier;
  }

  public virtual async Task<OneTimePasswordDto?> HandleAsync(ReadOneTimePassword query, CancellationToken cancellationToken)
  {
    return await OneTimePasswordQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
