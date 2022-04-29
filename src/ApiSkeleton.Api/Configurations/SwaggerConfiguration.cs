using Blog.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Blog.Api.Configurations;

public static class SwaggerConfiguration
{
    public static void AddSwaggerDocs (this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.SubstituteApiVersionInUrl = true;
            options.GroupNameFormat = "'v'VVV";
        });
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddSwaggerGen();
    }


    public static void UseSwaggerDocs (this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            return;
        }

        app.UseSwagger();
        var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider!.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
    }
}

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions (IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure (string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    public void Configure (SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            options.OperationFilter<SwaggerUiFilter.RemoveVersionFromParameter>();
            options.DocumentFilter<SwaggerUiFilter.ReplaceVersionWithExactValueInPath>();
        }

        options.IncludeXmlComments(GetXmlDocumentation());
    }

    private static string GetXmlDocumentation ()
    {
        var xmlPath = "";
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.FullName is not null && x.FullName.Contains("Blog"));
        foreach (var assembly in assemblies)
        {
            var xmlFile = $"{assembly.GetName().Name}.xml";
            xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        }

        return xmlPath;
    }

    private static OpenApiInfo CreateInfoForApiVersion (ApiVersionDescription description)
    {
        var contact = new OpenApiContact
        {
            Name = "Iury Ferreira",
            Email = "iury.franklinferreira@gmail.com",
            Url = new Uri("https://github.com/iuryferreira")
        };
        var license = new OpenApiLicense
        {
            Name = "Apache 2.0", Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.txt")
        };
        return new OpenApiInfo
        {
            Title = "Blog.Api",
            Version = description.ApiVersion.ToString(),
            Description =
                !description.IsDeprecated
                    ? "Api desenvolvida para o Blog de Iury Ferreira."
                    : "Esta Api está depreciada. Use a versão mais atual",
            Contact = contact,
            License = license
        };
    }
}
