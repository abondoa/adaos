using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IProgramSequence<CommandType,ExceptionType> where CommandType : ICommand where ExceptionType : ShellException
    {
        IEnumerable<CommandType> Commands { get; }
        IEnumerable<ExceptionType> Errors { get; }
    }

    public interface IProgramSequence : IProgramSequence<ICommand,ShellException>
    {
    }
}
