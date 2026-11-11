using Core.Exceptions.BaseExceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SearchApi.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;

	public GlobalExceptionHandlerMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context); // Proceed to the next middleware
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		string? jsonResponse = null;
		string? type = exception.GetType().Name?.Replace("Exception", string.Empty);

		context.Response.ContentType = "application/json";

		switch (exception)
		{
			case BusinessValidationException:
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				jsonResponse = JsonConvert.SerializeObject(new
				{
					error = type,
					details = exception.Message,
					trace = exception.StackTrace
				});
				break;
			case DatabaseException:
				context.Response.StatusCode = (int)HttpStatusCode.Conflict;
				jsonResponse = JsonConvert.SerializeObject(new
				{
					error = type,
					details = exception.Message,
					trace = exception.StackTrace
				});
				break;
			case InputValidationException:
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				jsonResponse = JsonConvert.SerializeObject(new
				{
					error = type,
					details = exception.Message,
					trace = exception.StackTrace
				});
				break;
			case InvalidDataException:
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				jsonResponse = JsonConvert.SerializeObject(new
				{
					error = type,
					details = exception.Message,
					trace = exception.StackTrace
				});
				break;
			default:
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				jsonResponse = JsonConvert.SerializeObject(new
				{
					type = "Generic Error",
					details = exception.Message,
					trace = exception.StackTrace
				});
				break;
		}


		return context.Response.WriteAsync(jsonResponse);
	}
}