using Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UsersApi.Domain.Entities;

namespace UsersApi.Domain.Contexts;

public class UserDBContext : DbContext
{
	private readonly IConfiguration Configuration;

	public DbSet<User> Users { get; set; } = null!;

	public UserDBContext(IConfiguration configuration)
	{
		this.Configuration = configuration;
	}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Base64.ConvertToString(Configuration.GetSection("DbSettings:ConnectionString").Value) ?? throw new ArgumentNullException("ConnectionString setting");

        optionsBuilder.UseNpgsql(connectionString, (pgOptions) =>
        {
            pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("api");
        modelBuilder.Entity<User>();
        modelBuilder.Entity<User>().Property(o => o.Username)
            .HasConversion<string>(u => u.Value, u => new Username(u));

        modelBuilder.Entity<User>().HasKey(o => o.Username);

        modelBuilder.Entity<User>().Property(o => o.Email)
             .HasConversion<string>(u => u.Value, u => new Email(u))
             .HasMaxLength(250);

        var hashedPasswordConverter = new ValueConverter<HashedPassword, string>(
                   v => JsonConvert.SerializeObject(v),
                   v => HashedPassword.FromJson(v));

        modelBuilder.Entity<User>().Property(o => o.HashedPassword)
             .HasConversion(hashedPasswordConverter);

        var rolesConverter = new ValueConverter<IEnumerable<Role>, string>(
                   v => String.Join(',', v.Select(g => g.UserRole)),
                   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => new Role(r)));

        modelBuilder.Entity<User>().Property(p => p.Roles)
            .HasConversion(rolesConverter);

        base.OnModelCreating(modelBuilder);
    }


}
