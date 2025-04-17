using Krakenar.Contracts;
using Logitar;

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
        error = new Error(this.GetErrorCode(), ErrorMessage);
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

  private static string BuildMessage(ApiResult result, ProblemDetails? problemDetails) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Result), JsonSerializer.Serialize(result))
    .AddData(nameof(ProblemDetails), problemDetails is null ? "<null>" : JsonSerializer.Serialize(problemDetails))
    .Build();
}
