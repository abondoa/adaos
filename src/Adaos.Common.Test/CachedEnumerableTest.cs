using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Common;

namespace Adaos.Shell.Execution.Test
{
    [TestClass]
    public class CachedEnumerableTest
    {
        [TestMethod]
        public void CachedEnumerable()
        {
            var res = _cachedEnumerableOuter(new object[] { 1, 2, 3 });
            Assert.AreEqual(_cachedEnumerablCounter, 0);
            foreach (var temp in res)
            {
                temp.ToString();
            }
            Assert.AreEqual(_cachedEnumerablCounter, (1));
            foreach (var temp in res)
            {
                temp.ToString();
            }
            Assert.AreEqual(_cachedEnumerablCounter, (1));
        }

        int _cachedEnumerablCounter = 0;

        private IEnumerable<object> _cachedEnumerableOuter(IEnumerable<object> args)
        {
            return Factory.CreateCachedEnumerable(_cachedEnumerableInner(args));
        }

        private IEnumerable<object> _cachedEnumerableInner(IEnumerable<object> args)
        {
            _cachedEnumerablCounter++;
            foreach (var arg in args)
            {
                yield return arg;
            }
            yield break;
        }
    }
}
