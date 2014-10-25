using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IScannerFactory<TToken>
    {
        IScanner<TToken> Create(string input);
        IScanner<TToken> Create(string input, int extraPosition);
        IScannerTable ScannerTable { get; set; }
    }
}
