using Krakenar.Contracts.Messages;
using Krakenar.Core.Users;
using RecipientCore = Krakenar.Core.Messages.Recipient;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Recipient
{
  public int RecipientId { get; private set; }
  public Guid Id { get; private set; }

  public Message? Message { get; private set; }
  public int MessageId { get; private set; }

  public RecipientType Type { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? PhoneCountryCode { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; private set; }
  public string? PhoneE164Formatted { get; private set; }
  public string? DisplayName { get; private set; }

  public User? User { get; private set; }
  public int? UserId { get; private set; }
  public Guid? UserUid { get; private set; }
  public string? UserUniqueName { get; private set; }
  public string? UserFullName { get; private set; }
  public string? UserPicture { get; private set; }

  public Recipient(Message message, RecipientCore recipient, User? user = null)
  {
    Id = Guid.NewGuid();

    Message = message;
    MessageId = message.MessageId;

    Type = recipient.Type;
    EmailAddress = recipient.Email?.Address;
    if (recipient.Phone is not null)
    {
      PhoneCountryCode = recipient.Phone.CountryCode;
      PhoneNumber = recipient.Phone.Number;
      PhoneExtension = recipient.Phone.Extension;
      PhoneE164Formatted = recipient.Phone.FormatToE164();
    }
    DisplayName = recipient.DisplayName?.Value;

    if (user is not null)
    {
      User = user;
      UserId = user.UserId;
      UserUid = user.Id;
      UserUniqueName = user.UniqueName;
      UserFullName = user.FullName;
      UserPicture = user.Picture;
    }
  }

  private Recipient()
  {
  }

  public override bool Equals(object? obj) => obj is Recipient recipient && recipient.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    List<string> parts = new(capacity: 3);
    if (DisplayName is not null)
    {
      parts.Add($"{DisplayName} <{EmailAddress ?? PhoneE164Formatted}>");
    }
    else if (EmailAddress is not null)
    {
      parts.Add(EmailAddress);
    }
    else if (PhoneE164Formatted is not null)
    {
      parts.Add(PhoneE164Formatted);
    }
    if (EmailAddress is not null && PhoneE164Formatted is not null)
    {
      parts.Add(PhoneE164Formatted);
    }
    parts.Add($"{GetType()} (Id={Id})");
    return string.Join(" | ", parts);
  }
}
