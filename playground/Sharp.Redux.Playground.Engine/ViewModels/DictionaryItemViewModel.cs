using Sharp.Redux.Playground.Engine.Core;

namespace Sharp.Redux.Playground.Engine.ViewModels
{
    public class DictionaryItemViewModel<T> : NotificableObject, IBoundViewModel<T>
    {
        public T State { get; private set; }
        public DictionaryItemViewModel(T data)
        {
            Update(data);
        }
        public void Update(T state)
        {
            State = state;
        }
    }
}
