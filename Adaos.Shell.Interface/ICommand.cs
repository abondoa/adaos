using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface ICommand
    {
        IEnumerable<string> EnvironmentNames { get; }
        string CommandName { get; }
        IEnumerable<IArgument> Arguments { get; }
        int Position { get; }
        CommandRelation RelationToPrevious { get; }
    }

    public enum CommandRelation
    {
        SEPARATED,
        PIPED,
        CONCATENATED
    }

    public static class CommandExtender
    {
        public static bool IsPiped(this ICommand self)
        {
            return self.RelationToPrevious == CommandRelation.PIPED;
        }

        public static  bool IsConcatenated(this ICommand self)
        {
            return self.RelationToPrevious == CommandRelation.CONCATENATED;
        }
    }
}
