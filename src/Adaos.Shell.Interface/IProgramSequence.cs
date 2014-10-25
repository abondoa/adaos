using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Interface
{
    public interface IProgramSequence<CommandType,ExceptionType> where CommandType : ICommand where ExceptionType : AdaosException
    {
        IEnumerable<CommandType> Commands { get; }
        IEnumerable<ExceptionType> Errors { get; }
    }

    public interface IProgramSequence : IProgramSequence<ICommand,AdaosException>
    {
    }
}
