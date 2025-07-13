using Krakenar.Core.ApiKeys;
using Krakenar.Core.Users;

namespace Krakenar.Core.Authentication;

public interface IAuthenticationService
{
  Task AuthenticatedAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
  Task AuthenticatedAsync(User user, CancellationToken cancellationToken = default);
}
