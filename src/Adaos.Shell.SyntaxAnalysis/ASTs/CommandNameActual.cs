using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class CommandNameActual : CommandName
    {
        public Word Name { get; private set; }
        public override int Position
        {
            get
            {
                return Name.Position;
            }
        }

        public override string ToString()
        {
            return Name.Spelling;
        }

        public CommandNameActual(int position, Word name) : base(position)
        {
            Name = name;
        }

        public CommandNameActual(Word name)
        {
            Name = name;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
