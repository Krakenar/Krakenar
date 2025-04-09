using Krakenar.Core.Realms;
using Krakenar.Core.Roles.Events;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Roles;

[Trait(Traits.Category, Categories.Unit)]
public class RoleTests
{
  private readonly UniqueNameSettings _uniqueNameSettings = new();
  private readonly Role _role;

  public RoleTests()
  {
    _role = new(new UniqueName(_uniqueNameSettings, "admin"));
  }

  [Fact(DisplayName = "Delete: it should delete the role.")]
  public void Given_Role_When_Delete_Then_Deleted()
  {
    Assert.False(_role.IsDeleted);

    _role.Delete();
    Assert.True(_role.IsDeleted);
    Assert.Contains(_role.Changes, change => change is RoleDeleted);

    _role.ClearChanges();
    _role.Delete();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Theory(DisplayName = "It should construct a new role from arguments.")]
  [InlineData(null, null, null)]
  [InlineData("c24fd223-5df7-4cb8-be57-1ae44bf3b5e7", "2bb42d55-c9fc-4013-98b1-a1fd699ca53d", "d8e67f9f-0d8d-45e2-9838-bc8cea33e9c3")]
  public void Given_Arguments_When_ctor_Then_Role(string? actorIdValue, string? realmIdValue, string? roleIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    RoleId? roleId = roleIdValue is null ? null : new(Guid.Parse(roleIdValue), realmId);

    Role role = new(_role.UniqueName, actorId, roleId);

    Assert.Equal(_role.UniqueName, role.UniqueName);
    Assert.Equal(actorId, role.CreatedBy);
    Assert.Equal(actorId, role.UpdatedBy);

    Assert.Equal(realmId, role.RealmId);
    if (roleId.HasValue)
    {
      Assert.Equal(roleId, role.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, role.EntityId);
    }
  }

  [Fact(DisplayName = "It should handle Description change correctly.")]
  public void Given_Changes_When_Description_Then_Changed()
  {
    Description description = new("  This is the administration role.  ");
    _role.Description = description;
    _role.Update(_role.CreatedBy);
    Assert.Equal(description, _role.Description);
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.Description?.Value is not null && updated.Description.Value.Equals(description));

    _role.ClearChanges();

    _role.Description = description;
    _role.Update();
    Assert.False(_role.HasChanges);
  }

  [Fact(DisplayName = "It should handle DisplayName change correctly.")]
  public void Given_Changes_When_DisplayName_Then_Changed()
  {
    DisplayName displayName = new(" Administrator ");
    _role.DisplayName = displayName;
    _role.Update(_role.CreatedBy);
    Assert.Equal(displayName, _role.DisplayName);
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.DisplayName?.Value is not null && updated.DisplayName.Value.Equals(displayName));

    _role.ClearChanges();

    _role.DisplayName = displayName;
    _role.Update();
    Assert.False(_role.HasChanges);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("CanViewContents");
    _role.SetCustomAttribute(key, bool.TrueString);
    _role.Update();

    _role.RemoveCustomAttribute(key);
    _role.Update();
    Assert.False(_role.CustomAttributes.ContainsKey(key));
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.CustomAttributes[key] is null);

    _role.ClearChanges();
    _role.RemoveCustomAttribute(key);
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("CanViewContents");
    _role.SetCustomAttribute(key, bool.TrueString);
    _role.Update();

    _role.SetCustomAttribute(key, value!);
    _role.Update();
    Assert.False(_role.CustomAttributes.ContainsKey(key));
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.CustomAttributes[key] is null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("CanViewContents");
    string value = $"  {bool.TrueString}  ";

    _role.SetCustomAttribute(key, value);
    _role.Update();
    Assert.Equal(_role.CustomAttributes[key], value.Trim());
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _role.ClearChanges();
    _role.SetCustomAttribute(key, value.Trim());
    _role.Update();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle changes correctly.")]
  public void Given_Changes_When_SetUniqueName_Then_Changed()
  {
    _role.ClearChanges();
    _role.SetUniqueName(_role.UniqueName);
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);

    UniqueName uniqueName = new(_uniqueNameSettings, "guest");
    _role.SetUniqueName(uniqueName);
    Assert.Equal(uniqueName, _role.UniqueName);
    Assert.Contains(_role.Changes, change => change is RoleUniqueNameChanged changed && changed.UniqueName.Equals(uniqueName));
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Role_When_ToString_Then_CorrectString()
  {
    Assert.StartsWith(_role.UniqueName.Value, _role.ToString());

    _role.DisplayName = new DisplayName("Administrator");
    Assert.StartsWith(_role.DisplayName.Value, _role.ToString());
  }
}
