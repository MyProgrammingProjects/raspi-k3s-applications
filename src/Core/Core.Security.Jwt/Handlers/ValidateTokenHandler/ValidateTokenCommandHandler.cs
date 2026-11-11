using Core.Exceptions.InputDataExceptions;
using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Security.Jwt.Handlers.ValidateTokenHandler;

public class ValidateTokenCommandHandler : ICommandHandler<ValidateTokenCommand, ValidateTokenCommandResult>
{
	private readonly string Audience;
	private readonly string Issuer;
	private readonly string SecretKey;
	private readonly long TimeToLive;

	private JwtSecurityTokenHandler JwtSecurityTokenHandler { get; }

	private SecurityTokenDescriptor securityTokenDescriptor = null!;

	private SecurityTokenDescriptor SecurityTokenDescriptor => securityTokenDescriptor ??= new SecurityTokenDescriptor
	{
		Audience = Audience,
		Issuer = Issuer,
		IssuedAt = null,
		Expires = null,
		Subject = null,
		SigningCredentials =
			new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
				"HS256")
	};


	public ValidateTokenCommandHandler(string audience, string issuer, string secretKey, long timeToLive)
	{
		if (string.IsNullOrEmpty(audience))
			throw new ArgumentNullException(nameof(audience));

		if (string.IsNullOrEmpty(issuer))
			throw new ArgumentNullException(nameof(issuer));

		if (string.IsNullOrEmpty(secretKey))
			throw new ArgumentNullException(nameof(secretKey));

		Audience = audience;
		Issuer = issuer;
		SecretKey = secretKey;
		TimeToLive = timeToLive;

		JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
	}

	public async Task<Result<ValidateTokenCommandResult>> HandleAsync(ValidateTokenCommand cmd, CancellationToken ct)
	{
		ValidateCommand(cmd);

		var validationResult = JwtSecurityTokenHandler
			.ValidateToken(cmd.DecodedToken,
				new TokenValidationParameters
				{
					ValidIssuers = new List<string> { Issuer },
					ValidAudiences = new List<string> { Audience },
					IssuerSigningKeys = new List<SecurityKey>
						{ new SymmetricSecurityKey(Convert.FromBase64String(SecretKey)) },
					ValidateLifetime = true,
					RequireExpirationTime = true,
					SaveSigninToken = false // stores token at principal.identity.bootstrapcontext
				}, out _);

		var output = Result<ValidateTokenCommandResult>.Success(new ValidateTokenCommandResult
		{
			ValidationResult = validationResult
		});

		return await Task.Run(() => output);
	}

	private void ValidateCommand(ValidateTokenCommand cmd)
	{
		if (cmd is null)
			throw new NullOrEmptyException(nameof(cmd));

		if (string.IsNullOrEmpty(cmd.DecodedToken))
			throw new NullOrEmptyException(nameof(cmd.DecodedToken));

	}
}