using NUnit.Framework;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using Sharp.Redux.Visualizer.Test.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Test.Services.Implementation
{
    public class PropertiesCollectorTest
    {
        [TestFixture]
        public class GetTypeMetadata
        {
            [Test]
            public void WhenSourceIsState_IsStateIsTrue()
            {
                var actual = PropertiesCollector.GetTypeMetadata(new State());

                Assert.That(actual.IsState, Is.True);
            }

            [Test]
            public void WhenSourceIsNotStateClass_IsStateIsFalse()
            {
                var actual = PropertiesCollector.GetTypeMetadata(new NotState());

                Assert.That(actual.IsState, Is.False);
            }

            [Test]
            public void WhenSourceIsPrimitiveType_IsStateIsFalse()
            {
                var actual = PropertiesCollector.GetTypeMetadata(5);

                Assert.That(actual.IsState, Is.False);
            }

            [Test]
            public void WhenSourceIsState_PropertiesInfoIsCollected()
            {
                var actual = PropertiesCollector.GetTypeMetadata(new State());

                Assert.That(actual.Properties.Length, Is.EqualTo(1));
                Assert.That(actual.Properties[0].Name, Is.EqualTo("Number"));
            }
        }
        [TestFixture]
        public class CollectAsync
        {
            [Test]
            public void WhenSimpleState_StateObjectDataIsUsed()
            {
                var actual = PropertiesCollector.Collect(new State());
                Assert.That(actual, Is.TypeOf<StateObjectData>());
            }
            [Test]
            public void WhenSimpleState_TypeNameIsPopulated()
            {
                var actual = PropertiesCollector.Collect(new State());
                Assert.That(actual.TypeName, Is.EqualTo("Sharp.Redux.Visualizer.Test.Services.Implementation.PropertiesCollectorTest+State"));
            }
            [Test]
            public void WhenSimpleState_PropertiesArePopulated()
            {
                var actual = (StateObjectData)PropertiesCollector.Collect(new State { Number = 5 });
                PrimitiveData numberData = (PrimitiveData)actual.Properties["Number"];
                Assert.That(numberData.Value, Is.EqualTo(5));
            }
            [Test]
            public void WhenNestedState_PropertiesArePopulated()
            {
                var actual = (StateObjectData)PropertiesCollector.Collect(new NestedState { Text = "Ala", State = new State { Number = 9 } });
                ObjectData stateProperty = actual.Properties["State"];
                Assert.That(stateProperty, Is.TypeOf<StateObjectData>());
                Assert.That(stateProperty.TypeName, Is.EqualTo("Sharp.Redux.Visualizer.Test.Services.Implementation.PropertiesCollectorTest+State"));
            }
            [Test]
            public void WhenPrimitiveType_PrimitiveDataIsUsed()
            {
                var primitive = 5;
                var actual = PropertiesCollector.Collect(primitive);
                Assert.That(actual, Is.TypeOf<PrimitiveData>());
            }
            [Test]
            public void WhenNonStateDecoratedObject_StateObjectDataIsUsed()
            {
                var actual = PropertiesCollector.Collect(new NotState());
                Assert.That(actual, Is.TypeOf<StateObjectData>());
            }
            [Test]
            public void WhenIDictionary_DictionaryDataIsUsed()
            {
                var actual = PropertiesCollector.Collect(ImmutableDictionary.Create<string, object>());
                Assert.That(actual, Is.TypeOf<DictionaryData>());
            }
            [Test]
            public void WhenIDictionary_DataIsPopulated()
            {
                var actual = (DictionaryData)PropertiesCollector.Collect(new Dictionary<string, object> { { "one", 1 }, { "two", 2 } });
                Assert.That(actual.Dictionary["one"].IsDataInt32(1));
                Assert.That(actual.Dictionary["two"].IsDataInt32(2));
            }
            [Test]
            public void WhenIEnumerable_ListDataIsUsed()
            {
                var actual = PropertiesCollector.Collect(new int[0]);
                Assert.That(actual, Is.TypeOf<ListData>());
            }
            [Test]
            public void WhenIEnumerable_ListIsPopulated()
            {
                var actual = (ListData)PropertiesCollector.Collect(new int[] { 1, 2, 3 });
                Assert.That(actual.List[0].IsDataInt32(1));
                Assert.That(actual.List[1].IsDataInt32(2));
                Assert.That(actual.List[2].IsDataInt32(3));
            }
            [Test]
            public void WhenPropertyWithNullValue_PrimitiveDataIsCreated()
            {
                var actual = (StateObjectData)PropertiesCollector.Collect(new StateWithNull());
                ObjectData property = actual.Properties["Text"];
                Assert.That(property, Is.TypeOf<PrimitiveData>());
                Assert.That(property.TypeName, Is.EqualTo(""));
                Assert.That(property.IsDataString(null));
            }
            [Test]
            public void WhenSourceIsString_ReturnIsPrimitiveData()
            {
                var actual = PropertiesCollector.Collect("yolo");
                Assert.That(actual, Is.TypeOf<PrimitiveData>());
            }
        }
        [TestFixture]
        public class ToTreeHierarchy
        {
            [Test]
            public void WhenRecursiveGraph_DoesNotRecourseIndefinitely()
            {
                var first = new RecursiveItem(1);
                var second = new RecursiveItem(2);
                var root = new RecursiveRoot(first, second);
                first.Parent = root;
                var task = Task.Run(() => PropertiesCollector.Collect(root), CancellationToken.None);

                // wait .1s ... more than enough for the task
                var success = task.Wait(TimeSpan.FromMilliseconds(100));

                Assert.IsTrue(success, "Should end in limited time");
            }
        }

        [ReduxState]
        public class State
        {
            public int Number { get; set; }
        }
        [ReduxState]
        public class NestedState
        {
            public string Text { get; set; }
            public State State { get; set; }
        }
        [ReduxState]
        public class StateWithNull
        {
            public string Text { get; set; }
        }

        public class NotState
        {
            public int Number { get; set; }
        }

        // recursive sample
        public class RecursiveRoot
        {
            public RecursiveItem[] Items { get; }
            public RecursiveRoot(params RecursiveItem[] items)
            {
                Items = items;
            }
        }

        public class RecursiveItem
        {
            public int Id { get; }
            public RecursiveRoot Parent { get; set; }
            public RecursiveItem(int id)
            {
                Id = id;
            }
        }
    }
}
