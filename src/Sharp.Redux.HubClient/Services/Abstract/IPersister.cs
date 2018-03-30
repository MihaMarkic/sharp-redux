using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubClient.Services.Abstract
{
    public interface IPersister : IDisposable
    {
        void Start(string dataFile);
        void Remove(Step[] steps);
        void Save(Step step);
    }
}
