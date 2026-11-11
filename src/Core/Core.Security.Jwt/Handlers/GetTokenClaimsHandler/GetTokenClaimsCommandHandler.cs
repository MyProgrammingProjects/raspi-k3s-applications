using Core.Exceptions.InputDataExceptions;
using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Security.Jwt.Handlers.GetTokenClaimsHandler;

public class GetTokenClaimsCommandHandler : ICommandHandler<GetTokenClaimsCommand, GetTokenClaimsCommandResult>
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


	public GetTokenClaimsCommandHandler(string audience, string issuer, string secretKey, long timeToLive)
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

	public async Task<Result<GetTokenClaimsCommandResult>> HandleAsync(GetTokenClaimsCommand cmd, CancellationToken ct)
	{
		ValidateCommand(cmd);
		var claims = JwtSecurityTokenHandler.ReadJwtToken(cmd.DecodedToken).Claims.ToList() ?? throw new ApplicationException($"Unable to read read token");

		var output = Result<GetTokenClaimsCommandResult>.Success(new GetTokenClaimsCommandResult
		{
			Claims = claims
		});

		return await Task.Run(() => output);
	}

	private void ValidateCommand(GetTokenClaimsCommand cmd)
	{
		if (cmd is null)
			throw new NullOrEmptyException(nameof(cmd));

		if (string.IsNullOrEmpty(cmd.DecodedToken))
			throw new NullOrEmptyException(nameof(cmd.DecodedToken));

	}
}