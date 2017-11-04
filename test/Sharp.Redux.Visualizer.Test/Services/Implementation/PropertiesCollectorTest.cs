using NUnit.Framework;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using Sharp.Redux.Visualizer.Test.Core;
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
            public async Task WhenSimpleState_StateObjectDataIsUsed()
            {
                var actual = await PropertiesCollector.CollectAsync(new State(), CancellationToken.None);
                Assert.That(actual, Is.TypeOf<StateObjectData>());
            }
            [Test]
            public async Task WhenSimpleState_TypeNameIsPopulated()
            {
                var actual = await PropertiesCollector.CollectAsync(new State(), CancellationToken.None);
                Assert.That(actual.TypeName, Is.EqualTo("Sharp.Redux.Visualizer.Test.Services.Implementation.State"));
            }
            [Test]
            public async Task WhenSimpleState_PropertiesArePopulated()
            {
                var actual = (StateObjectData)await PropertiesCollector.CollectAsync(new State { Number = 5 }, CancellationToken.None);
                PrimitiveData numberData = (PrimitiveData)actual.Properties["Number"];
                Assert.That(numberData.Value, Is.EqualTo(5));
            }
            [Test]
            public async Task WhenNestedState_PropertiesArePopulated()
            {
                var actual = (StateObjectData)await PropertiesCollector.CollectAsync(new NestedState { Text = "Ala", State = new State { Number = 9 } }, CancellationToken.None);
                ObjectData stateProperty = actual.Properties["State"];
                Assert.That(stateProperty, Is.TypeOf<StateObjectData>());
                Assert.That(stateProperty.TypeName, Is.EqualTo("Sharp.Redux.Visualizer.Test.Services.Implementation.State"));
            }
            [Test]
            public async Task WhenPrimitiveType_PrimitiveDataIsUsed()
            {
                var primitive = 5;
                var actual = await PropertiesCollector.CollectAsync(primitive, CancellationToken.None);
                Assert.That(actual, Is.TypeOf<PrimitiveData>());
            }
            [Test]
            public async Task WhenNonStateDecoratedObject_StateObjectDataIsUsed()
            {
                var actual = await PropertiesCollector.CollectAsync(new NotState(), CancellationToken.None);
                Assert.That(actual, Is.TypeOf<StateObjectData>());
            }
            [Test]
            public async Task WhenIDictionary_DictionaryDataIsUsed()
            {
                var actual = await PropertiesCollector.CollectAsync(ImmutableDictionary.Create<string, object>(), CancellationToken.None);
                Assert.That(actual, Is.TypeOf<DictionaryData>());
            }
            [Test]
            public async Task WhenIDictionary_DataIsPopulated()
            {
                var actual = (DictionaryData)await PropertiesCollector.CollectAsync(new Dictionary<string, object> { { "one", 1 }, { "two", 2 } }, CancellationToken.None);
                Assert.That(actual.Dictionary["one"].IsDataInt32(1));
                Assert.That(actual.Dictionary["two"].IsDataInt32(2));
            }
            [Test]
            public async Task WhenIEnumerable_ListDataIsUsed()
            {
                var actual = await PropertiesCollector.CollectAsync(new int[0], CancellationToken.None);
                Assert.That(actual, Is.TypeOf<ListData>());
            }
            [Test]
            public async Task WhenIEnumerable_ListIsPopulated()
            {
                var actual = (ListData)await PropertiesCollector.CollectAsync(new int[] { 1, 2, 3 }, CancellationToken.None);
                Assert.That(actual.List[0].IsDataInt32(1));
                Assert.That(actual.List[1].IsDataInt32(2));
                Assert.That(actual.List[2].IsDataInt32(3));
            }
            [Test]
            public async Task WhenPropertyWithNullValue_PrimitiveDataIsCreated()
            {
                var actual = (StateObjectData)await PropertiesCollector.CollectAsync(new StateWithNull(), CancellationToken.None);
                ObjectData property = actual.Properties["Text"];
                Assert.That(property, Is.TypeOf<PrimitiveData>());
                Assert.That(property.TypeName, Is.EqualTo(""));
                Assert.That(property.IsDataString(null));
            }
            [Test]
            public async Task WhenSourceIsString_ReturnIsPrimitiveData()
            {
                var actual = await PropertiesCollector.CollectAsync("yolo", CancellationToken.None);
                Assert.That(actual, Is.TypeOf<PrimitiveData>());
            }
        }
    }

    [ReduxState]
    public class State {
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
}
