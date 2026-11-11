using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;


[Serializable]
public class InvalidEmailException : BusinessValidationException
{
	public InvalidEmailException() { }
	public InvalidEmailException(string message) : base(message) { }
	public InvalidEmailException(string message, Exception inner) : base(message, inner) { }
}

