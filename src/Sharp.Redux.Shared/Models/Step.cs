using System;

namespace Sharp.Redux.Shared.Models
{
    public class Step
    {
        public DateTimeOffset Time { get; set; }
        public Guid Id { get; set; }
        public ReduxAction Action { get; set; } 
        public object State { get; set; }
    }
}
