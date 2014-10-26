﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Core;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Library.AdHoc
{
    internal class BareWord : IEnvironment
    {
        public BareWord(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public void Bind(Command command, params string[] commandNames)
        {
            throw new NotImplementedException();
        }

        public Command Retrieve(string commandName)
        {
            var res = new BareWord(commandName);

            if (res != null)
            {
                return res.SelfCommand;
            }

            return null;
        }

        public IEnumerable<string> Commands
        {
            get
            {
                yield break;
            }
        }

        public void UnBind(string commandName)
        {
            throw new NotImplementedException();
        }

        public bool AllowUnbinding
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<IEnvironmentUniqueIdentifier> Dependencies
        {
            get
            {
                yield return new EnvironmentUniqueIdentifier("barewords");
            }
        }

        public IEnumerable<IEnvironment> ChildEnvironments
        {
            get
            {
                yield break;
            }
        }

        public void AddEnvironment(IEnvironment environment)
        {
            throw new NotImplementedException();
        }

        public void RemoveEnvironment(IEnvironment environment)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<IArgument> SelfCommand(params IEnumerable<IArgument>[] args)
        {
            yield return new DummyArgument(Name);
            if (args[0].FirstOrDefault() != null)
            {
                foreach (var res in Retrieve(args[0].First().Value)(args[0].Skip(1)))
                {
                    yield return res;
                }
            }
            yield break;
        }


        public IEnvironmentUniqueIdentifier Identifier
        {
            get { throw new NotImplementedException(); }
        }


        public IEnvironment ChildEnvironment(string childEnvironmentName)
        {
            throw new NotImplementedException();
        }

        public IEnvironmentContext ToContext()
        {
            throw new NotImplementedException();
        }
    }
}
