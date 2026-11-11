using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Core.Security.Jwt;
using Core.Security.Jwt.Handlers.GetTokenClaimsHandler;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;

namespace UsersApi.Identity;



internal sealed class HttpUserContextAccessor : IUserContextAccessor
{
	private readonly IHttpContextAccessor HttpContextAccessor;
	private readonly ICommandHandler<GetTokenClaimsCommand, GetTokenClaimsCommandResult> GetTokenClaimsCommandHandler;
	private UserContext? Context;

	public HttpUserContextAccessor(IHttpContextAccessor httpContextAccessor
		, ICommandHandler<GetTokenClaimsCommand, GetTokenClaimsCommandResult> getTokenClaimsCommandHandler)
	{
		HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		GetTokenClaimsCommandHandler = getTokenClaimsCommandHandler ?? throw new ArgumentNullException(nameof(getTokenClaimsCommandHandler));
	}

	public UserContext UserContext => Context ??= GetUserContext();

	private UserContext GetUserContext()
	{
		Microsoft.Extensions.Primitives.StringValues? authorizationHeaders = HttpContextAccessor.HttpContext?.Request?.Headers["Authorization"];

		if (!string.IsNullOrEmpty(authorizationHeaders))
		{
			var token = authorizationHeaders?.FirstOrDefault(_ => _ != null && _.StartsWith("Bearer", StringComparison.InvariantCultureIgnoreCase));
			if (!string.IsNullOrEmpty(token))
			{
				token = token.Replace("Bearer", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
				Result<GetTokenClaimsCommandResult> getTokenClaimsCommandResult = GetTokenClaimsCommandHandler.HandleAsync(new GetTokenClaimsCommand
				{
					DecodedToken = token
				}, CancellationToken.None).GetAwaiter().GetResult();

				if (getTokenClaimsCommandResult.IsFailure)
					throw new Exception(getTokenClaimsCommandResult.Errors.FirstOrDefault()?.ToString());
				if (getTokenClaimsCommandResult.Data is null)
					throw new Exception("Unable to obtain Tokens Claims");


				UserContext? serviceContext = null;

				var usernameClaim = getTokenClaimsCommandResult.Data.Claims.FirstOrDefault(userClaim => userClaim.Type == Core.Security.Jwt.Claims.Username);
				var useridClaim = getTokenClaimsCommandResult.Data.Claims.FirstOrDefault(userClaim => userClaim.Type == Core.Security.Jwt.Claims.NameIdentifier);
				var expirationClaim = getTokenClaimsCommandResult.Data.Claims.FirstOrDefault(userClaim => userClaim.Type == Core.Security.Jwt.Claims.ExpirationDateTime);
				var rolesClaim = getTokenClaimsCommandResult.Data.Claims.Where(userClaim => userClaim.Type == Core.Security.Jwt.Claims.Role);


				if (usernameClaim != null && !string.IsNullOrWhiteSpace(usernameClaim?.Value) && rolesClaim.Any())
				{
					string? ipAddress = HttpContextAccessor!.HttpContext?.Connection?.RemoteIpAddress?.ToString();
					var expClaim = expirationClaim?.Value;
					var tokenExpiryTime = UnixTimeStampToDateTime(Convert.ToDouble(expClaim));
					bool isUser = rolesClaim.Any(c => c.Value == "User"); //TODO  have "User" and other roles in proper class

					serviceContext = new UserContext(
								  usernameClaim.Value
								, isUser
								, ipAddress
							);

					serviceContext.SetExpirationDate(tokenExpiryTime);

					return serviceContext;
				}
			}
		}

		return DefaultUserContext();
	}

	private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
		return dtDateTime;
	}

	private UserContext DefaultUserContext()
	{
		DateTime currentTime = DateTime.Now;
		DateTime expires = currentTime.AddMinutes(1);

		string? ipAddress = HttpContextAccessor!.HttpContext?.Connection?.RemoteIpAddress?.ToString();

		bool isUser = false;

		UserContext userContext = new UserContext(
				  null
				, isUser
				, ipAddress
			);

		userContext.SetExpirationDate(expires);

		return userContext;
	}
}
