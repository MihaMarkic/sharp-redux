using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient
{
    public struct SharpReduxSenderSettings
    {
        public bool PersistData { get; }
        public bool IncludeState { get; }
        public int BatchSize { get; }
        public TimeSpan CollectionSpan { get; }
        public Func<CancellationToken, Task> WaitForConnection { get; }
        public string DataFile { get; }
        public SharpReduxSenderSettings(bool persistData, bool includeState, int batchSize = 10, TimeSpan? collectionSpan = null, Func<CancellationToken, Task> waitForConnection = null,
            string dataFile = null)
        {
            PersistData = persistData;
            IncludeState = includeState;
            BatchSize = batchSize > 0 ? batchSize : throw new ArgumentOutOfRangeException(nameof(batchSize));
            CollectionSpan = collectionSpan ?? TimeSpan.FromSeconds(5);
            WaitForConnection = waitForConnection;
            if (PersistData && string.IsNullOrEmpty(dataFile))
            {
                throw new ArgumentNullException(nameof(dataFile));
            }
            DataFile = dataFile;
        }
    }
}
