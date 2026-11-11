using Core.Handlers.Output;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers.Handlers;

public interface ICommandHandler<in TCommand, TResult>
	where TResult : class?
	where TCommand : class?
{
	Task<Result<TResult>> HandleAsync(TCommand input, CancellationToken ct);
}
