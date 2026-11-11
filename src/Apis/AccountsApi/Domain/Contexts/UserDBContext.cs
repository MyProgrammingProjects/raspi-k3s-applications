using AccountsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Domain.Contexts
{
	public class UserDBContext : DbContext
	{
		public DbSet<User> Users { get; set; } = null!;

		public UserDBContext(DbContextOptions<UserDBContext> options)
		 : base(options)
		{
		}


		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 4);
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
}
