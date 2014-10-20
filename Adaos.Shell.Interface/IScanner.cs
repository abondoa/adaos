using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IScanner<TToken>
    {
        TToken Scan();
    }
}
