using System.Security.Principal;

namespace Core.Security.Jwt.Handlers.ValidateTokenHandler;

public class ValidateTokenCommandResult
{
	public IPrincipal? ValidationResult { get; set; }
}
