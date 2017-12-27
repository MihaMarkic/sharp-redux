using Todo.Engine.States;
using System.Threading;
using System.Threading.Tasks;
using Sharp.Redux;
using Todo.Engine.Actions;
using Todo.Engine.Models;
using System.Linq;
using System.Collections.Immutable;
using Todo.Engine.Core;
using System;

namespace Todo.Engine.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        public Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            RootState result;
            switch (action)
            {
                case SetFilterAction setFilterAction:
                    result = state.Clone(filter: setFilterAction.Filter, filteredItems: GetFilteredItems(state.Items, setFilterAction.Filter));
                    break;
                case NewItemTextChangedAction newItemTextAction:
                    result = state.Clone(newItemText: newItemTextAction.Text);
                    break;
                case AddItemAction addItemAction:
                    {
                        var newItem = new TodoItem(addItemAction.Key, isChecked: false, text: addItemAction.Text, isEditing: false, editText: null);
                        var items = state.Items.Add(newItem);
                        result = state.Clone(newItemText: "", items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                case ChangeIsCheckedItemAction changeIsCheckedItemAction:
                    {
                        var item = state.Items.Single(i => i.Key == changeIsCheckedItemAction.Key);
                        var changedItem = item.Clone(isChecked: changeIsCheckedItemAction.IsChecked);
                        var items = state.Items.Replace(item, changedItem);
                        result = state.Clone(items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                case RemoveCompletedAction _:
                    {
                        var items = state.Items.Where(i => !i.IsChecked).ToImmutableList();
                        result = state.Clone(items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                case ToggleAllIsCheckedAction toggleAllIsCheckedAction:
                    {
                        var candidates = state.FilteredItems.Where(i => i.IsChecked != toggleAllIsCheckedAction.IsChecked).ToArray();
                        if (candidates.Length > 0)
                        {
                            var items = state.Items;
                            foreach (var item in candidates)
                            {
                                items = items.Replace(item, item.Clone(isChecked: toggleAllIsCheckedAction.IsChecked));
                            }
                            result = state.Clone(allChecked: toggleAllIsCheckedAction.IsChecked, items: items, filteredItems: GetFilteredItems(items, state.Filter));
                        }
                        else
                        {
                            result = state.Clone(allChecked: toggleAllIsCheckedAction.IsChecked);
                        }
                    }
                    break;
                case StartEditItemAction startEditItemAction:
                    {
                        var item = state.Items.Single(i => i.Key == startEditItemAction.Key);
                        var changedItem = item.Clone(isEditing: true, editText: item.Text);
                        var items = state.Items.Replace(item, changedItem);
                        result = state.Clone(items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                case EditItemChangedTextAction editItemChangedTextAction:
                    {
                        var item = state.Items.Single(i => i.IsEditing);
                        var changedItem = item.Clone(editText: editItemChangedTextAction.Text);
                        var items = state.Items.Replace(item, changedItem);
                        result = state.Clone(items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                case EndEditItemAction _:
                    {
                        var item = state.Items.Single(i => i.IsEditing);
                        var changedItem = item.Clone(isEditing: false, editText: null, text: item.EditText);
                        var items = state.Items.Replace(item, changedItem);
                        result = state.Clone(items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                case CancelEditItemAction _:
                    {
                        var item = state.Items.Single(i => i.IsEditing);
                        var changedItem = item.Clone(isEditing: false, editText: null);
                        var items = state.Items.Replace(item, changedItem);
                        result = state.Clone(items: items, filteredItems: GetFilteredItems(items, state.Filter));
                    }
                    break;
                default:
                    result = state;
                    break;
            }
            return Task.FromResult(result);
        }

        static TodoItem[] GetFilteredItems(ImmutableList<TodoItem> items, ItemsFilter filter)
        {
            switch (filter)
            {
                case ItemsFilter.All:
                    return items.ToArray();
                case ItemsFilter.Active:
                    return items.Where(i => !i.IsChecked).ToArray();
                case ItemsFilter.Complete:
                    return items.Where(i => i.IsChecked).ToArray();
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter));
            }
        }
    }
}
