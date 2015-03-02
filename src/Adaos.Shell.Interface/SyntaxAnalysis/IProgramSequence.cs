using System.Collections.Generic;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Interface.SyntaxAnalysis
{
    /// <summary>
    /// An interface descrbining a programsequence node of the Adaos AST. 
    /// The program sequence is a list of commands.
    /// </summary>
    /// <typeparam name="CommandType">The type of commands in the program sequence.</typeparam>
    /// <typeparam name="ExceptionType">The type of errors of the prorgamsequence.</typeparam>
    public interface IProgramSequence<CommandType,ExceptionType> 
        where CommandType : ICommand 
        where ExceptionType : AdaosException
    {
        IEnumerable<CommandType> Commands { get; }
        IEnumerable<ExceptionType> Errors { get; }
    }

    /// <summary>
    /// An interface descrbining a programsequence node of the Adaos AST. 
    /// The program sequence is a list of <see cref="ICommand"/>s.
    /// </summary>
    public interface IProgramSequence : IProgramSequence<ICommand,AdaosException>
    {
    }
}
