using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentNested : Argument
    {
        public NestedWords Nested { get; private set; }
        public bool Execute { get; private set; }
        public override int Position
        {
            get
            {
                return Nested.Position;
            }
        }

        public ArgumentNested(int position, bool execute, NestedWords nested)
            : base(position)
        {
            Nested = nested;
            Execute = execute;
        }

        public ArgumentNested(bool execute, NestedWords nested)
        {
            Nested = nested;
            Execute = execute;
        }

        public override string ToString()
        {
            return Nested.ToString();
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this,obj);
        }

        public override bool ToExecute
        {
            get { return Execute; }
        }
    }
}
