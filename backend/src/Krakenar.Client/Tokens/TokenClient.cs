using Krakenar.Contracts.Tokens;

namespace Krakenar.Client.Tokens;

public class TokenClient : BaseClient, ITokenService
{
  protected virtual Uri Path { get; } = new("/api/tokens", UriKind.Relative);

  public TokenClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    ApiResult<CreatedToken> result = await PostAsync<CreatedToken>(Path, payload, cancellationToken);
    return result.Value ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    ApiResult<ValidatedToken> result = await PutAsync<ValidatedToken>(Path, payload, cancellationToken);
    return result.Value ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Put, Path, payload);
  }
}
