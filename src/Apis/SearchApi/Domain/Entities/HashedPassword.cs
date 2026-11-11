using Core.Exceptions.BusinessExceptions;
using Core.Exceptions.InputDataExceptions;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchApi.Domain.Entities;

public class HashedPassword
{
	public string Value { get; private set; }
	public string Salt { get; private set; }

	public HashedPassword(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new NullOrEmptyException(nameof(password));

		else if (!IsValid(password))
			throw new InvalidPasswordException(nameof(password));

		var hashedData = Hash(password);
		Value = Convert.ToBase64String(hashedData.PassHashedBytes);
		Salt = Convert.ToBase64String(hashedData.Salt);
	}

	public static HashedPassword FromJson(string json)
	{
        HashedPassword h = JsonConvert.DeserializeObject<HashedPassword>(json) ?? throw new InvalidOperationException();

        if (h is null)
			throw new ArgumentNullException("Invalid data for password recovery");

		return new HashedPassword(h.Value, h.Salt);
	}

    [JsonConstructor]
    public HashedPassword(string value, string salt)
    {
		if (string.IsNullOrWhiteSpace(value))
			throw new NullOrEmptyException(nameof(value));

		if (string.IsNullOrWhiteSpace(salt))
			throw new NullOrEmptyException(nameof(salt));

		Value = value;
		Salt = salt;
	}

	public void ChangePassword(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new NullOrEmptyException(nameof(password));

		else if (!IsValid(password))
			throw new InvalidPasswordException(nameof(password));

		var hashedData = Hash(password);
		Value = Convert.ToBase64String(hashedData.PassHashedBytes);
		Salt = Convert.ToBase64String(hashedData.Salt);
	}

    private (byte[] Salt, byte[] PassHashedBytes) Hash(string password)
    {
        byte[] _salt = new byte[16];
        RandomNumberGenerator.Create().GetBytes(_salt);
        Salt = Convert.ToBase64String(_salt);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            _salt,
            10000,
            HashAlgorithmName.SHA256,
            32
        );

        return (Salt: _salt, PassHashedBytes: hash);
    }

    private bool IsValid(string password)
	{
		/* https://stackoverflow.com/questions/2370015/regular-expression-for-password-validation
                ^.*              : Start
                (?=.{16,})        : Length
                (?=.*[a-zA-Z])   : Letters
                (?=.*\d)         : Digits
                (?=.*[!#$%&? "]) : Special characters
                .*$              : End
        */
		const string regexExp = @"^.*(?=.{16,})(?=.*[a-zA-Z])(?=.*\d)(?=.*[!#$%&? ""]).*$";

		var validator = new Regex(regexExp);

		return validator.IsMatch(password);
	}
}
