using System;

namespace Sharp.Redux
{
    /// <summary>
    /// Represents StateChanged event argument with a non generic state instance type.
    /// </summary>
    public class StateChangedEventArgs: EventArgs
    {
        /// <summary>
        /// Action that triggered this state change.
        /// </summary>
        public readonly ReduxAction Action;
        public readonly object State;
        /// <summary>
        /// Initializes a new instance of <see cref="StateChangedEventArgs"/> containing action that triggered change.
        /// </summary>
        /// <param name="action">Action that triggered this state change.</param>
        /// /// <param name="state">The state after the action.</param>
        public StateChangedEventArgs(ReduxAction action, object state)
        {
            Action = action;
            State = state;
        }
    }
    /// <summary>
    /// Represents StateChanged event argument.
    /// </summary>
    public class StateChangedEventArgs<TState> : StateChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StateChangedEventArgs"/> containing action that triggered change.
        /// </summary>
        /// <param name="action">Action that triggered this state change.</param>
        /// <param name="state">The state after the action.</param>
        public StateChangedEventArgs(ReduxAction action, TState state) : base(action, state)
        { }
        public new TState State => (TState)base.State;
    }
}
