using System;

namespace Sharp.Redux.Shared.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public DateTimeOffset ClientDateTime { get; set;}
        public string AppVersion { get; set; }
        public string UserName { get; set; }
    }
}
