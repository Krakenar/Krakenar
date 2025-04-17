using Microsoft.Extensions.Configuration;

namespace Krakenar;

public abstract class E2ETests : IDisposable
{
  protected virtual IConfiguration Configuration { get; }
  protected virtual Uri BaseUri { get; }
  protected virtual HttpClient HttpClient { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; }

  protected E2ETests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    BaseUri = new Uri(Configuration.GetValue<string>("BaseUrl") ?? string.Empty, UriKind.Absolute);

    HttpClient = new HttpClient
    {
      BaseAddress = BaseUri
    };

    SerializerOptions = new();
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  }

  public void Dispose()
  {
    HttpClient.Dispose();
    GC.SuppressFinalize(this);
  }
}
