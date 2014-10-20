using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IScannerTable
    {
        string Pipe { get; set; }
        string Execute { get; set; }
        string CommandSeparator { get; set; }
        string EnvironmentSeparator { get; set; }
        string CommandConcatenator { get; set; }
        string Escaper { get; set; }
        IScannerTable Copy();
    }
}
