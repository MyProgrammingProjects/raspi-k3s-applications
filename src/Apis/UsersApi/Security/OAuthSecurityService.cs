using Core.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace UsersApi.Security;

internal sealed class OAuthSecurityService
{
	public static void Configure(IServiceCollection servicesContainer, IConfiguration configuration)
	{
		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

		var configSecretKey = configuration["JWT:SecretKey"];
		if (configSecretKey.IsNullOrEmpty())
			throw new ArgumentNullException("JWT:SecretKey must be filled");

		byte[] secretKey = Convert.FromBase64String(configSecretKey!);

		servicesContainer.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
						 .AddJwtBearer(options =>
						 {
							 options.Audience = Base64.ConvertToString(configuration["JWT:Audience"]);
							 options.ClaimsIssuer = Base64.ConvertToString(configuration["JWT:Issuer"]);
							 options.RequireHttpsMetadata = false;
							 options.SaveToken = false;
							 options.TokenValidationParameters = new TokenValidationParameters
							 {
								 RequireExpirationTime = true,
								 RequireSignedTokens = true,
								 ValidateIssuerSigningKey = true,
								 IssuerSigningKey = new SymmetricSecurityKey(secretKey),
								 ValidateIssuer = true,
								 ValidIssuer = Base64.ConvertToString(configuration["JWT:Issuer"]),
								 ValidateAudience = true,
								 ValidAudience = Base64.ConvertToString(configuration["JWT:Audience"]),
								 ValidateLifetime = true,
								 ClockSkew = TimeSpan.Zero // remove delay of token when expire
							 };
						 });
	}
}
