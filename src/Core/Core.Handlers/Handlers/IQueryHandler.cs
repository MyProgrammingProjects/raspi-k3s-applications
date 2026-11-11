using Core.Handlers.Output;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers.Handlers;

public interface IQueryHandler<in TQuery, TResult>
where TResult : class?
where TQuery : class?
{
	Task<Result<TResult>> HandleAsync(TQuery input, CancellationToken ct);
}
