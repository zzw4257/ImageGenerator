using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// A helper class for serving static files with custom configurations.
/// </summary>
public static class StaticFileHelper
{
    /// <summary>
    /// Adds a custom static file server to the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="path">The physical path to the static files.</param>
    /// <param name="requestPath">The request path for the static files.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder AddCustomStaticFile(this IApplicationBuilder app, string path, string requestPath)
    {
        // Ensure directory exists
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(path),
            RequestPath = requestPath
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(path),
            RequestPath = requestPath,
            ContentTypeProvider = CustomFileTypeProvider(),
        });

        return app;
    }

    /// <summary>
    /// Creates a custom file extension content type provider.
    /// </summary>
    /// <returns>The custom content type provider.</returns>
    private static FileExtensionContentTypeProvider CustomFileTypeProvider()
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".mp3"] = "audio/mpeg";
        provider.Mappings[".m4a"] = "audio/mp4";
        provider.Mappings[".flac"] = "audio/flac";
        provider.Mappings[".wav"] = "audio/wav";
        return provider;
    }
}
