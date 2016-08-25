using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Common.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Common.Extenders.Tests
{
    [TestClass()]
    public class EnumerableExtenderTests
    {
        [TestMethod()]
        public void Zip_Simple()
        {
            var left = new[] { 1, 2 };
            var right = new[] { 3, 4 };

            var zipped = left.Zip(right);

            Assert.AreEqual(2, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(left.Skip(i).First(), right.Skip(i).First()), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_MoreInleft()
        {
            var left = new[] { 1, 2, 3 };
            var right = new[] { 4 };

            var zipped = left.Zip(right);

            Assert.AreEqual(3, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(left.Skip(i).First(), right.Skip(i).FirstOrDefault()), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_MoreInRight()
        {
            var left = new[] { 1 };
            var right = new[] { 2, 3, 4 };

            var zipped = left.Zip(right);

            Assert.AreEqual(3, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(left.Skip(i).FirstOrDefault(), right.Skip(i).First()), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_EmptyRight()
        {
            var left = new[] { 1, 2 };
            var right = new int[0];

            var zipped = left.Zip(right);

            Assert.AreEqual(2, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(left.Skip(i).First(), 0), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_EmptyLeft()
        {
            var left = new int[0];
            var right = new[] { 1,2 };

            var zipped = left.Zip(right);

            Assert.AreEqual(2, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(0, right.Skip(i).First()), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_MoreInleftCut()
        {
            var left = new[] { 1, 2, 3 };
            var right = new[] { 4, 5 };

            var zipped = left.Zip(right, true);

            Assert.AreEqual(2, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(left.Skip(i).First(), right.Skip(i).First()), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_MoreInRightCut()
        {
            var left = new[] { 1 };
            var right = new[] { 2, 3, 4 };

            var zipped = left.Zip(right, true);

            Assert.AreEqual(1, zipped.Count());
            for (int i = 0; i < zipped.Count(); i++)
            {
                Assert.AreEqual(new Tuple<int, int>(left.Skip(i).FirstOrDefault(), right.Skip(i).First()), zipped.Skip(i).First());
            }
        }

        [TestMethod()]
        public void Zip_EmptyRightCut()
        {
            var left = new[] { 1, 2 };
            var right = new int[0];

            var zipped = left.Zip(right, true);

            Assert.AreEqual(0, zipped.Count());
        }

        [TestMethod()]
        public void Zip_EmptyLeftCut()
        {
            var left = new int[0];
            var right = new[] { 1, 2 };

            var zipped = left.Zip(right, true);

            Assert.AreEqual(0, zipped.Count());
        }
    }
}