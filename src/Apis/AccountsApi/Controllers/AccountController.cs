using AccountsApi.Business.Account.AuthenticateHandler;
using Core.Handlers.Services.Dispatcher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace AccountsApi.Controllers
{
	[Authorize]
	[ApiController, Route("api/account"), Produces("application/json")]
	public class AccountController : ControllerBase
	{
		private readonly IDispatcherService IDispatcherService;

		public AccountController(IDispatcherService iDispatcherService)
		{
			IDispatcherService = iDispatcherService ?? throw new System.ArgumentNullException(nameof(iDispatcherService));
		}

		[AllowAnonymous]
		[HttpPost, Route("authenticate")]
		public async Task<JsonResult> AuthenticateAsync([FromBody] AuthenticateCommand input, CancellationToken ct)
		{
			var result = await IDispatcherService.DispatchCommandAsync<AuthenticateCommand, AuthenticateCommandResult>(input, ct).ConfigureAwait(false);

			return new JsonResult(result, new System.Text.Json.JsonSerializerOptions
			{
				WriteIndented = true,
			});
		}
	}
}