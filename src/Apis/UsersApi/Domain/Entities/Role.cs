using Core.Exceptions.BusinessExceptions;
using Core.Exceptions.InputDataExceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UsersApi.Domain.Entities;

public class Role
{
	private IEnumerable<string> validRoles = new List<string>()
	{
		UsersApi.Domain.Enums.Roles.Admin,
		UsersApi.Domain.Enums.Roles.User,
		UsersApi.Domain.Enums.Roles.ReadOnly,
	};

	public string UserRole { get; private set; }

	public Role(string role)
	{
		if (string.IsNullOrWhiteSpace(role))
			throw new NullOrEmptyException(nameof(role));

		else if (!IsValid(role))
			throw new InvalidRoleException(nameof(role));

		UserRole = role;
	}

	private bool IsValid(string role)
	{
		return validRoles.Contains(role);
	}
}
