using Core.Handlers.Domain;

namespace Core.Security.Jwt.Handlers.DecodeTokenHandler;

public class DecodeTokenCommand : Command
{
	public string Token { get; set; } = null!;
}
