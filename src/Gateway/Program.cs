using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System;

namespace Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var instanceName = Environment.MachineName;
        var environment = EnvironmentHelper.EnvironmentName();
        var appVersion = EnvironmentHelper.AppVersion();
        var baseImage = EnvironmentHelper.BaseImage();

        Console.WriteLine($"Instance Name: {instanceName}");

        Console.WriteLine(
            $"{DateTimeOffset.UtcNow:yyyy-MM-ddTHH:mm:ssZ} -> " +
            $"Running in Environment {environment}, " +
            $"App Version {appVersion} " +
            $"with base image {baseImage}");

        var builder = WebApplication.CreateBuilder(args);

        ConfigureConfiguration(builder);
        ConfigureLogging(builder);
        ConfigureServices(builder);

        var app = builder.Build();

        ConfigurePipeline(app);

        app.Run();
    }

    private static void ConfigureConfiguration(
        WebApplicationBuilder builder)
    {
        builder.Configuration.SetBasePath(AppContext.BaseDirectory);

        builder.Configuration
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)

            .AddJsonFile(
                $"appsettings.{EnvironmentHelper.EnvironmentName()}.json",
                optional: false,
                reloadOnChange: true);

        if (EnvironmentHelper.IsAzure())
        {
            var keyVaultName = builder.Configuration["KeyVaultName"];

            if (string.IsNullOrWhiteSpace(keyVaultName))
            {
                throw new InvalidOperationException(
                    "KeyVaultName configuration is missing.");
            }

            var secretClient = new SecretClient(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential());

            builder.Configuration.AddAzureKeyVault(
                secretClient,
                new KeyVaultSecretManager());
        }

        builder.Configuration.AddEnvironmentVariables();
    }

    private static void ConfigureServices(
        WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddHealthChecks();

        services.AddReverseProxy()
            .LoadFromConfig(
                configuration.GetSection("ReverseProxy"));

        ConfigureObservability(services);
    }

    private static void ConfigureObservability(
        IServiceCollection services)
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

    private static void ConfigureLogging(
        WebApplicationBuilder builder)
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
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        if (EnvironmentHelper.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHealthChecks("/health");

        app.MapReverseProxy();
    }
}