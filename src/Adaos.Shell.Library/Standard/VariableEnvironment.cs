using Adaos.Shell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.SyntaxAnalysis;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Common.Extenders;
using Adaos.Shell.Core.Extenders;

namespace Adaos.Shell.Library.Standard
{
    class VariableEnvironment : BaseEnvironment
    {
        public override string Name => "variable";
        virtual protected IVirtualMachine _vm { get; private set; }


        public VariableEnvironment(IVirtualMachine vm) : base(true)
        {
            _vm = vm;
            Bind(DeclareVariable, "var");
            Bind(DeleteVariable, "delete");
        }

        public override Command Retrieve(string commandName)
        {
            int temp;
            if (int.TryParse(commandName, out temp))
            {
                return args => { return new[] { new DummyArgument(commandName) }.Then(args.Flatten()); };
            }
            else
            {
                return base.Retrieve(commandName);
            }
        }

        private IEnumerable<IArgument> DeclareVariable(IEnumerable<IArgument> arguments)
        {
            var name = arguments.First();

            if(CustomEnvironment.Retrieve(name.Value) != null)
            {
                throw new SemanticException(name.Position, $"Variable '{name.Value}' already exists");
            }

            if (arguments.Skip(1).Any())
            {
                return SetVariable(name.Value, arguments.Skip(1), true, name.Position);
            }
            else
            {
                return SetVariable(name.Value, new [] { new DummyArgument("=") }, true, name.Position);
            }
        }

        private IEnumerable<IArgument> DeleteVariable(IEnumerable<IArgument> args)
        {
            if (CustomEnvironment.AllowUnbinding)
            {
                foreach (var arg in args)
                {
                    CustomEnvironment.Unbind(arg.Value);
                }
            }
            else
            {
                throw new SemanticException(-1, "Custom environment does not allow unbinding");
            }
            yield break;
        }

        private IEnvironment CustomEnvironment
        {
            get
            {
                IEnvironment custom = _vm.EnvironmentContainer.EnabledEnvironments.FirstOrDefault(x => x.Name.Equals("custom"));
                if (custom == null)
                {
                    throw new SemanticException(-1, "ADAOS VM does not have a custom environment loaded");
                }
                return this;
            }
        }

        private IEnumerable<IArgument> SetVariable(string variable, IEnumerable<IArgument> arguments, bool newVariable, int position)
        {
            var op = arguments.First();
            var values = arguments.Skip(1);

            if (op.Value != "=")
            {
                throw new SemanticException(op.Position, "To assign a variable use '='");
            }

            Command command;
            if (values.Count() == 1 && values.First() is IArgumentExecutable)
            {
                command = (args) =>
                {
                    try
                    {
                        var arg = values.First() as IArgumentExecutable;
                        return _vm.ShellExecutor.Execute(arg.ExecutionSequence, args,_vm);
                    }
                    catch (AdaosException e)
                    {
                        throw new SemanticException(-1, $"Error during execution of function '{variable}'", e);
                    }
                };
            }
            else
            {
                command = (args) => 
                {
                    if (args.Flatten().Any())
                    {
                        return HandleVariable(variable, values, args.Flatten(), position);
                    }
                    return values;
                };
            }
            if(!newVariable)
                CustomEnvironment.Unbind(variable);
            CustomEnvironment.Bind(command, variable);

            yield break;
        }

        private IEnumerable<IArgument> HandleVariable(string variable, IEnumerable<IArgument> values, IEnumerable<IArgument> arguments, int position)
        {
            var operatorArg = arguments.FirstOrDefault();
            if (operatorArg == null)
            {
                return values;
            }

            if (operatorArg.Value == "=")
            {
                return SetVariable(variable, arguments, false, position);
            }
            else if (operatorArg.Value == "+")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x + y);
            }
            else if (operatorArg.Value == "-")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x - y);
            }
            else if (operatorArg.Value == "*")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x * y);
            }
            else if (operatorArg.Value == "/")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x / y);
            }
            else
            {
                throw new SemanticException(operatorArg.Position, $"Unknown operator '{operatorArg.Value}' for handling variable '{variable}'");
            }
        }

        private IEnumerable<IArgument> VariableFunction(string variable, IEnumerable<IArgument> values, IEnumerable<IArgument> arguments, int position, Func<int,int,int> func)
        {
            var rightHandSide = CustomEnvironment.Retrieve(variable)?.Invoke(new[] { arguments });
            foreach (var pair in values.Zip(rightHandSide, (l,r) => Tuple.Create(l,r)))
            {
                int leftHand = 0;
                int rightHand = 0;
                if (!pair.Item1.TryParseTo(out leftHand))
                {
                    throw new SemanticException(position, $"Variable '{variable}' does not contain integers");
                }
                if (!pair.Item2.TryParseTo(out rightHand))
                {
                    throw new SemanticException(position, $"Variable '{pair.Item2.Name}' does not contain integers");
                }
                yield return new DummyArgument((func(leftHand,rightHand)).ToString());
            }
        }
    }
}
