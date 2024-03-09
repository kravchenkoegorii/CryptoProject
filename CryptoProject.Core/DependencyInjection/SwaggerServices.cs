using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CryptoProject.Core.DependencyInjection
{
    public static class SwaggerServices
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "CryptoProject",
                        Version = "v1",
                        Description = "Test Task Peanut Trade",
                    });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
