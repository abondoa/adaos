using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface.Exceptions
{
    public class ModuleMangingException : Exception
    {
        public ModuleMangingException() : base() { }

        public ModuleMangingException(string message) 
            : base(message) { }

        public ModuleMangingException(string message, Exception innerException) 
            : base(message, innerException) { }

        public ModuleMangingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) 
            : base(info,context) { }
    }
}
