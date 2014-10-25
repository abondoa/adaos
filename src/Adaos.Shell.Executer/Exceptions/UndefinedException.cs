using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer.Exceptions
{
    public class UndefinedException : ShellException
    {
        public UndefinedException(int position, string message, Exception innerException)
            : base(message, innerException, position)
        {
        }

        public override string Message
        {
            get
            {
                string inner = "";
                if (InnerException != null)
                {
                    inner = "\nInner message: " + InnerException.Message;
                }
                return "Undefined Error: " + base.Message + inner;
            }
        }
    }
}
