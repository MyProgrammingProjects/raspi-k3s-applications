using Core.Handlers.Domain;

namespace AccountsApi.Business.Account.AuthenticateHandler
{
	public class AuthenticateCommand : Command
	{
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}
