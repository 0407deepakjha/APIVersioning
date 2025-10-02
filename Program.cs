using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;

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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add API versioning services to DI
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}