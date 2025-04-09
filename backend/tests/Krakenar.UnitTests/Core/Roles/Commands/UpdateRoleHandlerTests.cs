using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateRoleHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IRoleQuerier> _roleQuerier = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();
  private readonly Mock<IRoleService> _roleService = new();

  private readonly UpdateRoleHandler _handler;

  private readonly UniqueNameSettings _uniqueNameSettings = new();

  public UpdateRoleHandlerTests()
  {
    _handler = new(_applicationContext.Object, _roleQuerier.Object, _roleRepository.Object, _roleService.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
  }

  [Fact(DisplayName = "It should return null when the role was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    UpdateRolePayload payload = new();
    UpdateRole command = new(Guid.Empty, payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateRolePayload payload = new()
    {
      UniqueName = "admin!",
      DisplayName = new Contracts.Change<string>(RandomStringGenerator.GetString(999))
    };
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", string.Empty));

    UpdateRole command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }

  [Fact(DisplayName = "It should update the role.")]
  public async Task Given_Role_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Role role = new(new UniqueName(_uniqueNameSettings, "adminn"), actorId, RoleId.NewId(realmId));
    role.SetCustomAttribute(new Identifier("manage_users"), bool.FalseString);
    role.SetCustomAttribute(new Identifier("all_permissions"), bool.TrueString);
    role.Update(actorId);
    _roleRepository.Setup(x => x.LoadAsync(role.Id, _cancellationToken)).ReturnsAsync(role);

    UpdateRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = new Contracts.Change<string>(" Administrator "),
      Description = new Contracts.Change<string>("  This is the administration role.  ")
    };
    payload.CustomAttributes.Add(new CustomAttribute("all_permissions", "   "));
    payload.CustomAttributes.Add(new CustomAttribute("manage_users", bool.TrueString));

    RoleDto dto = new();
    _roleQuerier.Setup(x => x.ReadAsync(role, _cancellationToken)).ReturnsAsync(dto);

    UpdateRole command = new(role.EntityId, payload);
    RoleDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _roleService.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);

    Assert.Equal(payload.UniqueName, role.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Value?.Trim(), role.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), role.Description?.Value);

    KeyValuePair<Identifier, string> customAttribute = Assert.Single(role.CustomAttributes);
    Assert.Equal("manage_users", customAttribute.Key.Value);
    Assert.Equal(bool.TrueString, customAttribute.Value);
  }
}
