using Krakenar.Contracts.Passwords;

namespace Krakenar.Client.Passwords;

public interface IOneTimePasswordClient : IOneTimePasswordService
{
  Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, RequestContext? context);
  Task<OneTimePassword?> ReadAsync(Guid id, RequestContext? context);
  Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, RequestContext? context);
}
