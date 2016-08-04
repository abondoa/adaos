using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Common.Extenders
{
    public static class EnumUtils
    {
        public static IEnumerable<T> GetValues<T>(this Type self) 
        {
            return Enum.GetValues(self).Cast<T>();
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return typeof(T).GetValues<T>();
        }
    }
}
