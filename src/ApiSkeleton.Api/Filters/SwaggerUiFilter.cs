using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Blog.Api.Filters;

public static class SwaggerUiFilter
{
    [UsedImplicitly]
    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply (OpenApiOperation operation, OperationFilterContext context)
        {
            if (!operation.Parameters.Any())
                return;
            var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "version");
            if (versionParameter != null) operation.Parameters.Remove(versionParameter);
        }
    }

    [UsedImplicitly]
    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply (OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();
            foreach (var (key, value) in swaggerDoc.Paths)
            {
                paths.Add(key.Replace("{version}", swaggerDoc.Info.Version.ElementAt(0).ToString()), value);
            }

            swaggerDoc.Paths = paths;
        }
    }
}
