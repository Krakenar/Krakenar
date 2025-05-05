using FluentValidation;
using Krakenar.Contracts.ApiKeys;

namespace Krakenar.Core.ApiKeys.Validators;

public class AuthenticateApiKeyValidator : AbstractValidator<AuthenticateApiKeyPayload>
{
  public AuthenticateApiKeyValidator()
  {
    RuleFor(x => x.XApiKey).NotEmpty();
  }
}
