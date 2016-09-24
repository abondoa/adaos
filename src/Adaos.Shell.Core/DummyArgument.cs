using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Core
{
    public class DummyArgument : IArgument
    {
        public static IArgument False => new DummyArgument("false");
        public static IArgument True => new DummyArgument("true");
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

        public DummyArgument(int position = -1) : this(false, null, position) { }
        public DummyArgument(string value, int position = -1) : this(false, value, position) { }
        public DummyArgument(bool toExecute, int position = -1) : this(toExecute, null, position) { }
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

        public bool Equals(IArgument other)
        {
            return HasName == other.HasName &&
                (!HasName || Name.Equals(other.Name)) &&
                Position == other.Position &&
                ToExecute == other.ToExecute &&
                Value.Equals(other.Value);
        }

        public override bool Equals(object other)
        {
            if (other is IArgument)
                return Equals(other as IArgument);
            return false;
        }
    }
}
