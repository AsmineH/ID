using NUnit.Framework;
using Repository.ImportData.Extensions;
using System;
using System.Collections.Generic;

namespace Tests.Repository.Extensions
{
    [TestFixture]
    public class MathsExtensionUnitTests
    {
        [Test]
        public void GetMedian_OddArray()
        {
            var input = new List<int>()
            {
                245,
                14,
                9,
                101,
                50
            };
            Assert.AreEqual(input.GetMedian(), 50m);
        }

        [Test]
        public void GetMedian_EvenArray()
        {
            var input = new List<int>()
            {
                84,
                87, 
                22, 
                245,
                4, 
                9, 
                100,
                101
            };
            Assert.AreEqual(input.GetMedian(), 85.5m);
        }

        [Test]
        public void GetMedian_EmptyArray()
        {
            var input = new List<int>();
            Assert.Throws<InvalidOperationException>(() => input.GetMedian());
        }

    }
}
