using Sharp.Redux.Visualizer.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sharp.Redux.Visualizer.Core
{
    /// <summary>
    /// Base class that implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public abstract class NotificableObject : DisposableObject, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
