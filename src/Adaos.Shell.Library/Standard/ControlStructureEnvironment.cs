using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.Core.Extenders;
using Adaos.Shell.Core;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.SyntaxAnalysis;
using Adaos.Shell.SyntaxAnalysis.ASTs;

namespace Adaos.Shell.Library.Standard
{
    public class ControlStructureEnvironment : BaseEnvironment
    {
        public override string Name => "controlstructure"; 
        virtual protected IVirtualMachine _vm { get; private set; }

        public ControlStructureEnvironment(IVirtualMachine vm)
        {
            _vm = vm;
            Bind(If, "if");
            Bind(While, "while");
        }

        public  IEnumerable<IArgument> If(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(2, x => { throw new SemanticException(-1, x); });
            //args.VerifyArgumentMaxCount(4, x => { throw new SemanticException(args.Skip(4).First().Position,x); });
            if(args.VerifyArgumentMinCount(3))
            {
                if(args.Third().Value != "else")
                {
                    throw new SemanticException(args.Third().Position, "Third argument must be 'else'");
                }
                if(!args.VerifyArgumentMinCount(4))
                {
                    throw new SemanticException(-1, "When using the 'if' command with the 'else' argument, something must come after the else argument.");
                }
            }
            
            if (ConvertToBoolean(args.First()))
            {
                return Execute(args.Second());
            }
            else if (args.ThirdOrDefault() != null)
            {
                var elseValue = args.Skip(3).First();
                if (elseValue.Value == "if")
                    return If(args.Skip(4));
                else
                    return Execute(elseValue);
            }
            return new IArgument[0];
        }

        private IEnumerable<IArgument> Execute(IArgument arg)
        {
            if (arg is IArgumentExecutable)
            {
                var argExec = arg as IArgumentExecutable;
                return _vm.ShellExecutor.Execute(argExec.ExecutionSequence, _vm);
            }
            else
            {
                return new[] { arg };
            }
        }

        public IEnumerable<IArgument> While(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentCount(2, x => { throw new SemanticException(-1, x); });

            while (ConvertToBoolean(args.First()))
            {
                foreach(var arg in Execute(args.Second()))
                {
                    yield return arg;
                }
            }
        }

        private bool ConvertToBoolean(IArgument arg)
        {
            bool res;
            Execute(arg).First().TryParseTo(out res, x => { throw new SemanticException(arg.Position, x); });
            return res;
        }
    }
}
