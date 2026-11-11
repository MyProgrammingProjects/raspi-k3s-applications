using System;
using System.Collections.Generic;

namespace UsersApi.Business.Users.GetUsersHandler;

public class GetUsersQueryResult
{
	public GetUsersQueryResult()
	{
		Users = System.Linq.Enumerable.Empty<UserResult>();
	}

	public IEnumerable<UserResult> Users { get; set; }
}

public class UserResult
{
	public Guid UserId { get; private set; }
	public string Username { get; private set; }
	public string Email { get; private set; }
	public IEnumerable<string> Roles { get; private set; }

	public UserResult(Guid userId, string username, string email, IEnumerable<string> roles)
	{
		UserId = userId;
		Username = username;
		Email = email;
		Roles = roles;
	}
}
