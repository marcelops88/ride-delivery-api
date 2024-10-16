using API.Configurations.Extensions;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Services;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;
using System.Reflection;

namespace API
{
    internal class Program
    {
        private static IConfiguration Configuration { get; } = BuildConfiguration();

        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureSwagger(builder);
            ConfigureLogging();
            ConfigureServices(builder);
            AddServices(builder);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                     .AllowAnyMethod()
                                     .AllowAnyHeader());
            });
            builder.Host.UseSerilog();

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.Run();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddMongoServices(Configuration);
            builder.Services.Configure<StorageSettings>(Configuration.GetSection("StorageSettings"));
            builder.Services.AddScoped<IImagemService, ImagemService>();
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static async void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
        }

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RideAndDeliver API",
                    Version = "v1",
                    Description = "A RideAndDeliver API é responsável por gerenciar operações de locação de motos e entregadores, como cadastro, consulta, alteração e devolução de motos. Ela também permite o envio de CNH e o gerenciamento de locações ativas.",
                    Contact = new OpenApiContact()
                    {
                        Name = "Equipe RideAndDeliver",
                        Email = "suporte@rideanddeliver.com.br",
                    },
                    Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    {
                        "x-logo", new OpenApiObject
                        {
                            { "url", new OpenApiString("https://motociclismoonline.com.br/wp-content/uploads/2024/05/aluguel_de_motos_pixabay.jpg") },
                            { "altText", new OpenApiString("RideAndDeliver API") }
                        }
                    }
                }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
                config.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var groupName = apiDesc.GroupName ?? string.Empty;
                    return groupName == "HealthCheck" || groupName == "Motos" || groupName == "Entregadores";
                });

                config.TagActionsBy(api =>
                {
                    return new List<string> { api.GroupName ?? "Outros" };
                });

                config.OrderActionsBy(apiDesc =>
                {
                    return apiDesc.GroupName switch
                    {
                        "HealthCheck" => "1-HealthCheck",
                        "Motos" => "2-Motos",
                        "Entregadores" => "3-Entregadores",
                        _ => "4-Outros",
                    };
                });
            });
        }

        private static void ConfigureLogging()
        {
            var minLogLevel = Serilog.Events.LogEventLevel.Information;

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(minLogLevel)
                .WriteTo.Console(formatter: new JsonFormatter())
                .WriteTo.File("logs/ride-delivery-api.json", rollingInterval: RollingInterval.Day);
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAllOrigins");
            app.MapControllers();
        }
    }
}