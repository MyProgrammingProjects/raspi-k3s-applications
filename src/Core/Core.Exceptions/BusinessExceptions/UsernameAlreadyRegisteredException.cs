using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;


[Serializable]
public class UsernameAlreadyRegisteredException : BusinessValidationException
{
	public UsernameAlreadyRegisteredException() { }
	public UsernameAlreadyRegisteredException(string message) : base(message) { }
	public UsernameAlreadyRegisteredException(string message, Exception inner) : base(message, inner) { }
}
