using Bogus;
using Krakenar.Client.Roles;
using Krakenar.Client.Users;
using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users;
using Logitar;
using Address = Krakenar.Core.Users.Address;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class UserClientE2ETests : E2ETests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public UserClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Users should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient roleClient = new();
    RoleClient roles = new(roleClient, KrakenarSettings);
    using HttpClient userClient = new();
    UserClient users = new(userClient, KrakenarSettings);

    CreateOrReplaceRolePayload createOrReplaceRole = new("editor");
    Role? editor = await roles.ReadAsync(id: null, createOrReplaceRole.UniqueName, _cancellationToken);
    if (editor is null)
    {
      CreateOrReplaceRoleResult roleResult = await roles.CreateOrReplaceAsync(createOrReplaceRole, id: null, version: null, _cancellationToken);
      editor = roleResult.Role;
      Assert.NotNull(editor);
    }

    Guid id = Guid.Parse("b4486f2a-56ca-4ec2-a1f9-45c22dc7a88f");
    UserDto? user = await users.ReadAsync(id, uniqueName: null, customIdentifier: null, _cancellationToken);

    CreateOrReplaceUserPayload createOrReplaceUser = new("admin");
    CreateOrReplaceUserResult userResult = await users.CreateOrReplaceAsync(createOrReplaceUser, id, version: null, _cancellationToken);
    Assert.Equal(userResult.Created, user is null);
    user = userResult.User;
    Assert.NotNull(user);
    Assert.Equal(id, user.Id);
    Assert.Equal(createOrReplaceUser.UniqueName, user.UniqueName);

    UpdateUserPayload enable = new()
    {
      IsDisabled = false
    };
    _ = await users.UpdateAsync(id, enable, _cancellationToken);

    using HttpClient contextualizedClient = new();
    KrakenarSettings.User = user.Id.ToString();
    UserClient contextualizedUsers = new(contextualizedClient, KrakenarSettings);
    UpdateUserPayload updateUser = new()
    {
      UniqueName = _faker.Person.UserName,
      Password = new ChangePasswordPayload("Test123!"),
      IsDisabled = true,
      Address = new Change<AddressPayload>(new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC")),
      Email = new Change<EmailPayload>(new EmailPayload(_faker.Person.Email, isVerified: true)),
      Phone = new Change<PhonePayload>(new PhonePayload("+15148454636", "CA", "123456", isVerified: true)),
      FirstName = new Change<string>(_faker.Person.FirstName),
      LastName = new Change<string>(_faker.Person.LastName),
      Birthdate = new Change<DateTime?>(_faker.Person.DateOfBirth),
      Gender = new Change<string>(_faker.Person.Gender.ToString()),
      Locale = new Change<string>(_faker.Locale),
      TimeZone = new Change<string>("America/Montreal"),
      Picture = new Change<string>(_faker.Person.Avatar),
      Profile = new Change<string>($"https://www.{_faker.Person.Website}/profile"),
      Website = new Change<string>($"https://www.{_faker.Person.Website}"),
      CustomAttributes = [new CustomAttribute("HealthInsuranceNumber", _faker.Person.BuildHealthInsuranceNumber())],
      Roles = [new RoleChange(editor.UniqueName, CollectionAction.Add)]
    };
    user = await contextualizedUsers.UpdateAsync(id, updateUser, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(updateUser.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Equal(user.Id, user.PasswordChangedBy?.Id);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(DateTime.UtcNow, user.PasswordChangedOn.Value, TimeSpan.FromSeconds(1));
    Assert.Equal(updateUser.IsDisabled, user.IsDisabled);
    Assert.Equal(user.Id, user.DisabledBy?.Id);
    Assert.NotNull(user.DisabledOn);
    Assert.Equal(DateTime.UtcNow, user.DisabledOn.Value, TimeSpan.FromSeconds(1));

    Assert.NotNull(updateUser.Address.Value);
    Assert.NotNull(user.Address);
    Assert.Equal(updateUser.Address.Value.Street, user.Address.Street);
    Assert.Equal(updateUser.Address.Value.Locality, user.Address.Locality);
    Assert.Equal(updateUser.Address.Value.PostalCode, user.Address.PostalCode);
    Assert.Equal(updateUser.Address.Value.Region, user.Address.Region);
    Assert.Equal(updateUser.Address.Value.Country, user.Address.Country);
    Assert.Equal(new Address(user.Address).ToString().Remove("\r"), user.Address.Formatted);
    Assert.False(user.Address.IsVerified);
    Assert.Null(user.Address.VerifiedBy);
    Assert.Null(user.Address.VerifiedOn);

    Assert.NotNull(updateUser.Email.Value);
    Assert.NotNull(user.Email);
    Assert.Equal(updateUser.Email.Value.Address, user.Email.Address);
    Assert.True(user.Email.IsVerified);
    Assert.Equal(user.Id, user.Email.VerifiedBy?.Id);
    Assert.NotNull(user.Email.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Email.VerifiedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(1));

    Assert.NotNull(updateUser.Phone.Value);
    Assert.NotNull(user.Phone);
    Assert.Equal(updateUser.Phone.Value.CountryCode, user.Phone.CountryCode);
    Assert.Equal(updateUser.Phone.Value.Number, user.Phone.Number);
    Assert.Equal(updateUser.Phone.Value.Extension, user.Phone.Extension);
    Assert.Equal(user.Phone.FormatToE164(), user.Phone.E164Formatted);
    Assert.True(user.Phone.IsVerified);
    Assert.Equal(user.Id, user.Phone.VerifiedBy?.Id);
    Assert.NotNull(user.Phone.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Phone.VerifiedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(1));

    Assert.True(user.IsConfirmed);
    Assert.Equal(updateUser.FirstName.Value, user.FirstName);
    Assert.Equal(updateUser.LastName.Value, user.LastName);
    Assert.NotNull(updateUser.Birthdate.Value);
    Assert.NotNull(user.Birthdate);
    Assert.Equal(updateUser.Birthdate.Value.Value.AsUniversalTime(), user.Birthdate.Value.AsUniversalTime(), TimeSpan.FromSeconds(1));
    Assert.Equal(updateUser.Gender.Value?.ToLowerInvariant(), user.Gender);
    Assert.Equal(updateUser.Locale.Value, user.Locale?.Code);
    Assert.Equal(updateUser.TimeZone.Value, user.TimeZone);
    Assert.Equal(updateUser.Picture.Value, user.Picture);
    Assert.Equal(updateUser.Profile.Value, user.Profile);
    Assert.Equal(updateUser.Website.Value, user.Website);
    Assert.Equal(updateUser.CustomAttributes, user.CustomAttributes);
    Assert.Equal(editor, Assert.Single(user.Roles));

    Assert.NotNull(await roles.DeleteAsync(editor.Id, _cancellationToken));

    user = await users.ReadAsync(id: null, user.UniqueName, customIdentifier: null, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(id, user.Id);
    Assert.Empty(user.Roles);

    string key = "Google";
    SaveUserIdentifierPayload saveUserIdentifier = new("1234567890");
    user = await users.SaveIdentifierAsync(user.Id, key, saveUserIdentifier, _cancellationToken);
    Assert.NotNull(user);
    Assert.Single(user.CustomIdentifiers);
    Assert.Contains(user.CustomIdentifiers, i => i.Key == key && i.Value == saveUserIdentifier.Value);

    user = await users.ReadAsync(id: null, uniqueName: null, new CustomIdentifier(key, saveUserIdentifier.Value), _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(id, user.Id);

    user = await users.RemoveIdentifierAsync(user.Id, key, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(id, user.Id);
    Assert.Empty(user.CustomIdentifiers);

    _ = await users.UpdateAsync(id, enable, _cancellationToken);

    ResetUserPasswordPayload resetUserPassword = new(PasswordString);
    user = await users.ResetPasswordAsync(user.Id, resetUserPassword, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(id, user.Id);

    AuthenticateUserPayload authenticateUser = new(_faker.Person.Email, PasswordString);
    user = await users.AuthenticateAsync(authenticateUser, _cancellationToken);
    Assert.Equal(id, user.Id);
    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(1));

    SearchUsersPayload searchUsers = new()
    {
      HasPassword = true,
      IsDisabled = false,
      IsConfirmed = true,
      HasAuthenticated = true,
      Ids = [user.Id]
      //Search = new TextSearch([new SearchTerm("%+15148454636%")]) // TODO(fpion): fix later
    };
    SearchResults<UserDto> results = await users.SearchAsync(searchUsers, _cancellationToken);
    Assert.Equal(1, results.Total);
    user = Assert.Single(results.Items);
    Assert.Equal(id, user.Id);
  }
}
