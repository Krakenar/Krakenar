using Krakenar.Core.Passwords.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords;

public class OneTimePassword : AggregateRoot
{
  public new OneTimePasswordId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public OneTimePassword() : base()
  {
  }

  public OneTimePassword(ActorId? actorId = null, OneTimePasswordId? oneTimePasswordId = null)
    : base((oneTimePasswordId ?? OneTimePasswordId.NewId()).StreamId)
  {
    Raise(new OneTimePasswordCreated(), actorId);
  }
  protected virtual void Handle(OneTimePasswordCreated @event)
  {
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new OneTimePasswordDeleted(), actorId);
    }
  }
}
