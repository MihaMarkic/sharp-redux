using Righthand.Immutable;
using Sharp.Redux;

namespace Todo.Engine.Models
{
    public class TodoItem: IKeyedItem<int>
    {
        public int Key { get; }
        public bool IsChecked { get; }
        public string Text { get; }
        public bool IsEditing { get; }
        public string EditText { get; }

        public TodoItem(int key, bool isChecked, string text, bool isEditing, string editText)
        {
            Key = key;
            IsChecked = isChecked;
            Text = text;
            IsEditing = isEditing;
            EditText = editText;
        }

        public TodoItem Clone(Param<int>? key = null, Param<bool>? isChecked = null, Param<string>? text = null, Param<bool>? isEditing = null, Param<string>? editText = null)
        {
            return new TodoItem(key.HasValue ? key.Value.Value : Key,
isChecked.HasValue ? isChecked.Value.Value : IsChecked,
text.HasValue ? text.Value.Value : Text,
isEditing.HasValue ? isEditing.Value.Value : IsEditing,
editText.HasValue ? editText.Value.Value : EditText);
        }
    }
}
