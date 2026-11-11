using System;

namespace Core.Security.Jwt;

public class UserContext
{
	public UserContext(string? username
	   , bool isUser
	   , string? ipAddress)
	{
		Username = username;
		IsUser = isUser;
		IpAddress = ipAddress;
	}

	public string? Username { get; set; }
	public bool IsUser { get; set; }
	public string? IpAddress { get; set; }

	public DateTime ExpirationDate { get; private set; }

	public void SetExpirationDate(DateTime exp)
	{
		ExpirationDate = exp;
	}

	public bool ServiceContextExpired()
	{
		return ExpirationDate < DateTime.UtcNow;
	}
}
