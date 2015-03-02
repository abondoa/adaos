using System.IO;

namespace Adaos.Shell.Interface.Execution
{
    /// <summary>
    /// An interface describing a terminal used to execute source string in the Adaos language.
    /// </summary>
    public interface ITerminal
    {
        /// <summary>
        /// Get or set the input stream reader, used to collect the source input to execute.
        /// </summary>
        StreamReader Input { get; set; }

        /// <summary>
        /// Get or set the output stream.
        /// </summary>
        StreamWriter Output { get; set; }

        /// <summary>
        /// Get or set the log stream, for writing log messages.
        /// </summary>
        StreamWriter Log { get; set; }

        /// <summary>
        /// Get or set the virtual machine used to execute commands in the Adaos language.
        /// </summary>
        IVirtualMachine VirtualMachine { get; set; }

        /// <summary>
        /// Start the Adaos terminal.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the Adaos terminal.
        /// </summary>
        void Stop();

        /// <summary>
        /// Get whether the Adaos terminal is currently running. 
        /// </summary>
        bool Running { get; }
    }
}
