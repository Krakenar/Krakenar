using Krakenar.Contracts.Constants;
using Krakenar.Web.Settings;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TemplateContent = Krakenar.Contracts.Templates.Content;

namespace Krakenar.Extensions;

internal static class SwaggerExtensions
{
  public static IServiceCollection AddKrakenarSwagger(this IServiceCollection services, AdminSettings settings)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.AddSecurity();
      config.MapType<TemplateContent>(() => new OpenApiSchema
      {
        Type = "object",
        Title = "TemplateContent"
      });
      config.OperationFilterDescriptors.Add(new FilterDescriptor
      {
        Arguments = [],
        Type = typeof(AddHeaderParameters)
      });
      config.SwaggerDoc(name: $"v{settings.Version.Major}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Francis Pion",
          Url = new Uri("https://www.francispion.ca/", UriKind.Absolute)
        },
        Description = "Krakenar is a tool suite aiming at handling non-business software requirements, allowing developers to focus on real, domain business requirements.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Krakenar/Krakenar/blob/main/LICENSE", UriKind.Absolute)
        },
        Title = settings.Title,
        Version = $"v{settings.Version}"
      });
    });
    services.AddTransient<IProblemDetailsWriter, PlainTextProblemDetailsWriter>();

    return services;
  }

  public static void UseKrakenarSwagger(this IApplicationBuilder builder, AdminSettings settings)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{settings.Version.Major}/swagger.json",
      name: $"{settings.Title} v{settings.Version}"
    ));
  }

  private static void AddSecurity(this SwaggerGenOptions options)
  {
    options.AddSecurityDefinition(Schemes.ApiKey, new OpenApiSecurityScheme
    {
      Description = "Enter your API key in the input below:",
      In = ParameterLocation.Header,
      Name = Headers.ApiKey,
      Scheme = Schemes.ApiKey,
      Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = Headers.ApiKey,
          Reference = new OpenApiReference
          {
            Id = Schemes.ApiKey,
            Type = ReferenceType.SecurityScheme
          },
          Scheme = Schemes.ApiKey,
          Type = SecuritySchemeType.ApiKey
        },
        new List<string>()
      }
    });

    options.AddSecurityDefinition(Schemes.Basic, new OpenApiSecurityScheme
    {
      Description = "Enter your credentials in the inputs below:",
      In = ParameterLocation.Header,
      Name = Headers.Authorization,
      Scheme = Schemes.Basic,
      Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = Headers.Authorization,
          Reference = new OpenApiReference
          {
            Id = Schemes.Basic,
            Type = ReferenceType.SecurityScheme
          },
          Scheme = Schemes.Basic,
          Type = SecuritySchemeType.Http
        },
        new List<string>()
      }
    });

    options.AddSecurityDefinition(Schemes.Bearer, new OpenApiSecurityScheme
    {
      Description = "Enter your access token in the input below:",
      In = ParameterLocation.Header,
      Name = Headers.Authorization,
      Scheme = Schemes.Bearer,
      Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name =  Headers.Authorization,
          Reference = new OpenApiReference
          {
            Id = Schemes.Bearer,
            Type = ReferenceType.SecurityScheme
          },
          Scheme = Schemes.Bearer,
          Type = SecuritySchemeType.Http
        },
        new List<string>()
      }
    });
  }
}
