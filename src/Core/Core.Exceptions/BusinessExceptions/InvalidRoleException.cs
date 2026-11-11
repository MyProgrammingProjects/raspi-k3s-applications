using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;


[Serializable]
public class InvalidRoleException : BusinessValidationException
{
	public InvalidRoleException() { }
	public InvalidRoleException(string message) : base(message) { }
	public InvalidRoleException(string message, Exception inner) : base(message, inner) { }
}

