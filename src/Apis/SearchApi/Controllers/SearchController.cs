using Core.Handlers.Services.Dispatcher;
using Core.Security.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Business.Users.GetUsersHandler;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Controllers;

[Authorize]
[ApiController, Route("api/search"), Produces("application/json")]
public class SearchController : ControllerBase
{
	private readonly IDispatcherService IDispatcherService;
	private readonly IUserContextAccessor UserContextAccessor;
	private readonly UserContext UserContext;

	public SearchController(IDispatcherService iDispatcherService, IUserContextAccessor userContextAccessor)
	{
		IDispatcherService = iDispatcherService ?? throw new System.ArgumentNullException(nameof(iDispatcherService));
		UserContextAccessor = userContextAccessor ?? throw new System.ArgumentNullException(nameof(userContextAccessor));
		UserContext = UserContextAccessor.UserContext;

	}

	private bool IsUserContextValid()
	{
		return UserContext != null && !UserContext.ServiceContextExpired();
	}

	[HttpGet, Route("users")]
	public async Task<JsonResult> GetUsersAsync(CancellationToken ct)
	{
		if (!IsUserContextValid())
			return new JsonResult(Unauthorized());

		GetUsersQuery input = new GetUsersQuery();

		var result = await IDispatcherService.DispatchQueryAsync<GetUsersQuery, GetUsersQueryResult>(input, ct).ConfigureAwait(false);

		return new JsonResult(result, new System.Text.Json.JsonSerializerOptions
		{
			WriteIndented = true,
		});
	}
}