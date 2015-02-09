using System;
using System.Collections.Generic;
using System.Linq;

namespace Adaos.Common.Extenders
{
    public static class ObjectExtender
    {
        public static IEnumerable<T> ToEnum<T>(this T self)
        {
            yield return self;
        }

        public static T Do<T>(this T self, params Action<T>[] actions)
        {
            foreach (var action in actions)
            {
                action(self);
            }
            return self;
        }
    }
}
