using System;

namespace Core.Exceptions.BaseExceptions;


[Serializable]
public abstract class DatabaseException : Exception
{
	protected DatabaseException() { }
	protected DatabaseException(string message) : base(message) { }
	protected DatabaseException(string message, Exception inner) : base(message, inner) { }
}
