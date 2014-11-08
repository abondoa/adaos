using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Core;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Library.Standard
{
    public class StandardEnvironment : BaseEnvironment
    {
        public override string Name
        {
            get { return "std"; }
        }
    }
}
