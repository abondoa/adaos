using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IShellParser<ProgramType> where ProgramType : IProgramSequence
    {
        ProgramType Parse(string input);
        ProgramType Parse(string input,int initialPosition);
        IScannerTable ScannerTable { get; set; }
    }

    public interface IShellParser : IShellParser<IProgramSequence> 
    {
    }
}
