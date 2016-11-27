using Adaos.Common.Extenders;
using Adaos.Shell.Core;
using Adaos.Shell.Core.Extenders;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Library.Standard
{
    public abstract class BaseVariableEnvironment : BaseEnvironment
    {
        private int _scope;

        virtual protected IVirtualMachine _vm { get; private set; }

        public BaseVariableEnvironment(IVirtualMachine vm, bool allowUnbinding) : base(allowUnbinding)
        {
            _vm = vm;
        }

        protected IEnumerable<IArgument> HandleVariable(string variable, IEnumerable<IArgument> values, IEnumerable<IArgument> arguments, int position)
        {
            var operatorArg = arguments.FirstOrDefault();
            if (operatorArg == null)
            {
                return values;
            }

            if (operatorArg.Value == "+")
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
            else if (operatorArg.Value == "==")
            {
                return VariableFunctionBool(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x == y);
            }
            else if (operatorArg.Value == "!=")
            {
                return VariableFunctionBool(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x != y);
            }
            else if (operatorArg.Value == ">")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x > y);
            }
            else if (operatorArg.Value == "<")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x < y);
            }
            else if (operatorArg.Value == ">=")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x >= y);
            }
            else if (operatorArg.Value == "<=")
            {
                return VariableFunction(arguments.Skip(1).First().Value, values, arguments.Skip(2), position, (x, y) => x <= y);
            }
            else
            {
                throw new SemanticException(operatorArg.Position, $"Unknown operator '{operatorArg.Value}' for handling variable '{variable}'");
            }
        }

        private IEnumerable<IArgument> VariableFunction<TReturn>(string variable, IEnumerable<IArgument> values, IEnumerable<IArgument> arguments, int position, Func<int, int, TReturn> func)
        {
            var rightHandSide = _vm.ShellExecutor.Execute(new DummyExecutionSequence(new DummyExecution(variable, null, arguments)), _vm);
            foreach (var pair in values.Zip(rightHandSide, (l, r) => Tuple.Create(l, r)))
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
                yield return new DummyArgument((func(leftHand, rightHand)).ToString());
            }
        }

        private IEnumerable<IArgument> VariableFunctionBool(string variable, IEnumerable<IArgument> values, IEnumerable<IArgument> arguments, int position, Func<string, string, bool> func)
        {
            var rightHandSide = _vm.ShellExecutor.Execute(new DummyExecutionSequence(new DummyExecution(variable, null, arguments)), _vm);
            foreach (var pair in values.Zip(rightHandSide, (l, r) => Tuple.Create(l, r)))
            {
                yield return new DummyArgument((func(pair.Item1.Value, pair.Item2.Value)).ToString());
            }
        }
    }
}
