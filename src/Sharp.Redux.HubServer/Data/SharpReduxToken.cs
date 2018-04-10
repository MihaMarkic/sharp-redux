using System;

namespace Sharp.Redux.HubServer.Data
{
    /// <summary>
    /// Access token. Tokens are used by clients to either upload or download actions.
    /// </summary>
    public class SharpReduxToken
    {
        public string Id { get; set; }
        public Guid ProjectId { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
    }
}
