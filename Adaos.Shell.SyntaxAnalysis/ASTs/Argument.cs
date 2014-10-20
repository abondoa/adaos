using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class Argument : AST, IArgument
    {
        public Argument(int position) : base(position) { }
        public Argument() { }

        public virtual string Value
        {
            get
            {
                return this.ToString();
            }
        }


        public abstract bool ToExecute
        {
            get;
        }
    }
}
