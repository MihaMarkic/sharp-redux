using System;
using System.Diagnostics;

namespace Sharp.Redux.Shared.Models
{
    [DebuggerDisplay("{Id} {Action.GetType().Name}")]
    public class Step
    {
        public int Id { get; set; }
        public Guid SessionId { get; set; }
        public DateTimeOffset Time { get; set; }
        public ReduxAction Action { get; set; } 
        public object State { get; set; }
    }
}
