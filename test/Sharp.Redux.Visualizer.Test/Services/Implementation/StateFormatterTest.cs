using NUnit.Framework;
using Sharp.Redux.Visualizer.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Test.Services.Implementation
{
    public class StateFormatterTest
    {
        [TestFixture]
        public class ToTreeHierarchy
        {
            //[Test]
            //public void WhenRecursiveGraph_DoesNotRecourseIndefinitely()
            //{
            //    var first = new RecursiveItem(1);
            //    var second = new RecursiveItem(2);
            //    var root = new RecursiveRoot(first, second);
            //    first.Parent = root;
            //    var task = Task.Run(() =>
            //    {
            //        var objectData = PropertiesCollector.Collect(root);
            //        var objectTreeItem = StateFormatter.ToTreeHierarchy(objectData);
            //    }, CancellationToken.None);

            //    // wait .1s ... more than enough for the task
            //    var success = task.Wait(TimeSpan.FromMilliseconds(100));

            //    Assert.IsTrue(success, "Should end in limited time");
            //}
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
