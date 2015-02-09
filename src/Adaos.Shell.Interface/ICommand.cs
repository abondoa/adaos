using System.Collections.Generic;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing a command node in the Adaos AST. 
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Get the collection of environment names associated with this command node.
        /// </summary>
        IEnumerable<string> EnvironmentNames { get; }

        /// <summary>
        /// Get the name of the command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Get the collection of arguments belonging to this command.
        /// </summary>
        IEnumerable<IArgument> Arguments { get; }

        /// <summary>
        /// Get the position of the command in the source string.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Get the type of relationship between this command and the previous command (if any).
        /// </summary>
        CommandRelation RelationToPrevious { get; }
    }

    /// <summary>
    /// An enum representing different ways a command can be related to other commands.
    /// </summary>
    public enum CommandRelation
    {
        /// <summary>
        /// Represents that the command is separate from the related command.
        /// </summary>
        Separated,
        /// <summary>
        /// Represents that the command is related to another command by a pipe.
        /// </summary>
        Piped,
        /// <summary>
        /// Represents that the command is concatenated with the related command.
        /// </summary>
        Concatenated
    }

    /// <summary>
    /// A static extension class for the <see cref="ICommand"/> interface.
    /// </summary>
    public static class CommandExtender
    {

        /// <summary>
        /// Check whether this command receives input from the previous command.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>True if the previous command is piped into this command.</returns>
        public static bool IsPipeRecipient(this ICommand self)
        {
            return self.RelationToPrevious == CommandRelation.Piped;
        }

        /// <summary>
        /// Check whether this command is concatenated with the previous command.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>True if this command is concatenated with the previous command.</returns>
        public static  bool IsConcatenated(this ICommand self)
        {
            return self.RelationToPrevious == CommandRelation.Concatenated;
        }
    }
}
