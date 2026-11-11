using Core.Exceptions.BusinessExceptions;
using Core.Exceptions.InputDataExceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchApi.Domain.Entities;

public class Role
{
	private IEnumerable<string> validRoles = new List<string>()
	{
		SearchApi.Domain.Enums.Roles.Admin,
		SearchApi.Domain.Enums.Roles.User,
		SearchApi.Domain.Enums.Roles.ReadOnly,
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
