using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class CommandWithEnvironment : CommandBase
    {
		public override IEnumerable<string> EnvironmentNames
        {
            get
            {
                foreach (var env in Environments)
                {
                    yield return env.Name;
                }
            }
        }

        public IEnumerable<Environment> Environments
        {
            get;
            private set;
        }

        public Environment Environment
        {
            get
            {
                return Environments.FirstOrDefault();
            }
        }

        public CommandWithEnvironment(int position, IEnumerable<Environment> environments, CommandName commandName, ArgumentSequence arguments)
            : base(position, commandName, arguments)
        {
            Environments = environments;
        }

        public CommandWithEnvironment(IEnumerable<Environment> environments, CommandName commandName, ArgumentSequence arguments)
            : base(commandName, arguments)
        {
            Environments = environments;
        }

        public CommandWithEnvironment(int position, Environment environment, CommandName commandName, ArgumentSequence arguments)
            : this(position,new Environment[]{environment}, commandName, arguments)
        {
        }

        public CommandWithEnvironment(Environment environment, CommandName commandName, ArgumentSequence arguments)
            : this (new Environment[]{environment},commandName, arguments)
        {
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }

        public override int Position
        {
            get
            {
                if (Environment == null)
                {
                    return this.CommName.Position;
                }
                return Environment.Position;
            }
        }
    }
}
