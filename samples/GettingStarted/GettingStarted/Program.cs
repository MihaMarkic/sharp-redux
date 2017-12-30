using System;
using System.Threading;
using System.Threading.Tasks;
using Sharp.Redux;

namespace GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initializes dispatcher with initial state, reducer instance and
            // the TaskScheduler where StateChanged event runs.
            var dispatcher = new ReduxDispatcher<RootState, Reducer>(
                // root state with counter set to 0
                initialState: new RootState(0),
                reducer: new Reducer(),
                // really doesn't matter since no UI thread is present
                notificationScheduler: TaskScheduler.Current
            );
            // handles all state changes with displaying the counter value
            dispatcher.StateChanged += (s, e) => 
                Console.WriteLine($"\nCounter is {e.State.Counter}");
            // dispatcher is all set, start it
            // it will wait for actions in a background thread
            dispatcher.Start();
            bool loop = true;
			Console.WriteLine("Type i to increment, d to decrement or any other key to exit.");
            do
            {
                char c = (char)Console.ReadKey().KeyChar;
                switch (c)
                {
                    case 'i':
                        // dispatches increment action
                        dispatcher.Dispatch(new IncrementAction());
                        break;
                    case 'd':
                        // dispatcher decrement action
                        dispatcher.Dispatch(new DecrementAction());
                        break;
                    default:
                        // exit the loop
                        loop = false;
                        break;
                }
            } while (loop);
            dispatcher.Dispose();
        }
    }
    /// <summary>
    /// This is the immutable state.
    /// </summary>
    class RootState
    {
        /// <summary>
        /// Current counter.
        /// </summary>
        /// <value>The counter.</value>
        public int Counter { get; }
        public RootState(int counter)
        {
            Counter = counter;
        }
    }
    /// <summary>
    /// Represents an increment action.
    /// </summary>
    class IncrementAction: ReduxAction
    {}
    /// <summary>
    /// Represents a decrement action.
    /// </summary>
    class DecrementAction : ReduxAction
    { }
    /// <summary>
    /// Reducer reduces state based on the action.
    /// </summary>
    class Reducer : IReduxReducer<RootState>
    {
        public Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            switch (action)
            {
                case IncrementAction _:
                    // increment counter and create new state
                    return Task.FromResult(new RootState(state.Counter + 1));
                case DecrementAction _:
                    // decrement counter and create new state
                    return Task.FromResult(new RootState(state.Counter - 1));
                default:
                    // reducer doesn't process this state, simply return state 
                    // withput modifications
                    return Task.FromResult(state);
            }
        }
    }
}
