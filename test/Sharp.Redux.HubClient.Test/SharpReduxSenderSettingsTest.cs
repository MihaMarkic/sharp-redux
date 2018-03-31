using NUnit.Framework;
using System;

namespace Sharp.Redux.HubClient.Test
{
    class SharpReduxSenderSettingsTest
    {
        [TestFixture]
        public class Constructor: SharpReduxSenderSettingsTest
        {
            [TestCase(0)]
            [TestCase(-1)]
            public void WhenBatchSizeIsLessThanOne_ThrowsArgumentOutOfRangeException(int batchSize)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new SharpReduxSenderSettings(persistData: false, true, batchSize));
            }
            [TestCase(1)]
            [TestCase(10)]
            public void WhenBatchSizeIsMoraThanZero_DoesNotThrow(int batchSize)
            {
                Assert.DoesNotThrow(() => new SharpReduxSenderSettings(persistData: false, true, batchSize));
            }
            [Test]
            public void WhenPersistDataAndNoDataFile_ThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SharpReduxSenderSettings(true, false, dataFile: null));
            }
            [Test]
            public void WhenPersistDataAndDataFile_DoesNotThrow()
            {
                Assert.DoesNotThrow(() => new SharpReduxSenderSettings(true, false, dataFile: "some/path"));
            }
        }
    }
}
