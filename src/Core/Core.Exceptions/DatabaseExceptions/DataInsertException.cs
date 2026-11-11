using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.DatabaseExceptions;


[Serializable]
public class DataInsertException : DatabaseException
{
	public DataInsertException() { }
	public DataInsertException(string message) : base(message) { }
	public DataInsertException(string message, Exception inner) : base(message, inner) { }
}
