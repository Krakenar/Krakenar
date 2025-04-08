using Bogus;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class SessionTests
{
  private readonly Faker _faker = new();
  private readonly User _user;
  private readonly Session _session;

  public SessionTests()
  {
    _user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    _session = new Session(_user);
  }

  [Fact(DisplayName = "Delete: it should delete the session.")]
  public void Given_Session_When_Delete_Then_Deleted()
  {
    Assert.False(_session.IsDeleted);

    _session.Delete();
    Assert.True(_session.IsDeleted);
    Assert.Contains(_session.Changes, change => change is SessionDeleted);

    _session.ClearChanges();
    _session.Delete();
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Theory(DisplayName = "It should construct a new session from arguments.")]
  [InlineData(null, null, null, false)]
  [InlineData("c24fd223-5df7-4cb8-be57-1ae44bf3b5e7", "2bb42d55-c9fc-4013-98b1-a1fd699ca53d", "d8e67f9f-0d8d-45e2-9838-bc8cea33e9c3", true)]
  public void Given_Arguments_When_ctor_Then_Session(string? actorIdValue, string? realmIdValue, string? sessionIdValue, bool isPersistent)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    SessionId? sessionId = sessionIdValue is null ? null : new(Guid.Parse(sessionIdValue), realmId);

    Base64Password? secret = isPersistent ? new(Guid.NewGuid().ToString()) : null;
    Session session = new(_user, secret, actorId, sessionId);

    Assert.Equal(_user.Id, session.UserId);
    Assert.Equal(isPersistent, session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Equal(actorId, session.CreatedBy);
    Assert.Equal(actorId, session.UpdatedBy);

    Assert.Equal(realmId, session.RealmId);
    if (sessionId.HasValue)
    {
      Assert.Equal(sessionId, session.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, session.EntityId);
    }
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("IpAddress");
    _session.SetCustomAttribute(key, _faker.Internet.Ip());
    _session.Update();

    _session.RemoveCustomAttribute(key);
    _session.Update();
    Assert.False(_session.CustomAttributes.ContainsKey(key));
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.CustomAttributes[key] is null);

    _session.ClearChanges();
    _session.RemoveCustomAttribute(key);
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("IpAddress");
    _session.SetCustomAttribute(key, _faker.Internet.Ip());
    _session.Update();

    _session.SetCustomAttribute(key, value!);
    _session.Update();
    Assert.False(_session.CustomAttributes.ContainsKey(key));
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.CustomAttributes[key] is null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("IpAddress");
    string value = $"  {_faker.Internet.Ip()}  ";

    _session.SetCustomAttribute(key, value);
    _session.Update();
    Assert.Equal(_session.CustomAttributes[key], value.Trim());
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _session.ClearChanges();
    _session.SetCustomAttribute(key, value.Trim());
    _session.Update();
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Theory(DisplayName = "SignOut: it should sign-out an active session.")]
  [InlineData(null)]
  [InlineData("aae4fc86-7e21-4324-9f84-ac531a6b67e7")]
  public void Given_Session_When_SignOut_Then_SignedOut(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));

    _session.SignOut(actorId);
    Assert.False(_session.IsActive);
    Assert.Contains(_session.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == (actorId ?? new ActorId(_user.Id.Value)));

    _session.ClearChanges();
    _session.SignOut(actorId);
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }
}
