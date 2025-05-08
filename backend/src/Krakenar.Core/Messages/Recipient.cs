using FluentValidation;
using Krakenar.Contracts.Messages;
using Krakenar.Core.Users;
using Logitar;

namespace Krakenar.Core.Messages;

public record Recipient
{
  public RecipientType Type { get; }

  public Email? Email { get; }
  public Phone? Phone { get; }
  public DisplayName? DisplayName { get; }

  public UserId? UserId { get; }

  [JsonIgnore]
  public User? User { get; }

  [JsonConstructor]
  public Recipient(RecipientType type = RecipientType.To, Email? email = null, Phone? phone = null, DisplayName? displayName = null, UserId? userId = null)
  {
    Type = type;

    Email = email;
    Phone = phone;
    DisplayName = displayName;

    UserId = userId;

    new Validator().ValidateAndThrow(this);
  }

  public Recipient(User user, RecipientType type = RecipientType.To)
    : this(type, user.Email, user.Phone, GetDisplayName(user), user.Id)
  {
    User = user;
  }

  private static DisplayName GetDisplayName(User user) => new(user.FullName is null ? user.UniqueName.Value : user.FullName.Truncate(DisplayName.MaximumLength));

  private class Validator : AbstractValidator<Recipient>
  {
    public Validator()
    {
      RuleFor(x => x.Type).IsInEnum();

      RuleFor(x => x).Must(x => x.Email is not null || x.Phone is not null)
        .WithErrorCode("RecipientValidator")
        .WithMessage(x => $"At least one of the following must be specified: {nameof(x.Email)}, {nameof(x.Phone)}.");

      When(x => x.User is not null, () => RuleFor(x => x.UserId).NotNull());
    }
  }
}
