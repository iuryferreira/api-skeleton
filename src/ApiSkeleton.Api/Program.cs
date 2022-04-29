using Blog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.AddApiConfiguration();
var app = builder.Build();
app.UseApiConfiguration();
await app.RunAsync();
