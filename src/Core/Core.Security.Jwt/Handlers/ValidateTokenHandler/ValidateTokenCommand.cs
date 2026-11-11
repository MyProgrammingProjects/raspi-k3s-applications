using Core.Handlers.Domain;

namespace Core.Security.Jwt.Handlers.ValidateTokenHandler;

public class ValidateTokenCommand : Command
{
	public string DecodedToken { get; set; } = null!;
}
