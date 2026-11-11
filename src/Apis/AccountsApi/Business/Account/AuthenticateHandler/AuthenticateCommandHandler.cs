using AccountsApi.Domain.Contexts;
using AccountsApi.Domain.Entities;
using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Core.Security.Jwt.Handlers.GenerateTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AccountsApi.Business.Account.AuthenticateHandler;

public class AuthenticateCommandHandler : ICommandHandler<AuthenticateCommand, AuthenticateCommandResult>
{
	private readonly UserDBContext _userDBContext;
	private readonly ICommandHandler<GenerateTokenCommand, GenerateTokenCommandResult> GenerateTokenCommandHandler;

	public AuthenticateCommandHandler(UserDBContext userDBContext
		, ICommandHandler<GenerateTokenCommand, GenerateTokenCommandResult> generateTokenCommandHandler)
	{
		_userDBContext = userDBContext ?? throw new ArgumentNullException(nameof(userDBContext));
		GenerateTokenCommandHandler = generateTokenCommandHandler ?? throw new ArgumentNullException(nameof(generateTokenCommandHandler));
	}
	public async Task<Result<AuthenticateCommandResult>> HandleAsync(AuthenticateCommand cmd, CancellationToken ct)
	{
		User? existingUser = _userDBContext.Users.SingleOrDefault(u => u.Username == new Username(cmd.Username));

		if (existingUser == null)
			return Result<AuthenticateCommandResult>.Failure(new List<Error> { new Error(ErrorTypeEnum.ValidationError, "INVALID_CREDENTIALS", $"Supplied Username/Password invalid") })!;

		HashedPassword inputPasswordHash = HashedPassword.FromKnownSalt(cmd.Password, existingUser.HashedPassword.Salt);

		if (!string.Equals(existingUser.HashedPassword.Value, inputPasswordHash.Value))
			return Result<AuthenticateCommandResult>.Failure(new List<Error> { new Error(ErrorTypeEnum.ValidationError, "INVALID_CREDENTIALS", $"Supplied Username/Password invalid") })!;

		List<Claim> claims = new List<Claim>();
		claims.Add(new Claim(Core.Security.Jwt.Claims.Username, cmd.Username));
		claims.Add(new Claim(Core.Security.Jwt.Claims.NameIdentifier, existingUser.UserId.ToString()));
		claims.Add(new Claim(Core.Security.Jwt.Claims.SessionId, Guid.NewGuid().ToString()));
		claims.AddRange(existingUser.Roles.Select(r => new Claim(Core.Security.Jwt.Claims.Role, r.UserRole)));

		GenerateTokenCommand generateTokenCommand = new GenerateTokenCommand
		{
			Username = cmd.Username,
			Claims = claims
		};

		Result<GenerateTokenCommandResult> generateTokenCommandResult = await GenerateTokenCommandHandler.HandleAsync(generateTokenCommand, ct);

		if (generateTokenCommandResult.IsFailure)
			return Result<AuthenticateCommandResult>.Failure(generateTokenCommandResult.Errors)!;

		var token = generateTokenCommandResult.Data?.Token ?? throw new ArgumentNullException("Unable to retrieve valid token");

		var output = Result<AuthenticateCommandResult>.Success(new AuthenticateCommandResult
		{
			Token = token
		});

		return output;
	}
}