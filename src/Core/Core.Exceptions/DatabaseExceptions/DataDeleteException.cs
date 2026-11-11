using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.DatabaseExceptions;


[Serializable]
public class DataDeleteException : DatabaseException
{
	public DataDeleteException() { }
	public DataDeleteException(string message) : base(message) { }
	public DataDeleteException(string message, Exception inner) : base(message, inner) { }
}
