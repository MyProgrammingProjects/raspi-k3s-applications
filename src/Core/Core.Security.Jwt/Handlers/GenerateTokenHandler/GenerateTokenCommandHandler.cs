using Core.Exceptions.InputDataExceptions;
using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Core.Helpers;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Security.Jwt.Handlers.GenerateTokenHandler;

public sealed class GenerateTokenCommandHandler : ICommandHandler<GenerateTokenCommand, GenerateTokenCommandResult>
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


	public GenerateTokenCommandHandler(string audience, string issuer, string secretKey, long timeToLive)
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
	public async Task<Result<GenerateTokenCommandResult>> HandleAsync(GenerateTokenCommand cmd, CancellationToken ct)
	{
		ValidateCommand(cmd);

		DateTime currentTime = DateTime.UtcNow;
		SecurityTokenDescriptor.IssuedAt = currentTime;
		SecurityTokenDescriptor.Expires = currentTime.AddMinutes(TimeToLive);
		SecurityTokenDescriptor.Subject = new ClaimsIdentity(cmd.Claims);

		var token = JwtSecurityTokenHandler.WriteToken(JwtSecurityTokenHandler.CreateToken(securityTokenDescriptor));
		var output = Result<GenerateTokenCommandResult>.Success(new GenerateTokenCommandResult
		{
			Token = token
		});

		return await Task.Run(() => output);

	}

	private void ValidateCommand(GenerateTokenCommand cmd)
	{
		if (cmd is null)
			throw new NullOrEmptyException(nameof(cmd));

		if (cmd.Claims.IsNullOrEmpty())
			throw new NullOrEmptyException(nameof(cmd.Claims));

	}
}
