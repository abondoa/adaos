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
    }
}
