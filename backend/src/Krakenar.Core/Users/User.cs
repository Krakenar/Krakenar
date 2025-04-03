using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users;

public class User : AggregateRoot
{
  public const string EntityType = "User";

  private UserUpdated _updated = new();
  private bool HasUpdates => _updated.Locale is not null;

  public new UserId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The user has not been initialized.");

  private Password? _password = null;
  public bool HasPassword => _password is not null;

  public bool IsDisabled { get; }

  public string? FullName { get; private set; }

  private Locale? _locale = null;
  public Locale? Locale
  {
    get => _locale;
    set
    {
      if (_locale != value)
      {
        _locale = value;
        _updated.Locale = new Change<Locale>(value);
      }
    }
  }

  public User(UniqueName uniqueName, Password? password = null, ActorId? actorId = null, UserId? userId = null) : base(userId?.StreamId)
  {
    Raise(new UserCreated(uniqueName, password), actorId);
  }
  protected virtual void Handle(UserCreated @event)
  {
    _uniqueName = @event.UniqueName;

    _password = @event.Password;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new UserUpdated();
    }
  }
  protected virtual void Handle(UserUpdated @event)
  {
    if (@event.Locale is not null)
    {
      _locale = @event.Locale.Value;
    }
  }

  public override string ToString() => $"{FullName ?? UniqueName.Value} | {base.ToString()}";
}
