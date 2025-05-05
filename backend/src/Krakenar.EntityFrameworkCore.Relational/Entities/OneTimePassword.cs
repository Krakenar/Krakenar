using Krakenar.Core.Passwords;
using Krakenar.Core.Passwords.Events;
using Logitar;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class OneTimePassword : Aggregate, ISegregatedEntity
{
  public int OneTimePasswordId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public User? User { get; private set; }
  public int? UserId { get; private set; }

  public Guid Id { get; private set; }

  public string PasswordHash { get; private set; } = string.Empty;

  public DateTime? ExpiresOn { get; private set; }
  public int? MaximumAttempts { get; private set; }

  public int AttemptCount { get; private set; }
  public DateTime? ValidationSucceededOn { get; private set; }

  public string? CustomAttributes { get; private set; }

  public OneTimePassword(Realm? realm, OneTimePasswordCreated @event) : this(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;
  }
  public OneTimePassword(User user, OneTimePasswordCreated @event) : this(user.Realm, @event)
  {
    User = user;
    UserId = user.UserId;
  }
  private OneTimePassword(OneTimePasswordCreated @event) : base(@event)
  {
    Id = new OneTimePasswordId(@event.StreamId).EntityId;

    PasswordHash = @event.Password.Encode();

    ExpiresOn = @event.ExpiresOn?.AsUniversalTime();
    MaximumAttempts = @event.MaximumAttempts;
  }

  private OneTimePassword() : base()
  {
  }

  public void Fail(OneTimePasswordValidationFailed @event)
  {
    Update(@event);

    AttemptCount++;
  }

  public void Succeed(OneTimePasswordValidationSucceeded @event)
  {
    Update(@event);

    AttemptCount++;
    ValidationSucceededOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Update(OneTimePasswordUpdated @event)
  {
    base.Update(@event);

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Core.Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value is null)
      {
        customAttributes.Remove(customAttribute.Key.Value);
      }
      else
      {
        customAttributes[customAttribute.Key.Value] = customAttribute.Value;
      }
    }
    SetCustomAttributes(customAttributes);
  }

  public Dictionary<string, string> GetCustomAttributes()
  {
    return (CustomAttributes is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }
}
