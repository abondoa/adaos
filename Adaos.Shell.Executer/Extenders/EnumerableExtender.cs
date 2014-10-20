using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Executer.Extenders
{
    public static class EnumerableExtender
    {

        /// <summary>
        /// Skips until <paramref name="start"/> and then returns <paramref name="limit"/> entries or until the end is reached
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="start">The initial number of entries to skip</param>
        /// <param name="limit">The maximum number of entries to take</param>
        /// <returns>Interval starting at <paramref name="start"/> and max count of <paramref name="limit"/></returns>
        public static IEnumerable<T> Interval<T>(this IEnumerable<T> self, int start, int limit)
        {
            return self.Skip(start).Take(limit);
        }

        public static IEnumerable<T> Then<T>(this IEnumerable<T> self, IEnumerable<T> then)
        {
            foreach (T item in self)
            {
                yield return item;
            }
            foreach (T item in then)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> Tail<T>(this IEnumerable<T> self, int number) 
        {
            int toSkip = Math.Max(0,self.Count() - number);
            return self.Skip(toSkip);
        }

        public static T Second<T>(this IEnumerable<T> self)
        {
            return self.Skip(1).First();
        }

        public static T Third<T>(this IEnumerable<T> self)
        {
            return self.Skip(1).Second();
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> self)
        {
            return self.Skip(1).FirstOrDefault();
        }

        public static T ThirdOrDefault<T>(this IEnumerable<T> self)
        {
            return self.Skip(1).SecondOrDefault();
        }
    }
}
