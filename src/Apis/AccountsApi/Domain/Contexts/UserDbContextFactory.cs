using Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AccountsApi.Domain.Contexts;

public class UserDbContextFactory
    : IDesignTimeDbContextFactory<UserDBContext>
{
    public UserDBContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{EnvironmentHelper.EnvironmentName()}.json", optional: false)
            .Build();

        var connectionString = Base64.ConvertToString(configuration.GetSection("DbSettings:ConnectionString").Value) ?? throw new ArgumentNullException("ConnectionString setting");

        var optionsBuilder =
            new DbContextOptionsBuilder<UserDBContext>();

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable(
                "__EFMigrationsHistory",
                "api");
        });

        return new UserDBContext(optionsBuilder.Options);
    }
}