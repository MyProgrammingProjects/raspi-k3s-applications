using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.DatabaseExceptions;


[Serializable]
public class DataUpdateException : DatabaseException
{
	public DataUpdateException() { }
	public DataUpdateException(string message) : base(message) { }
	public DataUpdateException(string message, Exception inner) : base(message, inner) { }
}
