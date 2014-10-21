using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class Command : AST, ICommand
    {
        public Command(int position) : base(position) 
        {
            RelationToPrevious = CommandRelation.SEPARATED;
        }
        public Command() { }

        public abstract string CommandName
        {
            get;
        }

        public abstract IEnumerable<IArgument> Arguments
        {
            get;
        }

        public abstract IEnumerable<string> EnvironmentNames
        {
            get;
        }

        int ICommand.Position
        {
            get
            {
                return (this as AST).Position;
            }
        }

        public bool IsPiped
        {
            get { return this.IsPiped(); }
        }

        public bool IsConcatenated
        {
            get { return this.IsConcatenated(); }
        }

        public CommandRelation RelationToPrevious
        {
            get;
            internal set;
        }
    }
}
