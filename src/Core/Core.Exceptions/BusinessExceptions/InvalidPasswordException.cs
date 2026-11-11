using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;


[Serializable]
public class InvalidPasswordException : BusinessValidationException
{
	public InvalidPasswordException() { }
	public InvalidPasswordException(string message) : base(message) { }
	public InvalidPasswordException(string message, Exception inner) : base(message, inner) { }
}


