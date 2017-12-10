using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Core
{
    /// <summary>
    /// Synchronizes reporting with UI.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks><see cref="shouldReport"/> is not UI synchronized.</remarks>
    public class UISyncedProgress<T> : IProgress<T>
    {
        readonly Action<T> handler;
        readonly Func<T, bool> shouldReport;
        readonly TaskScheduler uiScheduler;
        public UISyncedProgress(Action<T> handler, Func<T, bool> shouldReport = null)
        {
            this.handler = handler;
            this.shouldReport = shouldReport;
            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }
        public void Report(T value)
        {
            if (shouldReport?.Invoke(value) ?? true)
            {
                Task.Factory.StartNew(p => handler((T)p), value, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        }
    }
}
