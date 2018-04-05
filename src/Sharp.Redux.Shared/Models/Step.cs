using System;
using System.Diagnostics;

namespace Sharp.Redux.Shared.Models
{
    [DebuggerDisplay("{Id}")]
    public class Step
    {
        public int Id { get; set; }
        public Guid SessionId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Action { get; set; }
        public string ActionType { get; set; }
        public object State { get; set; }
    }
}
