using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public interface IReduxReducer<TState>
    {
        /// <summary>
        /// Reduces the state using the given action.
        /// </summary>
        /// <param name="state">Current state.</param>
        /// <param name="action">Action to use.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the work.</param>
        /// <returns>A task representing the result.</returns>
        Task<TState> ReduceAsync(TState state, ReduxAction action, CancellationToken ct);
    }
}
