using NUnit.Framework;
using Sharp.Redux.Visualizer.Models;
using Sharp.Redux.Visualizer.Services.Implementation;

namespace Sharp.Redux.Visualizer.Test.Services.Implementation
{
    public class TreeComparerTest
    {
        [TestFixture]
        public class CreateDifferenceTree
        {
            [Test]
            public void WhenSimpleState_IsEqual_ReturnNull()
            {
                var current = new StateObjectTreeItem(new ObjectTreeItem[0], "pn", "tn", isRoot: true);
                var next = new StateObjectTreeItem(new ObjectTreeItem[0], "pn", "tn", isRoot: true);

                var actual = TreeComparer.CreateDifferenceTree(current, next);

                Assert.That(actual, Is.Null);
            }
        }
    }
}
