using Todo.Engine.Core;
using Righthand.Immutable;
using Todo.Engine.Models;
using System.Collections.Immutable;
using System.Linq;

namespace Todo.Engine.States
{
    public class RootState
    {
        public bool AllChecked { get; }
        public string NewItemText { get; }
        public ItemsFilter Filter { get; }
        public ImmutableList<TodoItem> Items { get; }
        public TodoItem[] FilteredItems { get; }
        public int? EditedItemId { get; }
        public string EditText { get; }

        public RootState(bool allChecked, string newItemText, ItemsFilter filter, ImmutableList<TodoItem> items, TodoItem[] filteredItems, int? editedItemId, string editText)
        {
            AllChecked = allChecked;
            NewItemText = newItemText;
            Filter = filter;
            Items = items;
            FilteredItems = filteredItems;
            EditedItemId = editedItemId;
            EditText = editText;
        }
        public bool IsEditing => FilteredItems.Any(i => i.IsEditing);
        public bool HasCompleted => Items.Any(i => i.IsChecked);
        public RootState Clone(Param<bool>? allChecked = null, Param<string>? newItemText = null, Param<ItemsFilter>? filter = null, Param<ImmutableList<TodoItem>>? items = null, Param<TodoItem[]>? filteredItems = null, Param<int?>? editedItemId = null, Param<string>? editText = null)
        {
            return new RootState(allChecked.HasValue ? allChecked.Value.Value : AllChecked,
newItemText.HasValue ? newItemText.Value.Value : NewItemText,
filter.HasValue ? filter.Value.Value : Filter,
items.HasValue ? items.Value.Value : Items,
filteredItems.HasValue ? filteredItems.Value.Value : FilteredItems,
editedItemId.HasValue ? editedItemId.Value.Value : EditedItemId,
editText.HasValue ? editText.Value.Value : EditText);
        }
    }
}
