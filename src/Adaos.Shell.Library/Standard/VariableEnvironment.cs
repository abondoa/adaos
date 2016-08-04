using Adaos.Shell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Common.Extenders;

namespace Adaos.Shell.Library.Standard
{
    class VariableEnvironment : BaseEnvironment
    {
        public override string Name => "variable";
        virtual protected IVirtualMachine _vm { get; private set; }


        public VariableEnvironment(IVirtualMachine vm)
        {
            _vm = vm;
            Bind(DeclareVariable, "var");
        }

        private IEnumerable<IArgument> DeclareVariable(IEnumerable<IArgument> arguments)
        {
            var name = arguments.First();
            var op = arguments.Second();
            var values = arguments.Skip(2);

            //if(op.Value != "=")
            //{
            //    throw new SemanticException(op.Position, "The second argument to 'var' must be '='");
            //}

            IEnvironment custom = _vm.EnvironmentContainer.LoadedEnvironments.FirstOrDefault(x => x.Name.Equals("custom"));
            if (custom == null)
            {
                throw new SemanticException(-1, "ADAOS VM does not have a custom environment loaded");
            }

            Command command;
            if (values.Count() == 1 && values.First() is ArgumentExecutable)
                command = (args) => { return _vm.Execute((values.First() as ArgumentExecutable).ExecutionSequence, args); };
            else
                command = (args) => { return values; };
            custom.Bind(command, name.Value);

            yield break;
        }
    }
}
