using Core.Exceptions.BaseExceptions;
using System;

namespace Core.Exceptions.InputDataExceptions;

[Serializable]
public class NullOrEmptyException : InvalidDataException
{
	public NullOrEmptyException() { }
	public NullOrEmptyException(string message) : base(message) { }
	public NullOrEmptyException(string message, Exception inner) : base(message, inner) { }
}

