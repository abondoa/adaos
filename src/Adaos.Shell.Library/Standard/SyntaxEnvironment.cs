using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.SyntaxAnalysis;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Core.Extenders;
using Adaos.Shell.Core;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Library.Standard
{
    class SyntaxEnvironment : BaseEnvironment
    {
        private IVirtualMachine _vm;
        private IShellParser Parser { get { return _vm.Parser; } }

        public override string Name
        {
            get { return "syntax"; }
        }

        public SyntaxEnvironment(IVirtualMachine vm)
        {
            if (vm == null)
            {
                throw new ArgumentNullException("SyntaxEnvironment requires an actual IVirtualMachine.");
            }
            _vm = vm;
            Bind(Pipe, "pipe", "commandpipe","cmdpipe");
            Bind(Execute, "execute", "exec");
            Bind(CommandSeparator, "commandseparator", "cmdsep");
            Bind(CommandConcatenator, "commandconcatenator", "cmdconcat");
            Bind(EnvironmentSeparator, "environmentseparator", "envsep");
            Bind(Escaper, "escaper", "esc");
        }

        private IEnumerable<IArgument> Pipe(IEnumerable<IArgument> args)
        {
            return HandleScannerTable(args, x => x.Pipe, (x, value) => x.Pipe = value);
        }

        private IEnumerable<IArgument> CommandConcatenator(IEnumerable<IArgument> args)
        {
            return HandleScannerTable(args, x => x.CommandConcatenator, (x, value) => x.CommandConcatenator = value);
        }

        private IEnumerable<IArgument> Execute(IEnumerable<IArgument> args)
        {
            return HandleScannerTable(args, x => x.Execute, (x, value) => x.Execute = value);
        }

        private IEnumerable<IArgument> CommandSeparator(IEnumerable<IArgument> args)
        {
            return HandleScannerTable(args, x => x.CommandSeparator, (x, value) => x.CommandSeparator = value);
        }

        private IEnumerable<IArgument> EnvironmentSeparator(IEnumerable<IArgument> args)
        {
            return HandleScannerTable(args, x => x.EnvironmentSeparator, (x, value) => x.EnvironmentSeparator = value);
        }

        private IEnumerable<IArgument> Escaper(IEnumerable<IArgument> args)
        {
            return HandleScannerTable(args, x => x.Escaper, (x, value) => x.Escaper = value);
        }

        private IEnumerable<IArgument> HandleScannerTable(IEnumerable<IArgument> args, Func<IScannerTable, string> getter, Action<IScannerTable, string> setter)
        { 
            args.VerifyArgumentCount(0, 1, x => { throw new SemanticException(-1,x); });
            string curPipe = getter(Parser.ScannerTable);
            IArgument arg = args.FirstOrDefault();
            if (arg != null)
            {
                setter(Parser.ScannerTable, arg.Value);
            }
            yield return new DummyArgument(curPipe);
        }
    }
}
