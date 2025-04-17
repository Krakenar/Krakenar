using Logitar;

namespace Krakenar.Client;

public class InvalidApiResponseException : Exception
{
  private const string ErrorMessage = "An API response was expected, but it was null after deserialization.";

  public string ClientType
  {
    get => (string)Data[nameof(ClientType)]!;
    private set => Data[nameof(ClientType)] = value;
  }
  public string MethodName
  {
    get => (string)Data[nameof(MethodName)]!;
    private set => Data[nameof(MethodName)] = value;
  }
  public string HttpMethod
  {
    get => (string)Data[nameof(HttpMethod)]!;
    private set => Data[nameof(HttpMethod)] = value;
  }
  public string RequestUrl
  {
    get => (string)Data[nameof(RequestUrl)]!;
    private set => Data[nameof(RequestUrl)] = value;
  }
  public string? Content
  {
    get => (string?)Data[nameof(Content)];
    private set => Data[nameof(Content)] = value;
  }

  public InvalidApiResponseException(Type clientType, string methodName, HttpMethod httpMethod, Uri requestUri, string? content)
    : base(BuildMessage(clientType, methodName, httpMethod, requestUri, content))
  {
    ClientType = clientType.GetNamespaceQualifiedName();
    MethodName = methodName;
    HttpMethod = httpMethod.Method;
    RequestUrl = requestUri.ToString();
    Content = content;
  }

  private static string BuildMessage(Type clientType, string methodName, HttpMethod httpMethod, Uri uri, string? content) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ClientType), clientType.GetNamespaceQualifiedName())
    .AddData(nameof(MethodName), methodName)
    .AddData(nameof(HttpMethod), httpMethod)
    .AddData(nameof(RequestUrl), uri)
    .AddData(nameof(Content), content, "<null>")
    .Build();
}
