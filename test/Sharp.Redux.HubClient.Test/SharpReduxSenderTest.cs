using NUnit.Framework;
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
        [TestFixture]
        public class WaitForBatch: SharpReduxSenderTest
        {
            protected BlockingCollection<Step> buffer;
            protected List<Step> steps;
            [SetUp]
            public void SetUp()
            {
                buffer = new BlockingCollection<Step>();
                steps = new List<Step>();
            }
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
    }
}
