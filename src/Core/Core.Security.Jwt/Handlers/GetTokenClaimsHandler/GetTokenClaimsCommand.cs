using Core.Handlers.Domain;

namespace Core.Security.Jwt.Handlers.GetTokenClaimsHandler;

public class GetTokenClaimsCommand : Command
{
	public string DecodedToken { get; set; } = null!;
}
