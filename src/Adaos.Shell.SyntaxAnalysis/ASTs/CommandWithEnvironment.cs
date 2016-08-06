using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ExecutionWithEnvironment : ExecutionBase
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

        public ExecutionWithEnvironment(int position, IEnumerable<Environment> environments, CommandName commandName, ArgumentSequence arguments)
            : base(position, commandName, arguments)
        {
            Environments = environments;
        }

        public ExecutionWithEnvironment(IEnumerable<Environment> environments, CommandName commandName, ArgumentSequence arguments)
            : base(commandName, arguments)
        {
            Environments = environments;
        }

        public ExecutionWithEnvironment(int position, Environment environment, CommandName commandName, ArgumentSequence arguments)
            : this(position,new Environment[]{environment}, commandName, arguments)
        {
        }

        public ExecutionWithEnvironment(Environment environment, CommandName commandName, ArgumentSequence arguments)
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
