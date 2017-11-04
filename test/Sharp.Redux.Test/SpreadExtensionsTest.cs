using NUnit.Framework;

namespace Sharp.Redux.Test
{
    public class SpreadExtensionsTest
    {
        [TestFixture]
        public class Spread
        {
            [Test]
            public void WhenSourceIsEmpty_NewItemIsAdded()
            {
                var source = new string[] { };

                var actual = source.Spread("new");

                Assert.That(actual.Length, Is.EqualTo(1));
                Assert.That(actual[0], Is.EqualTo("new"));
            }
            [Test]
            public void WhenSourceContainsElement_NewItemIsAddedAtTheEnd()
            {
                var source = new string[] { "old" };

                var actual = source.Spread("new");

                Assert.That(actual.Length, Is.EqualTo(2));
                Assert.That(actual[1], Is.EqualTo("new"));
            }
            [Test]
            public void WhenNewItemIsAdded_OldItemReferenceDoesNotChange()
            {
                var source = new string[] { "old" };

                var actual = source.Spread("new");

                Assert.That(ReferenceEquals(source[0], actual[0]));
            }
        }
    }
}
