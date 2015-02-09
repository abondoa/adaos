using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Common.Enumerable;
using Adaos.Common.Interface;

namespace Adaos.Common
{
    /// <summary>
    /// Handles all external construction requests of classes in Adaos.Common
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Create an ICachedEnumerable based on an existing IEnumerable. The inner IEnumerable
        /// will only be run through at most once.
        /// </summary>
        /// <typeparam name="T">The type contained in the IEnumerable</typeparam>
        /// <param name="inner">The IEnumerable, which we want to cache</param>
        /// <returns>An IEnumerable which is cached, such that the inner is only run through at most once</returns>
        public static ICachedEnumerable<T> CreateCachedEnumerable<T>(IEnumerable<T> inner)
        {
            return new CachedEnumerable<T>(inner);
        }
    }
}
