using System;

namespace Sharp.Redux.Shared.Models
{
    public class SessionInfo
    {
        public Guid Id { get; set; }
        public DateTimeOffset ClientDateTime { get; set; }
        public string AppVersion { get; set; }
        public string UserName { get; set; }
        public int ActionsCount { get; set; }
        public DateTimeOffset? FirstActionDate { get; set; }
        public DateTimeOffset? LastActionDate { get; set; }
    }
}
