using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Core
{
    public class DummyCommand : ICommand
    {
        public DummyCommand(string commandName,
            IEnumerable<string> environmentNames = null, 
            IEnumerable<IArgument> arguments = null,
            int position = -1,
            CommandRelation relationToPrevious = CommandRelation.SEPARATED,
            bool isPiped = false)
        {
            if (environmentNames == null)
            {
                environmentNames = new List<string>();
            }
            EnvironmentNames = environmentNames;
            CommandName = commandName;
            if (arguments == null) arguments = new List<IArgument>();
            Arguments = arguments;
            Position = position;
            RelationToPrevious = relationToPrevious;
        }

        public string EnvironmentName
        {
            get { return EnvironmentNames.FirstOrDefault(); }
        }

        public string CommandName
        {
            get;
            private set;
        }

        public IEnumerable<IArgument> Arguments
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }

        public IEnumerable<string> EnvironmentNames
        {
            get;
            private set;
        }


        public CommandRelation RelationToPrevious
        {
            get;
            private set;
        }
    }
}
