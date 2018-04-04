using NSubstitute;
using NUnit.Framework;
using Sharp.Redux.HubClient.Models;
using Sharp.Redux.HubClient.Services.Abstract;
using Sharp.Redux.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient.Test
{
    public class SharpReduxSenderTest
    {
        protected BlockingCollection<Step> buffer;
        protected List<Step> steps;
        [SetUp]
        public void SetUp()
        {
            buffer = new BlockingCollection<Step>();
            steps = new List<Step>();
        }

        [TestFixture]
        public class WaitForBatch: SharpReduxSenderTest
        {
            [Test]
            public async Task WhenNoStepsInGivenTime_TaskNotCompleted()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, default));
                await Task.Delay(timeout);

                bool isCompleted = task.IsCompleted;

                Assert.That(isCompleted, Is.False);
            }
            [Test]
            public async Task WhenNoStepsInGivenTimeAndCancelled_ReturnsFalse()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var cts = new CancellationTokenSource();
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, cts.Token));
                cts.Cancel();
                await Task.Delay(timeout);

                bool isCompleted = task.IsCompleted;
                bool? actual = isCompleted ? task.Result : (bool?)null;

                Assert.That(actual, Is.False);
            }
            [Test]
            public async Task WhenOnlySingleStep_TaskCompletedAfterTimeout()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, default));
                buffer.Add(new Step());
                await Task.Delay(TimeSpan.FromMilliseconds(150));

                bool isCompleted = task.IsCompleted;

                Assert.That(isCompleted, Is.True);
            }
            [Test]
            public async Task WhenOnlySingleStep_ResultIsTrue()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, default));
                buffer.Add(new Step());
                await Task.Delay(TimeSpan.FromMilliseconds(150));

                bool isCompleted = task.IsCompleted;
                bool? actual = isCompleted ? task.Result : (bool?)null;

                Assert.That(actual, Is.True);
            }
            [Test]
            public async Task WhenOnlySingleStep_StepsContainsStep()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, default));
                Step step = new Step();
                buffer.Add(step);
                await Task.Delay(TimeSpan.FromMilliseconds(150));

                Assert.That(steps.ToArray(), Is.EquivalentTo(new Step[] { step }));
            }
            [Test]
            public async Task WhenCancelledAfterSingleStep_ReturnsFalse()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var cts = new CancellationTokenSource();
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, cts.Token));
                buffer.Add(new Step());
                cts.Cancel();
                await Task.Delay(timeout);

                bool isCompleted = task.IsCompleted;
                bool? actual = isCompleted ? task.Result : (bool?)null;

                Assert.That(actual, Is.False);
            }
            [Test]
            public async Task WhenTwoSteps_StepsContainsBoth()
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(100);
                var task = Task.Run(() => SharpReduxSender.WaitForBatch(buffer, steps, 10, timeout, default));
                Step step1 = new Step();
                buffer.Add(step1);
                Step step2 = new Step();
                buffer.Add(step2);
                await Task.Delay(TimeSpan.FromMilliseconds(150));

                Assert.That(steps.ToArray(), Is.EquivalentTo(new Step[] { step1, step2 }));
            }
        }
        [TestFixture]
        public class CreateStepFromStateChange: SharpReduxSenderTest
        {
            [Test]
            public void WhenStateIsNotRequired_StateIsNotMapped()
            {
                var e = new StateChangedEventArgs(new DummyAction(), new object());

                var actual = SharpReduxSender.CreateStepFromStateChange(Guid.NewGuid(), 1, new SharpReduxSenderSettings(false, false), e);

                Assert.That(actual.State, Is.Null);
            }
            [Test]
            public void WhenStateIsRequired_StateIsMapped()
            {
                object state = new object();
                var e = new StateChangedEventArgs(new DummyAction(), state);

                var actual = SharpReduxSender.CreateStepFromStateChange(Guid.NewGuid(), 1, new SharpReduxSenderSettings(false, true), e);

                Assert.That(actual.State, Is.SameAs(state));
            }
        }
        [TestFixture]
        public class ProcessPersistedAsync: SharpReduxSenderTest
        {
            protected IPersister persister;
            protected IReduxDispatcher dispatcher;
            protected ICommunicator communicator;
            protected SharpReduxSender target;
            protected SharpReduxSenderSettings settings;
            [SetUp]
            public new void SetUp()
            {
                persister = Substitute.For<IPersister>();
                dispatcher = Substitute.For<IReduxDispatcher>();
                communicator = Substitute.For<ICommunicator>();
                settings = new SharpReduxSenderSettings(true, false, dataFile: "test.file");
                target = new SharpReduxSender(Guid.NewGuid(), serverUri: new Uri("https://blog.rthand.com/"), dispatcher,
                    new SessionInfo("tVersion", "user"),settings, persister, communicator);
            }
            [Test]
            public async Task WhenNoSessions_DoesNothing()
            {
                persister.GetSessions().Returns(new Session[0]);

                await target.ProcessPersistedAsync(default);

                persister.DidNotReceiveWithAnyArgs().GetStepsFromSession(Guid.Empty, 0);
            }
            [Test]
            public async Task WhenSessionAndNoSteps_GetStepsFromSessionIsCalled()
            {
                var session = new Session { Id = Guid.NewGuid() };
                persister.GetSessions().Returns(new Session[] { session });

                await target.ProcessPersistedAsync(default);

                persister.Received().GetStepsFromSession(session.Id,settings.BatchSize);
            }
            [Test]
            public async Task WhenSessionAndNoSteps_DoesNotUpload()
            {
                var session = new Session { Id = Guid.NewGuid() };
                persister.GetSessions().Returns(new Session[] { session });

                await target.ProcessPersistedAsync(default);

                await communicator.DidNotReceiveWithAnyArgs().UploadStepsAsync(default, default);
            }
            [Test]
            public async Task WhenSessionAndNoSteps_RemovesSession()
            {
                var session = new Session { Id = Guid.NewGuid() };
                persister.GetSessions().Returns(new Session[] { session });

                await target.ProcessPersistedAsync(default);

                persister.Received().RemoveSession(session.Id);
            }
            [Test]
            public async Task WhenSessionAndOneStep_Uploads()
            {
                bool didReturn = false;
                var session = new Session { Id = Guid.NewGuid() };
                var steps = new Step[] { new Step { Id = 1, SessionId = Guid.NewGuid() } };
                persister.GetSessions().Returns(new Session[] { session });
                persister.GetStepsFromSession(session.Id, settings.BatchSize).Returns(ci =>
                {
                    if (!didReturn)
                    {
                        didReturn = true;
                        return steps;
                    }
                    else
                    {
                        return new Step[0];
                    }
                });


                await target.ProcessPersistedAsync(default);

                await communicator.Received().UploadStepsAsync(steps, default);
            }
            [Test]
            public async Task WhenSessionAndOneStep_RemovesPersistedSteps()
            {
                bool didReturn = false;
                var session = new Session { Id = Guid.NewGuid() };
                var steps = new Step[] { new Step { Id = 1, SessionId = Guid.NewGuid() } };
                persister.GetSessions().Returns(new Session[] { session });
                persister.GetStepsFromSession(session.Id, settings.BatchSize).Returns(ci =>
                {
                    if (!didReturn)
                    {
                        didReturn = true;
                        return steps;
                    }
                    else
                    {
                        return new Step[0];
                    }
                });


                await target.ProcessPersistedAsync(default);

                persister.Received().Remove(steps);
            }
        }
    }
    public class DummyAction: ReduxAction
    { }
}
