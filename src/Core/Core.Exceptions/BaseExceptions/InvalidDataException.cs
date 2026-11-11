using System;

namespace Core.Exceptions.BaseExceptions;


[Serializable]
public abstract class InvalidDataException : Exception
{
	protected InvalidDataException() { }
	protected InvalidDataException(string message) : base(message) { }
	protected InvalidDataException(string message, Exception inner) : base(message, inner) { }
}
