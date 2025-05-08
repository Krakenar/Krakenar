using Krakenar.Client;
using Krakenar.Client.Realms;
using Krakenar.Contracts.Realms;
using Microsoft.Extensions.Configuration;

namespace Krakenar;

public abstract class E2ETests : IDisposable
{
  protected virtual IConfiguration Configuration { get; }
  protected virtual KrakenarSettings KrakenarSettings { get; }
  protected virtual HttpClient HttpClient { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; }

  private Realm? _realm = null;
  protected virtual Realm Realm => _realm ?? throw new InvalidOperationException("The realm has not been initialized.");

  protected E2ETests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("3818b49c-60be-4493-b0cd-f436cc8e1aad")
      .Build();
    KrakenarSettings = KrakenarSettings.Initialize(Configuration);

    HttpClient = new HttpClient();
    if (!string.IsNullOrWhiteSpace(KrakenarSettings.BaseUrl))
    {
      HttpClient.BaseAddress = new Uri(KrakenarSettings.BaseUrl.Trim(), UriKind.Absolute);
    }

    SerializerOptions = new();
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  }

  public void Dispose()
  {
    HttpClient.Dispose();
    GC.SuppressFinalize(this);
  }

  protected virtual async Task InitializeRealmAsync(CancellationToken cancellationToken = default)
  {
    using HttpClient httpClient = new();
    RealmClient realmClient = new(httpClient, KrakenarSettings);

    Guid id = Guid.Parse("86f6c624-d99a-4d43-9298-386cbaee3e91");
    CreateOrReplaceRealmPayload payload = new("e2e-tests")
    {
      DisplayName = "End-to-End Tests",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    CreateOrReplaceRealmResult result = await realmClient.CreateOrReplaceAsync(payload, id, version: null, cancellationToken);
    Assert.NotNull(result.Realm);
    _realm = result.Realm;
  }
}
