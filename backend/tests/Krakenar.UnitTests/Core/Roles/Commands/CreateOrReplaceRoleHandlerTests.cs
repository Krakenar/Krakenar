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
public class CreateOrReplaceRoleHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IRoleQuerier> _roleQuerier = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();
  private readonly Mock<IRoleService> _roleService = new();

  private readonly CreateOrReplaceRoleHandler _handler;

  public CreateOrReplaceRoleHandlerTests()
  {
    _handler = new(_applicationContext.Object, _roleQuerier.Object, _roleRepository.Object, _roleService.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
  }

  [Theory(DisplayName = "It should create a new role.")]
  [InlineData(null)]
  [InlineData("73a33b25-91cf-4d70-9868-19a42473aecb")]
  public async Task Given_NotExists_When_HandleAsync_Then_Created(string? idValue)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = " Administrator ",
      Description = "  This is the administration role.  "
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", bool.TrueString));

    RoleDto dto = new();
    _roleQuerier.Setup(x => x.ReadAsync(It.IsAny<Role>(), _cancellationToken)).ReturnsAsync(dto);

    Role? role = null;
    _roleService.Setup(x => x.SaveAsync(It.IsAny<Role>(), _cancellationToken)).Callback<Role, CancellationToken>((r, _) => role = r);

    CreateOrReplaceRole command = new(id, payload, Version: null);
    CreateOrReplaceRoleResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Role);
    Assert.Same(dto, result.Role);

    Assert.NotNull(role);
    Assert.Equal(actorId, role.CreatedBy);
    Assert.Equal(actorId, role.UpdatedBy);
    Assert.Equal(realmId, role.RealmId);
    Assert.Equal(payload.UniqueName, role.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), role.Description?.Value);
    Assert.Equal(payload.CustomAttributes.Count, role.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, role.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    if (id.HasValue)
    {
      Assert.Equal(id.Value, role.EntityId);

      _roleRepository.Verify(x => x.LoadAsync(It.Is<RoleId>(i => i.RealmId == realmId && i.EntityId == id.Value), _cancellationToken), Times.Once);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, role.EntityId);
    }
  }

  [Fact(DisplayName = "It should replace an existing role.")]
  public async Task Given_Found_When_HandleAsync_Then_Replaced()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Role role = new(new UniqueName(_uniqueNameSettings, "guest"), actorId);
    role.SetCustomAttribute(new Identifier("manage_api"), bool.FalseString);
    role.Update(actorId);
    _roleRepository.Setup(x => x.LoadAsync(role.Id, _cancellationToken)).ReturnsAsync(role);

    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = " Administrator ",
      Description = "  This is the administration role.  "
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", bool.TrueString));

    RoleDto dto = new();
    _roleQuerier.Setup(x => x.ReadAsync(role, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceRole command = new(role.EntityId, payload, Version: null);
    CreateOrReplaceRoleResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Role);
    Assert.Same(dto, result.Role);

    Assert.Equal(actorId, role.UpdatedBy);
    Assert.Equal(payload.UniqueName, role.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), role.Description?.Value);
    Assert.Equal(payload.CustomAttributes.Count, role.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, role.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    _roleRepository.Verify(x => x.LoadAsync(It.IsAny<RoleId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);
    _roleService.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the role does not exist.")]
  public async Task Given_NotFound_Then_HandleAsync_Then_NullReturned()
  {
    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "not_found"
    };
    CreateOrReplaceRole command = new(Guid.Empty, payload, Version: -1);
    CreateOrReplaceRoleResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.Role);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "invalid!",
      DisplayName = RandomStringGenerator.GetString(999)
    };
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", string.Empty));

    CreateOrReplaceRole command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }

  [Fact(DisplayName = "It should update an existing role.")]
  public async Task Given_Found_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Role role = new(new UniqueName(_uniqueNameSettings, "guest"), actorId);
    _roleRepository.Setup(x => x.LoadAsync(role.Id, _cancellationToken)).ReturnsAsync(role);

    Role reference = new(role.UniqueName, actorId, role.Id);
    _roleRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("  This is the administration role.  ");
    role.Description = description;
    role.Update(actorId);

    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = " Administrator "
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", bool.TrueString));

    RoleDto dto = new();
    _roleQuerier.Setup(x => x.ReadAsync(role, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceRole command = new(role.EntityId, payload, reference.Version);
    CreateOrReplaceRoleResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Role);
    Assert.Same(dto, result.Role);

    Assert.Equal(actorId, role.UpdatedBy);
    Assert.Equal(payload.UniqueName, role.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName?.Value);
    Assert.Equal(description, role.Description);
    Assert.Equal(payload.CustomAttributes.Count, role.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, role.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    _roleRepository.Verify(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken), Times.Once);
    _roleService.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }
}
