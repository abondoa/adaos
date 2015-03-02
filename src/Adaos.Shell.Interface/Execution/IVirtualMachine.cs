using System.Collections.Generic;
using System.IO;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Interface.Execution
{
    /// <summary>
    /// An interface describing a virtual machine used to execute commands of the Adaos language.
    /// </summary>
    public interface IVirtualMachine
    {
        /// <summary>
        /// The virtual machine will parse and execute the given command.
        /// Errors are handled by the HandleError property.
        /// </summary>
        /// <param name="command">The source command string to parse and execute.</param>
        void Execute(string command);

        /// <summary>
        /// The virtual machine will execute the given parsed command.
        /// </summary>
        /// <param name="prog">The parse tree/concrete syntax tree to execute.</param>
        IEnumerable<IArgument> Execute(IProgramSequence prog);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partialCommand"></param>
        /// <returns></returns>
        string SuggestCommand(string partialCommand);

        /// <summary>
        /// Get or set the Adaos parsed, used by the virtual machine to parse commands.
        /// </summary>
        IShellParser Parser { get; set; }

        /// <summary>
        /// Get or set the errorhandler.
        /// </summary>
        ErrorHandler HandleError { get; set; }

        /// <summary>
        /// Get or set the output stream, used when executing commands.
        /// </summary>
        StreamWriter Output { get; set; }

        /// <summary>
        /// Get or set the log stream, used for logging while executing commands.
        /// </summary>
        StreamWriter Log { get; set; }

        /// <summary>
        /// Get or set the module manager for the virtual machine.
        /// </summary>
        IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Get or set the command resolver, used to resolve command nodes in the parse tree as
        /// executable commands in the environments of the virtual machine.
        /// </summary>
        IResolver Resolver { get; set; }

        /// <summary>
        /// Get or set the container used to store the environments available to the virtual machine.
        /// </summary>
        IEnvironmentContainer EnvironmentContainer { get; set; }
    }
}

