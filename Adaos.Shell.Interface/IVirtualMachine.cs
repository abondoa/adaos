using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Adaos.Shell.Interface
{
    public interface IVirtualMachine
    {
        /// <summary>
        /// The virtual machine will parse and execute the given command.
        /// Errors are handled by the HandleError property.
        /// </summary>
        /// <param name="command"></param>
        void Execute(string command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partialCommand"></param>
        /// <returns></returns>
        string SuggestCommand(string partialCommand);

        IEnumerable<IEnvironment> Environments { get; }
        IEnvironment PrimaryEnvironment { get; set; }
        void LoadEnvironment(IEnvironment environment);
        void UnloadEnvironment(IEnvironment environment);
        ErrorHandler HandleError { get; set; }
        IShellParser Parser { get; set; }
        StreamWriter Output { get; set; }
        StreamWriter Log { get; set; }
    }
}
