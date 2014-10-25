using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class Argument : AST, IArgument
    {
        public Argument(int position, bool execute, Word wordName = null)
            : base(position)
        {
            ToExecute = execute;
            WordName = wordName;
        }

        public Argument(bool execute, Word wordName = null)
        {
            ToExecute = execute;
            WordName = wordName;
        }

        public Word WordName { get; private set; }

        public abstract string Value
        {
            get;
        }

        public virtual bool ToExecute
        {
            get;
            protected set;
        }

        public virtual string Name
        {
            get { return WordName.Spelling; }
        }

        public virtual bool HasName
        {
            get { return WordName != null; }
        }
    }
}
