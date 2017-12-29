using System;

namespace Sharp.Redux
{
    /// <summary>
    /// Represents StateChanged event argument.
    /// </summary>
    public class StateChangedEventArgs: EventArgs
    {
        /// <summary>
        /// Action that triggered this state change.
        /// </summary>
        public readonly ReduxAction Action;
        /// <summary>
        /// Initializes a new instance of <see cref="StateChangedEventArgs"/> containing action that triggered change.
        /// </summary>
        /// <param name="action">Action that triggered this state change.</param>
        public StateChangedEventArgs(ReduxAction action)
        {
            Action = action;
        }
    }
}
