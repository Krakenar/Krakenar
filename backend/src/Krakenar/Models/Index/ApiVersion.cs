using Krakenar.Constants;

namespace Krakenar.Models.Index;

public record ApiVersion
{
  public static ApiVersion Current => new(Api.Title, Api.Version);

  public string Title { get; set; }
  public string Version { get; set; }

  public ApiVersion() : this(string.Empty, string.Empty)
  {
  }

  public ApiVersion(string title, Version version) : this(title, version.ToString())
  {
  }

  public ApiVersion(string title, string version)
  {
    Title = title;
    Version = version;
  }
}
