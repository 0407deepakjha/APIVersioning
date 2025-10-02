using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace APIVersioning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers with JSON options (disable camelCase)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            // Add API versioning configuration
            builder.Services.AddApiVersioning(options =>
            {
                // This allows the API to use a default version if the client does not specify one.
                options.AssumeDefaultVersionWhenUnspecified = true;

                // This sets the default version (here, 1.0).
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // This adds headers in the response informing clients about all the supported API versions.
                options.ReportApiVersions = true;

                // This tells the framework to use the query string parameter api-version for versioning
                options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
            });

            // Add Swagger generation and configure multiple version docs
            builder.Services.AddSwaggerGen(options =>
            {
                // Define a Swagger document for API version 1.0
                // This generates a separate swagger.json with metadata for version 1.0 endpoints
                options.SwaggerDoc("1.0", new OpenApiInfo
                {
                    Title = "API Version", // Display title of the API
                    Version = "1.0"        // The API version string shown in Swagger UI
                });

                // Define a Swagger document for API version 2.0
                // Similarly, this creates swagger.json for version 2.0 endpoints
                options.SwaggerDoc("2.0", new OpenApiInfo
                {
                    Title = "API Version",
                    Version = "2.0"
                });

                // In case of route conflicts (when multiple actions match the same path and HTTP method),
                // this resolves the conflict by selecting the first matching action.
                // This prevents Swagger generation errors caused by duplicate routes.
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                // DocInclusionPredicate controls which endpoints get included in each Swagger document.
                // It receives the document version (string) and the ApiDescription for an endpoint.
                // Return true to include the endpoint in the current document, false to exclude it.
                options.DocInclusionPredicate((version, apiDesc) =>
                {
                    // Try to get MethodInfo for the endpoint, if unavailable exclude it from docs.
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo method))
                        return false;

                    // Extract ApiVersion attributes applied on the action method.
                    var methodVersions = method.GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()         // Only consider [ApiVersion] attributes
                        .SelectMany(attr => attr.Versions);    // Extract all versions declared

                    // Extract ApiVersion attributes applied on the controller class.
                    var controllerVersions = method.DeclaringType?
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions) ?? Enumerable.Empty<ApiVersion>();

                    // Combine versions declared at both method and controller levels,
                    // to get the full set of API versions this endpoint supports.
                    var allVersions = methodVersions.Union(controllerVersions).Distinct();

                    // Only include this endpoint if any of its versions match the current Swagger doc version
                    // This ensures that only endpoints with a matching [ApiVersion] appear in each doc
                    return allVersions.Any(v => v.ToString() == version);
                });
            });

            var app = builder.Build();

            // Configure middleware for development environment
            if (app.Environment.IsDevelopment())
            {
                // Enable middleware to serve the generated Swagger JSON documents as endpoints.
                // This middleware makes the swagger.json files (for all versions) available under /swagger/{version}/swagger.json
                app.UseSwagger();

                // Enable middleware to serve Swagger UI, the interactive web page where users can explore and test API endpoints.
                // Swagger UI needs to know about all versioned Swagger JSON endpoints to show them as separate selectable versions.
                app.UseSwaggerUI(options =>
                {
                    // Add a Swagger endpoint for version 1.0
                    // This links the UI to the generated swagger.json for API Version 1.0
                    options.SwaggerEndpoint("/swagger/1.0/swagger.json", "API Version 1.0");

                    // Add a Swagger endpoint for version 2.0
                    // This links the UI to the generated swagger.json for API Version 2.0
                    options.SwaggerEndpoint("/swagger/2.0/swagger.json", "API Version 2.0");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}