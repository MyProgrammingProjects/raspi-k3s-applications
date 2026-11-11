using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;


[Serializable]
public class InvalidUsernameException : BusinessValidationException
{
	public InvalidUsernameException() { }
	public InvalidUsernameException(string message) : base(message) { }
	public InvalidUsernameException(string message, Exception inner) : base(message, inner) { }
}

