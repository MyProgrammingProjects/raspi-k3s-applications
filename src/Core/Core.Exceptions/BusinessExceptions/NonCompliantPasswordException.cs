using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.BusinessExceptions;

[Serializable]
public class NonCompliantPasswordException : BusinessValidationException
{
	public NonCompliantPasswordException() { }
	public NonCompliantPasswordException(string message) : base(message) { }
	public NonCompliantPasswordException(string message, Exception inner) : base(message, inner) { }
}

