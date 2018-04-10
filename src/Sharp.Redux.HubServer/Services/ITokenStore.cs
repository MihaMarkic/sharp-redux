using Sharp.Redux.HubServer.Data;
using System;

namespace Sharp.Redux.HubServer.Services
{
    public interface ITokenStore
    {
        SharpReduxToken AddWriteToken(Guid projectId);
        SharpReduxToken AddReadToken(Guid projectId);
        SharpReduxToken Get(string id);
        SharpReduxToken[] GetForProject(Guid projectId);
    }
}
