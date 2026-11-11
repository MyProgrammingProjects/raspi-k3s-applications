using Core.Handlers.Domain;
using System.Collections.Generic;
using System.Security.Claims;

namespace Core.Security.Jwt.Handlers.GenerateTokenHandler;

public sealed class GenerateTokenCommand : Command
{
	public GenerateTokenCommand()
	{
		Claims = System.Linq.Enumerable.Empty<Claim>();
	}


	public string Username { get; set; } = null!;
	public IEnumerable<Claim> Claims { get; set; }
}
