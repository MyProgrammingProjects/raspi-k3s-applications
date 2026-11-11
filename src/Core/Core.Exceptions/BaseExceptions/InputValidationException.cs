using System;

namespace Core.Exceptions.BaseExceptions;

[Serializable]
public abstract class InputValidationException : Exception
{
	protected InputValidationException() { }
	protected InputValidationException(string message) : base(message) { }
	protected InputValidationException(string message, Exception inner) : base(message, inner) { }
}
