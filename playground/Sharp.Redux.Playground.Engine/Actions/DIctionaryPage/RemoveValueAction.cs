﻿using Righthand.Immutable;

namespace Sharp.Redux.Playground.Engine.Actions.DictionaryPage
{
    public class RemoveValueAction: ReduxAction
    {
        public int Key { get; }

        public RemoveValueAction(int key) : base()
        {
            Key = key;
        }

        public RemoveValueAction Clone(Param<int>? key = null)
        {
            return new RemoveValueAction(key.HasValue ? key.Value.Value : Key);
        }
    }
}
