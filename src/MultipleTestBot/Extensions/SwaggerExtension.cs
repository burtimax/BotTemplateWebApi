// using System.Reflection;
// using Microsoft.OpenApi.Models;
//
// namespace MultipleTestBot.Extensions;
//
// public static class SwaggerExtension
// {
//     public static void AddSwagger(this IServiceCollection services, string title, string version = "V1")
//     {
//         services.AddSwaggerGen(c =>
//         {
//             c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version });
//             c.EnableAnnotations();
//             
//             var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//             c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
//             
//             // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//             // {
//             //     Description = "JWT Authorization header using Bearer scheme",
//             //     Name = "Authorization",
//             //     Scheme = "Bearer",
//             //     BearerFormat = "JWT",
//             //     In = ParameterLocation.Header,
//             //     Type = SecuritySchemeType.ApiKey
//             // });
//             //
//             // c.AddSecurityRequirement(new OpenApiSecurityRequirement
//             // {
//             //     {
//             //         new OpenApiSecurityScheme
//             //         {
//             //             Reference = new OpenApiReference
//             //             {
//             //                 Type = ReferenceType.SecurityScheme,
//             //                 Id = "Bearer"
//             //             }
//             //         },
//             //         Array.Empty<string>()
//             //     }
//             // });
//             
//         });
//     }
// }