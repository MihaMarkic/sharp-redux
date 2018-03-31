using System;

namespace Sharp.Redux.Shared.Models
{
    public class Step
    {
        public Guid SessionId { get; set; }
        public int Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public ReduxAction Action { get; set; } 
        public object State { get; set; }
    }
}
