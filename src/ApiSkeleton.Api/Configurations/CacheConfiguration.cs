namespace Blog.Api.Configurations;

public static class CacheConfiguration
{
    public static void AddCacheConfiguration (this IServiceCollection services)
    {
        services.AddMemoryCache();
    }
}
