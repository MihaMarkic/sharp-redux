using NUnit.Framework;
using System;

namespace Sharp.Redux.HubClient.Test
{
    class SharpReduxManagerSettingsTest
    {
        [TestFixture]
        public class Constructor: SharpReduxManagerSettingsTest
        {
            [TestCase(0)]
            [TestCase(-1)]
            public void WhenBatchSizeIsLessThanOne_ThrowsArgumentOutOfRangeException(int batchSize)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new SharpReduxManagerSettings(persistData: false, true, batchSize));
            }
            [TestCase(1)]
            [TestCase(10)]
            public void WhenBatchSizeIsMoraThanZero_DoesNotThrow(int batchSize)
            {
                Assert.DoesNotThrow(() => new SharpReduxManagerSettings(persistData: false, true, batchSize));
            }
            [Test]
            public void WhenPersistDataAndNoDataFile_ThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SharpReduxManagerSettings(true, false, dataFile: null));
            }
            [Test]
            public void WhenPersistDataAndDataFile_DoesNotThrow()
            {
                Assert.DoesNotThrow(() => new SharpReduxManagerSettings(true, false, dataFile: "some/path"));
            }
        }
    }
}
