using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentNested : Argument
    {
        public NestedWords Nested { get; private set; }
        public override int Position
        {
            get
            {
                return Nested.Position;
            }
        }

        public ArgumentNested(int position, bool execute, NestedWords nested, Word wordName = null)
            : base(position,execute,wordName)
        {
            Nested = nested;
        }

        public ArgumentNested(bool execute, NestedWords nested, Word wordName = null)
            : base(execute, wordName)
        {
            Nested = nested;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this,obj);
        }

        public override string Value
        {
            get { return Nested.ToString(); }
        }
    }
}
