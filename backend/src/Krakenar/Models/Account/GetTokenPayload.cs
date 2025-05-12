using FluentValidation;

namespace Krakenar.Models.Account;

public record GetTokenPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  [JsonPropertyName("credentials")]
  public SignInAccountPayload? Credentials { get; set; }

  public GetTokenPayload()
  {
  }

  public GetTokenPayload(string refreshToken)
  {
    RefreshToken = refreshToken;
  }

  public GetTokenPayload(SignInAccountPayload credentials)
  {
    Credentials = credentials;
  }
}

internal class GetTokenValidator : AbstractValidator<GetTokenPayload>
{
  public GetTokenValidator()
  {
    When(x => string.IsNullOrWhiteSpace(x.RefreshToken), () => RuleFor(x => x.Credentials).NotNull());
    When(x => !string.IsNullOrWhiteSpace(x.RefreshToken), () => RuleFor(x => x.Credentials).Null());

    When(x => x.Credentials is null, () => RuleFor(x => x.RefreshToken).NotEmpty());
    When(x => x.Credentials is not null, () => RuleFor(x => x.RefreshToken).Empty());
  }
}
