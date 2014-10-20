using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Adaos.Shell.Interface
{
    public interface IShell
    {
        StreamReader Input { get; set; }
        StreamWriter Output { get; set; }
        StreamWriter Log { get; set; }
        IVirtualMachine VirtualMachine { get; set; }
        void Start();
        void Stop();
        bool Running { get; }
    }
}
