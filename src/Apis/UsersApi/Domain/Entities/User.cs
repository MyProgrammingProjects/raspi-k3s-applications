using System;
using System.Collections.Generic;
using System.Linq;

namespace UsersApi.Domain.Entities;

public class User
{
	public Guid UserId { get; private set; }
	public Username Username { get; private set; } = null!;
	public Email Email { get; private set; } = null!;
	public HashedPassword HashedPassword { get; private set; } = null!;
	public IEnumerable<Role> Roles { get; private set; } = null!;

	private User() { } //ORM requires parameterless ctor

	public User(string username, string password, string email, IEnumerable<Role> roles)
	{
		UserId = Guid.NewGuid();
		Username = new Username(username);
		HashedPassword = new HashedPassword(password);
		Email = new Email(email);
		Roles = roles;
	}

	public User(string username, string password, string email)
	{
		UserId = Guid.NewGuid();
		Username = new Username(username);
		HashedPassword = new HashedPassword(password);
		Email = new Email(email);
		Roles = new List<Role> { new Role(UsersApi.Domain.Enums.Roles.User) };
	}

	private User(Guid userid, Username username, HashedPassword hashedPassword, Email email, IEnumerable<Role> roles)
	{
		UserId = userid;
		Username = username;
		HashedPassword = hashedPassword;
		Email = email;
		Roles = roles ?? Enumerable.Empty<Role>();
	}
}
