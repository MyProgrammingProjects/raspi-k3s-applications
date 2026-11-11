using Core.Handlers.Output;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers.Services.Dispatcher;

public interface IDispatcherService
{
	Task<Result<TResult>> DispatchCommandAsync<TCommand, TResult>(TCommand command, CancellationToken ct)
		where TResult : class?
		where TCommand : class?;
	Task<Result<TResult>> DispatchQueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct)
		where TResult : class?
		where TQuery : class?;
}
