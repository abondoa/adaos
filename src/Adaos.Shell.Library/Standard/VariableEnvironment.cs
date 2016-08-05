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
            Bind(DeleteVariable, "delete");
        }

        private IEnumerable<IArgument> DeclareVariable(IEnumerable<IArgument> arguments)
        {
            var name = arguments.First();

            IEnvironment custom = _vm.EnvironmentContainer.LoadedEnvironments.FirstOrDefault(x => x.Name.Equals("custom"));
            if (custom == null)
            {
                throw new SemanticException(-1, "ADAOS VM does not have a custom environment loaded");
            }

            if(custom.Retrieve(name.Value) != null)
            {
                throw new SemanticException(name.Position, $"Variable '{name.Value}' already exists");
            }

            if (arguments.Skip(1).Any())
            {
                return SetVariable(name.Value, arguments.Skip(1), true);
            }
            else
            {
                return SetVariable(name.Value, new [] { new DummyArgument("=") }, true);
            }
        }

        private IEnumerable<IArgument> DeleteVariable(IEnumerable<IArgument> args)
        {
            IEnvironment custom = _vm.EnvironmentContainer.LoadedEnvironments.FirstOrDefault(x => x.Name.Equals("custom"));
            if (custom == null)
            {
                throw new SemanticException(-1, "ADAOS VM does not have a custom environment loaded");
            }
            if (custom.AllowUnbinding)
            {
                foreach (var arg in args)
                {
                    custom.Unbind(arg.Value);
                }
            }
            else
            {
                throw new SemanticException(-1, "Custom environment does not allow unbinding");
            }
            yield break;
        }

        private IEnumerable<IArgument> SetVariable(string variable, IEnumerable<IArgument> arguments, bool newVariable)
        {
            var op = arguments.First();
            var values = arguments.Skip(1);

            if (op.Value != "=")
            {
                throw new SemanticException(op.Position, "To assign a variable use '='");
            }

            IEnvironment custom = _vm.EnvironmentContainer.LoadedEnvironments.FirstOrDefault(x => x.Name.Equals("custom"));
            if (custom == null)
            {
                throw new SemanticException(-1, "ADAOS VM does not have a custom environment loaded");
            }

            Command command;
            if (values.Count() == 1 && values.First() is ArgumentExecutable)
            {
                command = (args) =>
                {
                    var arg = values.First() as ArgumentExecutable;
                    return _vm.Execute(arg.ExecutionSequence, args);
                };
            }
            else
            {
                command = (args) => 
                {
                    if (args.Flatten().Any())
                    {
                        return SetVariable(variable, args.Flatten(),false);
                    }
                    return values;
                };
            }
            if(!newVariable)
                custom.Unbind(variable);
            custom.Bind(command, variable);

            yield break;
        }
    }
}
