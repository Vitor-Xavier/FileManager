using Microsoft.OpenApi.Models;
using System.Reflection;

namespace FileManager.Extensions
{
    public static class ServiceExtensions
    {
        internal static void ConfigureSwagger(this IServiceCollection services) =>
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "File Manager",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Vitor Xavier de Souza",
                        Email = "vitorvxs@live.com",
                        Url = new Uri("https://github.com/Vitor-Xavier")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                //c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                //{
                //    Name = "Authorization",
                //    Description = "JWT Authorization header using the Bearer scheme.",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "bearer",
                //    BearerFormat = "JWT"

                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                //        },
                //        Array.Empty<string>()
                //    }
                //});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
    }
}
