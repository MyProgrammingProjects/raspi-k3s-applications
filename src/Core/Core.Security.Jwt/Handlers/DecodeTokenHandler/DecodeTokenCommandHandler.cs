using Core.Exceptions.InputDataExceptions;
using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Security.Jwt.Handlers.DecodeTokenHandler;

public class DecodeTokenCommandHandler : ICommandHandler<DecodeTokenCommand, DecodeTokenCommandResult>
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


	public DecodeTokenCommandHandler(string audience, string issuer, string secretKey, long timeToLive)
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

	public async Task<Result<DecodeTokenCommandResult>> HandleAsync(DecodeTokenCommand cmd, CancellationToken ct)
	{
		ValidateCommand(cmd);
		var decodedToken = JwtSecurityTokenHandler.ReadToken(cmd.Token)?.ToString() ?? throw new ApplicationException($"Unable to read read token");

		var output = Result<DecodeTokenCommandResult>.Success(new DecodeTokenCommandResult
		{
			DecodedToken = decodedToken
		});

		return await Task.Run(() => output);
	}

	private void ValidateCommand(DecodeTokenCommand cmd)
	{
		if (cmd is null)
			throw new NullOrEmptyException(nameof(cmd));

		if (string.IsNullOrEmpty(cmd.Token))
			throw new NullOrEmptyException(nameof(cmd.Token));

	}
}
