using Core.Handlers.Domain;

namespace UsersApi.Business.Users.RegisterHandler;

public class RegisterCommand : Command
{
	public string Username { get; set; } = null!;
	public string Password { get; set; } = null!;
	public string Email { get; set; } = null!;
}
