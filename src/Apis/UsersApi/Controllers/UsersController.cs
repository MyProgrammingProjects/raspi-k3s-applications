using Core.Handlers.Services.Dispatcher;
using Core.Security.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using UsersApi.Business.Users.GetUsersHandler;
using UsersApi.Business.Users.RegisterHandler;

namespace UsersApi.Controllers;

[Authorize]
[ApiController, Route("api/users"), Produces("application/json")]
public class UsersController : ControllerBase
{
	private readonly IDispatcherService IDispatcherService;
	private readonly IUserContextAccessor UserContextAccessor;
	private readonly UserContext UserContext;

	public UsersController(IDispatcherService iDispatcherService, IUserContextAccessor userContextAccessor)
	{
		IDispatcherService = iDispatcherService ?? throw new System.ArgumentNullException(nameof(iDispatcherService));
		UserContextAccessor = userContextAccessor ?? throw new System.ArgumentNullException(nameof(userContextAccessor));
		UserContext = UserContextAccessor.UserContext;

	}

	private bool IsUserContextValid()
	{
		return UserContext != null && !UserContext.ServiceContextExpired();
	}

	[AllowAnonymous]
	[HttpPost, Route("register")]
	public async Task<JsonResult> RegisterAsync([FromBody] RegisterCommand input, CancellationToken ct)
	{
		var result = await IDispatcherService.DispatchCommandAsync<RegisterCommand, RegisterCommandResult>(input, ct).ConfigureAwait(false);

		return new JsonResult(result, new System.Text.Json.JsonSerializerOptions
		{
			WriteIndented = true,
		});
	}

	[HttpPost, Route("search")]
	public async Task<JsonResult> GetUsersAsync([FromBody] GetUsersQuery input, CancellationToken ct)
	{
		if (!IsUserContextValid())
			return new JsonResult(Unauthorized());

		var result = await IDispatcherService.DispatchQueryAsync<GetUsersQuery, GetUsersQueryResult>(input, ct).ConfigureAwait(false);

		return new JsonResult(result, new System.Text.Json.JsonSerializerOptions
		{
			WriteIndented = true,
		});
	}
}