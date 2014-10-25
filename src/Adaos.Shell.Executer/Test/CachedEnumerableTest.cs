using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.Executer.Environments;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Executer.Extenders;
using Adaos.Shell.Executer.CachedEnumerable;

namespace Adaos.Shell.Executer.Test
{
    [TestFixture]
    public class CachedEnumerableTest
    {
        [Test]
        public void CachedEnumerable()
        {
            var res = _cachedEnumerableOuter(new object[] { 1, 2, 3 });
            Assert.That(_cachedEnumerablCounter, Is.EqualTo(0));
            foreach (var temp in res)
            {
                temp.ToString();
            }
            Assert.That(_cachedEnumerablCounter, Is.EqualTo(1));
            foreach (var temp in res)
            {
                temp.ToString();
            }
            Assert.That(_cachedEnumerablCounter, Is.EqualTo(1));
        }

        int _cachedEnumerablCounter = 0;

        private IEnumerable<object> _cachedEnumerableOuter(IEnumerable<object> args)
        {
            return new CachedEnumerable<object>(_cachedEnumerableInner(args));
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
