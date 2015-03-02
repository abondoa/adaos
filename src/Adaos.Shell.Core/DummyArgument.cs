using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Core
{
    public class DummyArgument : IArgument
    {
        public string Value
        {
            get;
            private set;
        }

        public bool ToExecute
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }

        public DummyArgument(int position = -1) : this(false,null,position) { }
        public DummyArgument(string value, int position = -1) : this(false,value,position) { }
        public DummyArgument(bool toExecute, int position = -1) : this(toExecute, null,position) { }
        public DummyArgument(bool toExecute, string value, int position = -1)
        {
            Value = value;
            ToExecute = toExecute;
            Position = position;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool HasName
        {
            get;
            private set;
        }
    }
}
