using Core.Handlers.Handlers;
using Core.Handlers.Output;
using SearchApi.Domain.Contexts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Business.Users.GetUsersHandler;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, GetUsersQueryResult>
{
	private readonly UserDBContext _userDBContext;
	public GetUsersQueryHandler(UserDBContext userDBContext)
	{
		_userDBContext = userDBContext ?? throw new ArgumentNullException(nameof(userDBContext));
	}
	public async Task<Result<GetUsersQueryResult>> HandleAsync(GetUsersQuery query, CancellationToken ct)
	{
		var output = await Task.Run(() =>
		{
			var users = _userDBContext.Users;
			var lUsers = users.ToList();

			return Result<GetUsersQueryResult>.Success(new GetUsersQueryResult
			{
				Users = lUsers.Select(u => new UserResult(u.UserId, u.Username.Value, u.Email.Value, u.Roles.Select(r => r.UserRole)))
			});
		});

		return output;
	}
}
