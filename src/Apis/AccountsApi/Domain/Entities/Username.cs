using Core.Exceptions.BusinessExceptions;
using Core.Exceptions.InputDataExceptions;
using System.Text.RegularExpressions;

namespace AccountsApi.Domain.Entities
{
	public class Username
	{
		private string _username { get; set; }

		public Username(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				throw new NullOrEmptyException(nameof(username));
			else if (!IsValid(username))
				throw new InvalidUsernameException(nameof(username));

			_username = username;
		}

		public string Value => _username;


		public void ChangeUsername(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				throw new NullOrEmptyException(nameof(username));

			else if (!IsValid(username))
				throw new InvalidUsernameException(nameof(username));

			_username = username;
		}

		private bool IsValid(string username)
		{
			const string regexExp = @"^[a-z]{1}[a-z0-9]{9,24}$";

			var validator = new Regex(regexExp, RegexOptions.IgnoreCase);

			return validator.IsMatch(username);
		}
	}
}
