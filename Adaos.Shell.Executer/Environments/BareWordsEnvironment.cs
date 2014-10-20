using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Executer.Environments.AdHocEnvironments;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer.Environments
{
    public class BareWordsEnvironment : Environment
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
