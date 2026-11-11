using System;

namespace Core.Exceptions.BaseExceptions;

[Serializable]
public abstract class BusinessValidationException : Exception
{
	protected BusinessValidationException() { }
	protected BusinessValidationException(string message) : base(message) { }
	protected BusinessValidationException(string message, Exception inner) : base(message, inner) { }
}
