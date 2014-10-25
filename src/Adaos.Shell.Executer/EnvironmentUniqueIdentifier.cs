using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer
{
    class EnvironmentUniqueIdentifier : IEnvironmentUniqueIdentifier
    {
        public string Identifier
        {
            get;
            private set;
        }

        public EnvironmentUniqueIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        override public bool Equals(object other)
        {
            if (other is IEnvironmentUniqueIdentifier)
            {
                return this.Identifier == (other as IEnvironmentUniqueIdentifier).Identifier;
            }
            return false;
        }

		override public int GetHashCode()
		{
			return this.Identifier.GetHashCode();
		}

        public override string ToString()
        {
            return Identifier;
        }
    }
}
