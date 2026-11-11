using System.Collections.Generic;
using System.Linq;

namespace Core.Handlers.Output;


public sealed class Result<T>
where T : class?
{
	public T Data { get; set; }
	public List<Error> Errors { get; }
	public List<Warning> Warnings { get; }
	public List<Information> Informations { get; }

	private Result(T data)
	{
		IsSuccess = true;
		Data = data;
		Errors = new List<Error>();
		Warnings = new List<Warning>();
		Informations = new List<Information>();
	}

	private Result(T data, List<Error> errors)
	{
		IsSuccess = errors.Any() == false;
		Data = data;
		Errors = errors;
		Warnings = new List<Warning>();
		Informations = new List<Information>();
	}


	public bool IsSuccess { get; private set; }

	public bool IsFailure => !IsSuccess;

	public static Result<T> Success(T data) => new Result<T>(data);
	public static Result<T?> Failure(List<Error> errors) => new Result<T?>(default, errors);

	public void AddErrors(List<Error> errors)
	{
		IsSuccess = false;
		Errors.AddRange(errors);
	}

	public void AddWarnings(List<Warning> warnings)
	{
		Warnings.AddRange(warnings);
	}

	public void AddInformations(List<Information> informations)
	{
		Informations.AddRange(informations);
	}
	public void AddError(Error error)
	{
		IsSuccess = false;
		Errors.Add(error);
	}

	public void AddWarning(Warning warning)
	{
		Warnings.Add(warning);
	}

	public void AddInformation(Information information)
	{
		Informations.Add(information);
	}
}
