using System.Collections.Generic;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface descrbining a programsequence node of the Adaos AST. 
    /// The program sequence is a list of commands.
    /// </summary>
    /// <typeparam name="CommandType">The type of commands in the program sequence.</typeparam>
    /// <typeparam name="ExceptionType">The type of errors of the prorgamsequence.</typeparam>
    public interface IExecutionSequence<CommandType,ExceptionType> 
        where CommandType : IExecution 
        where ExceptionType : AdaosException
    {
        IEnumerable<CommandType> Executions { get; }
        IEnumerable<ExceptionType> Errors { get; }
    }

    /// <summary>
    /// An interface descrbining a programsequence node of the Adaos AST. 
    /// The program sequence is a list of <see cref="IExecution"/>s.
    /// </summary>
    public interface IExecutionSequence : IExecutionSequence<IExecution,AdaosException>
    {
    }
}
