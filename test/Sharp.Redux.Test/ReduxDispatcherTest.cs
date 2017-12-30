using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Test
{
    public class ReduxDispatcherTest
    {
        protected RootState initialState;
        protected IReduxReducer<RootState> reducer;
        protected TestDispatcher dispatcher;

        [SetUp]
        public void SetUp()
        {
            initialState = new RootState();
            reducer = Substitute.For<IReduxReducer<RootState>>();
            dispatcher = new TestDispatcher(initialState, reducer, TaskScheduler.Current);
        }
        [TestFixture]
        public class IsProcessorRunning: ReduxDispatcherTest
        {
            [Test]
            public void AfterCreation_ReturnsFalse()
            {
                Assert.That(dispatcher.IsProcessorRunning, Is.False);
            }
        }
        [TestFixture]
        public class Start: ReduxDispatcherTest
        {
            [Test]
            public void AfterStart_ProcessorIsRunning()
            {
                dispatcher.Start();

                Assert.That(dispatcher.IsProcessorRunning, Is.True);
            }
        }
        [TestFixture]
        public class ResetState: ReduxDispatcherTest
        {
            [Test]
            public async Task WhenGivenNewState_StateIsAssignedToDispatcher()
            {
                var newState = new RootState();
                bool isStateSame = false;

                dispatcher.StateChanged += (s, e) => isStateSame = ReferenceEquals(e.State, newState);
                await dispatcher.ResetStateAsync(newState);

                Assert.That(isStateSame, Is.True);
            }
            [Test]
            public async Task WhenProcessorIsRunning_ProcessorIsStopped()
            {
                await dispatcher.ResetStateAsync(new RootState());

                Assert.That(dispatcher.IsProcessorRunning, Is.False);
            }
        }
        [TestFixture]
        public class ReplayActionsAsync: ReduxDispatcherTest
        {
            [Test]
            public void WhenProcessorIsRunning_ThrowsException()
            {
                dispatcher.Start();

                Assert.ThrowsAsync<Exception>(async () => await dispatcher.ReplayActionsAsync(new ReduxAction[0], null, CancellationToken.None));
            }
            [Test]
            public async Task WhenNullActionsIsPassed_DoesNotProcessAnyAction()
            {
                await dispatcher.ReplayActionsAsync(null, null, CancellationToken.None);

                await reducer.DidNotReceiveWithAnyArgs().ReduceAsync(null, null, default(CancellationToken));
            }
            [Test]
            public async Task WhenNoActionsArePassed_DoesNotProcessAnyAction()
            {
                await dispatcher.ReplayActionsAsync(new ReduxAction[0], null, CancellationToken.None);

                await reducer.DidNotReceiveWithAnyArgs().ReduceAsync(null, null, default(CancellationToken));
            }
        }
        [TestFixture]
        public class ProcessActionAsync: ReduxDispatcherTest
        {
            [Test]
            public async Task WhenStateChanges_DoNotify()
            {
                bool hasBeenNotified = false;
                dispatcher.StateChanged += (s, e) => hasBeenNotified = true;
                reducer.ReduceAsync(null, null, default(CancellationToken)).ReturnsForAnyArgs(Task.FromResult(new RootState()));

                await dispatcher.ProcessActionAsync(new NoOpAction(), ct: default(CancellationToken));

                Assert.That(hasBeenNotified, Is.True);
            }
        }
        [TestFixture]
        public class ReplyActionsCoreAsync: ReduxDispatcherTest
        {
            [Test(Description = "Prevents infinite loops")]
            public async Task WhenActionIsPassed_ReduceOnlyOnce()
            {
                int reduceCallsCount = 0;
                reducer.ReduceAsync(null, null, default(CancellationToken)).ReturnsForAnyArgs(
                    ci => {
                        if (reduceCallsCount == 0)
                        {
                            reduceCallsCount++;
                            return new RootState();
                        }
                        else
                        {
                            throw new Exception("Called more than once");
                        }
                    });

                await dispatcher.ReplyActionsCoreAsync(new[] { new NoOpAction() }, progress: null, ct: default(CancellationToken));

                Assert.Pass();
            }
        }
        [TestFixture]
        public class OnStateChanged: ReduxDispatcherTest
        {
            [Test]
            public void WhenRunningTaskAreAddedAndDoNotEnd_WaitIndefinitely()
            {
                var tcs = new TaskCompletionSource<bool>();
                dispatcher.StateChanged += (s, e) =>
                {
                    e.AddRunningTask(tcs.Task);
                };
                var ignore = Task.Run(async () =>
                {
                    await Task.Delay(10);
                });

                var result = dispatcher.FireOnStateChangedAsync(new StateChangedEventArgs<RootState>(new NoOpAction(), new RootState())).Wait(100);

                Assert.That(result, Is.False);
            }
            [Test]
            public void WhenRunningTaskAreAdded_WaitForThem()
            {
                var tcs = new TaskCompletionSource<bool>();
                bool isCompleted = false;
                dispatcher.StateChanged += (s, e) =>
                {
                    e.AddRunningTask(tcs.Task);
                };
                var ignore = Task.Run(async () =>
                {
                    await Task.Delay(200);
                    isCompleted = true;
                    tcs.SetResult(true);
                });

                var result = dispatcher.FireOnStateChangedAsync(new StateChangedEventArgs<RootState>(new NoOpAction(), new RootState())).Wait(500);

                Assert.That(result, Is.True);
                Assert.That(isCompleted, Is.True);
            }
        }

        public class RootState
        {}

        public class NoOpAction: ReduxAction
        {}

        public class TestDispatcher : ReduxDispatcher<RootState, IReduxReducer<RootState>>
        {
            public TestDispatcher(RootState initialState, IReduxReducer<RootState> reducer, TaskScheduler notificationScheduler) : base(initialState, reducer, notificationScheduler)
            { }
            public Task FireOnStateChangedAsync(StateChangedEventArgs<RootState> e) => OnStateChangedAsync(e);
        }
    }
}
