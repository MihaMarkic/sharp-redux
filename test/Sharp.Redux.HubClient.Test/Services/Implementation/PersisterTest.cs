using NUnit.Framework;
using Sharp.Redux.HubClient.Services.Implementation;
using Sharp.Redux.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sharp.Redux.HubClient.Test.Services.Implementation
{
    public class PersisterTest
    {
        protected Persister target;
        [SetUp]
        public void SetUp()
        {
            target = new Persister();
            target.Start(new MemoryStream());
        }
        [TearDown]
        public void TearDown()
        {
            target.Dispose();
        }
        [TestFixture]
        public class GetSessions: PersisterTest
        {
            [Test]
            public void NoSessionsByDefault()
            {
                var actual = target.GetSessions();

                Assert.That(actual, Is.Empty);
            }
            [Test]
            public void WhenSessionIsAdded_OneSessionReturned()
            {
                target.RegisterSession(new Session());

                var actual = target.GetSessions();

                Assert.That(actual.Length, Is.EqualTo(1));
            }
        }
        [TestFixture]
        public class RemoveSession: PersisterTest
        {
            [Test]
            public void WhenRemovingSessions_CorrectSessionIsRemoved()
            {
                var alpha = new Session { Id = Guid.NewGuid() };
                var beta = new Session { Id = Guid.NewGuid() };
                target.RegisterSession(alpha);
                target.RegisterSession(beta);
                target.RemoveSession(beta.Id);

                var actual = target.GetSessions();

                Assert.That(actual, Is.EquivalentTo(new Session[] { alpha }).Using(new SessionIdComparer()));
            }
        }
        [TestFixture]
        public class Remove: PersisterTest
        {
            [Test]
            public void WhenDeletingSteps_CorrectStepsAreDeleted()
            {
                var sessionId = Guid.NewGuid();
                var one = new Step { SessionId = sessionId, Id = 1 };
                var two = new Step { SessionId = sessionId, Id = 2 };
                var three = new Step { SessionId = sessionId, Id = 3 };
                target.Save(one);
                target.Save(two);
                target.Save(three);

                target.Remove(new Step[] { one, three });

                var actual = target.GetStepsFromSession(sessionId, 5);

                Assert.That(actual, Is.EquivalentTo(new Step[] { two }).Using(new StepIdComparer()));
            }
            [Test]
            public void WhenAllStepsAreNotDeleted_Throws()
            {
                var sessionId = Guid.NewGuid();
                var one = new Step { SessionId = sessionId, Id = 1 };
                var two = new Step { SessionId = sessionId, Id = 2 };
                var three = new Step { SessionId = sessionId, Id = 3 };
                target.Save(one);
                target.Save(two);

                Assert.Throws<Exception>(() => target.Remove(new Step[] { one, three }));
            }
        }
    }

    public class SessionIdComparer : IComparer<Session>
    {
        public int Compare(Session x, Session y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }
    public class StepIdComparer : IComparer<Step>
    {
        public int Compare(Step x, Step y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }
}
