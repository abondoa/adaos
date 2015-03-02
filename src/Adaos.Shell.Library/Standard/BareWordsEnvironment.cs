using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Library.AdHoc;
using Adaos.Shell.Interface;
using Adaos.Shell.Core;

namespace Adaos.Shell.Library.Standard
{
    public class BareWordsEnvironment : BaseEnvironment
    {
        public override string Name
        {
            get { return "barewords"; }
        }

        public override Command Retrieve(string commandName)
        {
            var res = base.Retrieve(commandName);
            if (res != null)
            {
                return res;
            }
            var bareWord = new BareWord(commandName);
            if (bareWord != null)
            {
                return bareWord.SelfCommand;
            }
            return null;
        }
    }
}
