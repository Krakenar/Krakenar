using Krakenar.Contracts;
using System.Text;
using System.Text.Json;

namespace Krakenar.Client;

public class KrakenarClientException : ErrorException
{
  private const string ErrorMessage = "An unexpected exception occurred from the Krakenar client.";

  public ApiResult Result
  {
    get => (ApiResult)Data[nameof(Result)]!;
    private set => Data[nameof(Result)] = value;
  }
  public ProblemDetails? ProblemDetails
  {
    get => (ProblemDetails?)Data[nameof(ProblemDetails)];
    private set => Data[nameof(ProblemDetails)] = value;
  }

  public override Error Error
  {
    get
    {
      Error? error = ProblemDetails?.Error;
      if (error is null)
      {
        error = new Error("KrakenarClient", ErrorMessage);
        error.Data[nameof(Result)] = Result;
        error.Data[nameof(ProblemDetails)] = ProblemDetails;
      }
      return error;
    }
  }

  public KrakenarClientException(ApiResult result, ProblemDetails? problemDetails = null) : base(BuildMessage(result, problemDetails))
  {
    Result = result;
    ProblemDetails = problemDetails;
  }

  private static string BuildMessage(ApiResult result, ProblemDetails? problemDetails)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(Result)).Append(": ").AppendLine(JsonSerializer.Serialize(result));
    message.Append(nameof(ProblemDetails)).Append(": ").AppendLine(problemDetails is null ? "<null>" : JsonSerializer.Serialize(problemDetails));
    return message.ToString();
  }
}
