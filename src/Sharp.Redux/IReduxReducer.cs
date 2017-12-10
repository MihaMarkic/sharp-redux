using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public interface IReduxReducer<TState>
    {
        Task<TState> ReduceAsync(TState state, ReduxAction action, CancellationToken ct);
    }
}
