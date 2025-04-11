using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;

namespace Krakenar.Infrastructure.Passwords;

public record PasswordInput
{
  public string Password { get; }

  public PasswordInput(IPasswordSettings settings, string password)
  {
    Password = password;
    new Validator(settings).ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<PasswordInput>
  {
    public Validator(IPasswordSettings settings)
    {
      RuleFor(x => x.Password).Password(settings);
    }
  }
}
