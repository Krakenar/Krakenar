using Bogus;
using FluentValidation.Results;
using Krakenar.Client;
using Krakenar.Client.Users;
using Krakenar.Contracts;
using Krakenar.Contracts.Users;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ExceptionHandlerE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public ExceptionHandlerE2ETests() : base()
  {
  }

  [Fact(DisplayName = "It should handle 400 Bad Request errors correctly.")]
  public async Task Given_ExceptionHandler_When_ValidationException_Then_400BadRequest()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.Id.ToString();
    UserClient users = new(httpClient, KrakenarSettings);

    CreateOrReplaceUserPayload payload = new("invalid!")
    {
      Password = new ChangePasswordPayload("AAaa!!11")
    };
    var exception = await Assert.ThrowsAsync<KrakenarClientException>(
      async () => await users.CreateOrReplaceAsync(payload, id: null, version: null, _cancellationToken));

    ApiResult result = exception.Result;
    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    Assert.False(result.IsSuccessStatusCode);
    Assert.Equal("Bad Request", result.ReasonPhrase);
    Assert.NotNull(result.Content);

    ProblemDetails? problemDetails = exception.ProblemDetails;
    Assert.NotNull(problemDetails);
    Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.1", problemDetails.Type);
    Assert.Equal("Validation", problemDetails.Title);
    Assert.Equal((int)result.StatusCode, problemDetails.Status);
    Assert.Equal("Validation failed.", problemDetails.Detail);
    Assert.EndsWith("/api/users", problemDetails.Instance);

    Error? error = problemDetails.Error;
    Assert.NotNull(error);
    Assert.Equal(problemDetails.Title, error.Code);
    Assert.Equal(problemDetails.Detail, error.Message);
    Assert.Single(error.Data);
    object? value = error.Data["Failures"];
    Assert.NotNull(value);
    JsonElement element = (JsonElement)value;
    IReadOnlyCollection<ValidationFailure>? failures = element.Deserialize<IReadOnlyCollection<ValidationFailure>>(SerializerOptions);
    Assert.NotNull(failures);
    Assert.Equal(2, failures.Count);
    Assert.Contains(failures, f => f.ErrorCode == "AllowedCharactersValidator" && f.PropertyName == "UniqueName");
    Assert.Contains(failures, f => f.ErrorCode == "PasswordRequiresUniqueChars" && f.PropertyName == "Password.New");
  }

  [Fact(DisplayName = "It should handle 404 Not Found errors correctly.")]
  public async Task Given_ExceptionHandler_When_RolesNotFoundException_Then_404NotFound()
  {
    using HttpClient httpClient = new();
    UserClient users = new(httpClient, KrakenarSettings);

    CreateOrReplaceUserPayload payload = new(_faker.Person.UserName)
    {
      Roles = ["  not_found  ", "a27928fb-a19c-47c8-8689-14046bfcd0fc"]
    };
    var exception = await Assert.ThrowsAsync<KrakenarClientException>(
      async () => await users.CreateOrReplaceAsync(payload, id: null, version: null, _cancellationToken));

    ApiResult result = exception.Result;
    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    Assert.False(result.IsSuccessStatusCode);
    Assert.Equal("Not Found", result.ReasonPhrase);
    Assert.NotNull(result.Content);

    ProblemDetails? problemDetails = exception.ProblemDetails;
    Assert.NotNull(problemDetails);
    Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.5", problemDetails.Type);
    Assert.Equal("Roles Not Found", problemDetails.Title);
    Assert.Equal((int)result.StatusCode, problemDetails.Status);
    Assert.Equal("The specified roles could not be found.", problemDetails.Detail);
    Assert.EndsWith("/api/users", problemDetails.Instance);

    Error? error = problemDetails.Error;
    Assert.NotNull(error);
    Assert.Equal("RolesNotFound", error.Code);
    Assert.Equal(problemDetails.Detail, error.Message);
    Assert.Equal(3, error.Data.Count);
    Assert.Null(error.Data["RealmId"]);
    Assert.Equal("Roles", ((JsonElement)error.Data["PropertyName"]!).GetString());

    object? value = error.Data["Roles"];
    Assert.NotNull(value);
    JsonElement element = (JsonElement)value;
    IReadOnlyCollection<string>? roles = element.Deserialize<IReadOnlyCollection<string>>(SerializerOptions);
    Assert.NotNull(roles);
    Assert.Equal(payload.Roles, roles);
  }

  [Fact(DisplayName = "It should handle 409 Conflict errors correctly.")]
  public async Task Given_ExceptionHandler_When_UniqueNameAlreadyUsedException_Then_409Conflict()
  {
    using HttpClient httpClient = new();
    UserClient users = new(httpClient, KrakenarSettings);

    CreateOrReplaceUserPayload payload = new("admin");
    var exception = await Assert.ThrowsAsync<KrakenarClientException>(
      async () => await users.CreateOrReplaceAsync(payload, id: null, version: null, _cancellationToken));

    ApiResult result = exception.Result;
    Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
    Assert.False(result.IsSuccessStatusCode);
    Assert.Equal("Conflict", result.ReasonPhrase);
    Assert.NotNull(result.Content);

    ProblemDetails? problemDetails = exception.ProblemDetails;
    Assert.NotNull(problemDetails);
    Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.10", problemDetails.Type);
    Assert.Equal("Unique Name Already Used", problemDetails.Title);
    Assert.Equal((int)result.StatusCode, problemDetails.Status);
    Assert.Equal("The specified unique name is already used.", problemDetails.Detail);
    Assert.EndsWith("/api/users", problemDetails.Instance);

    Error? error = problemDetails.Error;
    Assert.NotNull(error);
    Assert.Equal("UniqueNameAlreadyUsed", error.Code);
    Assert.Equal(problemDetails.Detail, error.Message);
    Assert.Equal(6, error.Data.Count);
    Assert.Null(error.Data["RealmId"]);
    Assert.Equal("User", ((JsonElement)error.Data["EntityType"]!).GetString());
    Assert.NotNull(error.Data["EntityId"]);
    Assert.NotNull(error.Data["ConflictId"]);
    Assert.Equal(payload.UniqueName, ((JsonElement)error.Data["UniqueName"]!).GetString());
    Assert.Equal("UniqueName", ((JsonElement)error.Data["PropertyName"]!).GetString());
  }
}
