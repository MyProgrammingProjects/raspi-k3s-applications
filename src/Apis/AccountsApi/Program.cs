using AccountsApi.Business.Account.AuthenticateHandler;
using AccountsApi.Domain.Contexts;
using AccountsApi.Middlewares;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Security.KeyVault.Secrets;
using Core.Handlers.Handlers;
using Core.Handlers.Services.Dispatcher;
using Core.Helpers;
using Core.Security.Jwt.Handlers.GenerateTokenHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System;

namespace AccountsApi;

public class Program
{
    public static void Main(string[] args)
    {
        var instanceName = Environment.MachineName;
        var environment = EnvironmentHelper.EnvironmentName();
        var appVersion = EnvironmentHelper.AppVersion();
        var baseImage = EnvironmentHelper.BaseImage();

        Console.WriteLine($"Instance Name: {instanceName}");
        Console.WriteLine($"{DateTimeOffset.UtcNow:yyyy-MM-ddTHH:mm:ssZ} -> Running in Environment {environment}, App Version {appVersion} with base image {baseImage}");

        var builder = WebApplication.CreateBuilder(args);

        ConfigureConfiguration(builder);
        ConfigureLogging(builder);
        ConfigureServices(builder);

        var app = builder.Build();

        ConfigurePipeline(app);

        app.Run();
    }

    private static void ConfigureConfiguration(WebApplicationBuilder builder)
    {
        builder.Configuration.SetBasePath(AppContext.BaseDirectory);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{EnvironmentHelper.EnvironmentName()}.json", optional: false, reloadOnChange: true);

        if (EnvironmentHelper.IsAzure())
        {
            var keyVaultName = builder.Configuration["KeyVaultName"];

            if (string.IsNullOrWhiteSpace(keyVaultName))
                throw new InvalidOperationException("KeyVaultName configuration is missing.");

            var secretClient = new SecretClient(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential());

            builder.Configuration.AddAzureKeyVault(
                secretClient,
                new KeyVaultSecretManager());
        }

        builder.Configuration.AddEnvironmentVariables();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddControllers();

        services.AddHealthChecks();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddAntiforgery();

        services
            .AddDataProtection()
            .SetApplicationName("Accounts Management");

        ConfigureObservability(services);
        ConfigureDatabase(services, configuration);
        ConfigureApplicationServices(services);
        ConfigureJwtInfrastructure(services, configuration);
    }

    private static void ConfigureObservability(IServiceCollection services)
    {
        if (EnvironmentHelper.IsAzure())
        {
            services
                .AddOpenTelemetry()
                .UseAzureMonitor();
        }
        else
        {
            services
                .AddOpenTelemetry()
                .UseOtlpExporter();
        }
    }

    private static void ConfigureDatabase(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UserDBContext>(options =>
        {
            var encodedConnectionString =
                configuration["DbSettings:ConnectionString"];

            if (string.IsNullOrWhiteSpace(encodedConnectionString))
                throw new InvalidOperationException(
                    "DbSettings:ConnectionString is missing.");

            var connectionString =
                Base64.ConvertToString(encodedConnectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Decoded PostgreSQL connection string is invalid.");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseQuerySplittingBehavior(
                    QuerySplittingBehavior.SplitQuery);

                npgsqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "api");

                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });
        });
    }

    private static void ConfigureApplicationServices(
        IServiceCollection services)
    {
        services.AddSingleton<IDispatcherService, DispatcherService>();

        services.AddScoped<
            ICommandHandler<AuthenticateCommand, AuthenticateCommandResult>,
            AuthenticateCommandHandler>();
    }

    private static void ConfigureJwtInfrastructure(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtAudience =
            Base64.ConvertToString(configuration["JWT:Audience"])
            ?? throw new InvalidOperationException(
                "JWT:Audience is missing.");

        var jwtIssuer =
            Base64.ConvertToString(configuration["JWT:Issuer"])
            ?? throw new InvalidOperationException(
                "JWT:Issuer is missing.");

        var jwtSecretKey =
            Base64.ConvertToString(configuration["JWT:SecretKey"])
            ?? throw new InvalidOperationException(
                "JWT:SecretKey is missing.");

        var ttl = Convert.ToInt32(configuration["JWT:TimeToLive"]);

        services.AddSingleton<
            ICommandHandler<GenerateTokenCommand, GenerateTokenCommandResult>>(
            _ => new GenerateTokenCommandHandler(
                jwtAudience,
                jwtIssuer,
                jwtSecretKey,
                ttl));
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddConsole();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Logging.SetMinimumLevel(LogLevel.Information);

        builder.Logging.AddFilter<ConsoleLoggerProvider>(
            string.Empty,
            LogLevel.Information);

        builder.Logging.AddFilter<OpenTelemetryLoggerProvider>(
            string.Empty,
            LogLevel.Information);

        builder.Logging.AddFilter<OpenTelemetryLoggerProvider>(
            nameof(Microsoft.EntityFrameworkCore),
            LogLevel.Information);

        builder.Logging.AddFilter<OpenTelemetryLoggerProvider>(
            "Microsoft.EntityFrameworkCore.Database.Command",
            LogLevel.Warning);
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        if (EnvironmentHelper.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler("/error");

        using (var scope = app.Services.CreateScope())
        {
            var applicationDbContext =
                scope.ServiceProvider.GetRequiredService<UserDBContext>();

            if (EnvironmentHelper.IsDevelopment())
            {
                applicationDbContext.Database.EnsureCreated();
            }

            Console.WriteLine("*********** ENSURE DB *********");

            applicationDbContext.Database.SetCommandTimeout(120);

            applicationDbContext.Database.Migrate();

            applicationDbContext.Database.SetCommandTimeout(30);
        }

        app.UseHealthChecks("/health");

        app.UseCors();

        app.UseAuthentication();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseAuthorization();

        app.MapControllers();
    }
}