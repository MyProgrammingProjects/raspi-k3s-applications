using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers.Services.Dispatcher;

public class DispatcherService : IDispatcherService
{
	private readonly IServiceScopeFactory ServiceScopeFactory;

	public DispatcherService(IServiceScopeFactory serviceScopeFactory)
	{
		ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
	}

	public async Task<Result<TResult>> DispatchQueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct)
		where TResult : class?
		where TQuery : class?
	{
		//Exceptions should be dealt by calling client. In case of Controllers, they should use Global Exception Handling, so that proper HTTP Response code can be returned
		using var scope = ServiceScopeFactory.CreateScope();
		IQueryHandler<TQuery, TResult> handler = scope.ServiceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>)) as IQueryHandler<TQuery, TResult> ?? throw new ApplicationException("Query Handler cannot be instantiated");
		return await handler.HandleAsync(query, ct);


	}

	public async Task<Result<TResult>> DispatchCommandAsync<TCommand, TResult>(TCommand command, CancellationToken ct)
		where TResult : class?
		where TCommand : class?
	{
		//Exceptions should be dealt by calling client. In case of Controllers, they should use Global Exception Handling, so that proper HTTP Response code can be returned
		using var scope = ServiceScopeFactory.CreateScope();
		ICommandHandler<TCommand, TResult> handler = scope.ServiceProvider.GetService(typeof(ICommandHandler<TCommand, TResult>)) as ICommandHandler<TCommand, TResult> ?? throw new ApplicationException("Command Handler cannot be instantiated");
		return await handler.HandleAsync(command, ct);
	}
}
