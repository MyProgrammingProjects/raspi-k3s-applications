using Core.Exceptions.BusinessExceptions;
using Core.Exceptions.InputDataExceptions;
using System.Text.RegularExpressions;

namespace SearchApi.Domain.Entities;

public class Email
{
	private string _email { get; set; }

	public Email(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
			throw new NullOrEmptyException(nameof(email));

		else if (!IsValid(email))
			throw new InvalidEmailException(nameof(email));

		_email = email;
	}

	public string Value => _email;


	public void ChangeEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
			throw new NullOrEmptyException(nameof(email));

		else if (!IsValid(email))
			throw new InvalidEmailException(nameof(email));

		_email = email;
	}

	private bool IsValid(string email)
	{
		const string regexExp = @"^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

		var validator = new Regex(regexExp, RegexOptions.IgnoreCase);

		return validator.IsMatch(email);
	}
}
