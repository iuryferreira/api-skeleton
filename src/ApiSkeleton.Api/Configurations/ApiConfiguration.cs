using System.Text.RegularExpressions;
using Blog.Api.Filters;
using Blog.Core.All;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Blog.Api.Configurations;

public static class ApiConfiguration
{
    public static void AddApiConfiguration (this WebApplicationBuilder builder)
    {
        Console.WriteLine("Development " + builder.Environment.IsDevelopment());
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddCacheConfiguration();
        builder.AddCorsConfiguration();
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            options.Filters.Add(new ExceptionFilter());
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocs();
        builder.Services.AddCore(builder.Configuration);
    }

    public static void UseApiConfiguration (this WebApplication app)
    {
        app.UseSwaggerDocs(app.Environment);
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCorsConfiguration();
        app.UseAuthorization();
        app.MapControllers();
    }
}

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound (object? value)
    {
        if (value is not null)
        {
            value = Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2").ToLower();
        }

        return (string?)value;
    }
}
