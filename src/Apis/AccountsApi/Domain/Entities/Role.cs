using Core.Exceptions.BusinessExceptions;
using Core.Exceptions.InputDataExceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Domain.Entities
{
	public class Role
	{
		private IEnumerable<string> validRoles = new List<string>()
		{
			AccountsApi.Domain.Enums.Roles.Admin,
			AccountsApi.Domain.Enums.Roles.User,
			AccountsApi.Domain.Enums.Roles.ReadOnly,
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
}
