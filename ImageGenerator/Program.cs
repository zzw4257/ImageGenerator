using ImageGenerator.Helpers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    string openApiUrl = "/openapi/ig.json";
    app.MapOpenApi(openApiUrl);
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern(openApiUrl);
    });
}

app.UseHttpsRedirection();

app.AddCustomStaticFile(Path.Combine(Directory.GetCurrentDirectory(), "images"), "/images");

app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();

app.MapControllers();

app.Run();
