using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;


[Serializable]
public class EmailAlreadyRegisteredException : BusinessValidationException
{
	public EmailAlreadyRegisteredException() { }
	public EmailAlreadyRegisteredException(string message) : base(message) { }
	public EmailAlreadyRegisteredException(string message, Exception inner) : base(message, inner) { }
}
