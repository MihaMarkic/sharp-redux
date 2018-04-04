using System;

namespace Sharp.Redux.HubServer.Data
{
    public class SharpReduxProject
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Description { get; set; }
    }
}
