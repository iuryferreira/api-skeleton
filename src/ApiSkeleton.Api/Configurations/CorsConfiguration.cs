namespace Blog.Api.Configurations;

public static class CorsConfiguration
{
    private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static void AddCorsConfiguration (this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(MyAllowSpecificOrigins, policyBuilder =>
            {
                var urls = builder.Configuration.GetValue<string>("ClientUrl") ?? "localhost:5002";
                policyBuilder.WithOrigins(urls).AllowAnyHeader().AllowAnyMethod();
            });
        });
    }

    public static void UseCorsConfiguration (this IApplicationBuilder app)
    {
        app.UseCors(MyAllowSpecificOrigins);
    }
}
