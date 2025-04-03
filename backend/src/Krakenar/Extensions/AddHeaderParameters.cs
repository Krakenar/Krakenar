using Krakenar.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Krakenar.Extensions;

internal class AddHeaderParameters : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    operation.Parameters.Add(new OpenApiParameter
    {
      In = ParameterLocation.Header,
      Name = Headers.Realm,
      Description = "Enter your realm ID or unique slug in the input below:"
    });
    operation.Parameters.Add(new OpenApiParameter
    {
      In = ParameterLocation.Header,
      Name = Headers.User,
      Description = "Enter your user ID or unique name/email in the input below:"
    });
  }
}
