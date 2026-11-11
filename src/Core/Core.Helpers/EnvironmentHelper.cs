using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Helpers;

public class EnvironmentHelper
{
    public static bool IsDevelopment()
    {
        return string.Equals("development", EnvironmentName(), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsTesting()
    {
        return string.Equals("testing", EnvironmentName(), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsQuality()
    {
        return string.Equals("quality", EnvironmentName(), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsProduction()
    {
        return string.Equals("production", EnvironmentName(), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAzure()
    {
        return string.Equals("azure", EnvironmentName(), StringComparison.OrdinalIgnoreCase);
    }

    public static string EnvironmentName()
    {
        var aspnetcoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.IsNullOrWhiteSpace(aspnetcoreEnvironment) == false ? aspnetcoreEnvironment : throw new ArgumentException("Hosting Environment no set in ASPNETCORE_ENVIRONMENT or AZURE_FUNCTIONS_ENVIRONMENT");
    }

    public static string? ApplicationIdentifier()
    {
        var appIdentifier = Environment.GetEnvironmentVariable("APP_IDENTIFIER_NAME");
        return (string.IsNullOrWhiteSpace(appIdentifier) == false) ? appIdentifier : null;
    }

    public static string? AppVersion()
    {
        var appVersion = Environment.GetEnvironmentVariable("APP_VERSION");
        return (string.IsNullOrWhiteSpace(appVersion) == false) ? appVersion : null;
    }

    public static string? BaseImage()
    {
        var baseImage = Environment.GetEnvironmentVariable("BASE_IMAGE");
        return (string.IsNullOrWhiteSpace(baseImage) == false) ? baseImage : null;
    }
}
