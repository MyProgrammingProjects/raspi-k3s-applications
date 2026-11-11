using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace Core.Security.Jwt.Handlers.GetTokenClaimsHandler;

public class GetTokenClaimsCommandResult
{
	public GetTokenClaimsCommandResult()
	{
		Claims = ReadOnlyCollection<Claim>.Empty;
	}

	public IReadOnlyCollection<Claim> Claims { get; internal set; }
}
