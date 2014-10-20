using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Executer.Extenders
{
    public static class StringExtender
    {
        public static int IndexOf(this string self, Predicate<char> predicate)
        {
            int i = 0;
            foreach(var ch in self)
            {
                if (predicate(ch))
                {
                    return i;
                }
                else
                {
                    ++i;
                }
            }
            throw new ArgumentException("No index found for given predicate");
        }
    }
}
