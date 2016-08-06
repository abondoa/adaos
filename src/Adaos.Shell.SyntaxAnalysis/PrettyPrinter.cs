using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis
{
    public class PrettyPrinter : IVisitor
    {
        private IScannerTable _scannerTable;

        public PrettyPrinter(IScannerTable scannerTable)
        {
            _scannerTable = scannerTable;
        }

        public object Visit(ArgumentSequenceEmpty argSeq, object obj)
        {
            return "";
        }

        public object Visit(ExecutionActual comm, object obj)
        {
            var initial = GetExecutionSeparator(comm.RelationToPrevious, (bool)obj);
            return initial + comm.CommName.Visit(this, obj) + " " + comm.Args.Visit(this, obj);
        }

        public object Visit(CommandNameActual commName, object obj)
        {
            return commName.Name.Visit(this,obj);
        }

        public object Visit(ExecutionSequenceFollowActual execSeqF, object obj)
        {
            return execSeqF.Execution.Visit(this, true).ToString() + (execSeqF.FollowingExecutions?.Visit(this, obj) ?? "");
        }

        public object Visit(WordActual word, object obj)
        {
            return word.Spelling;
        }

        public object Visit(ArgumentExecutable argumentExecution, object obj)
        {
            return (argumentExecution.HasName ? argumentExecution.WordName.Visit(this,obj) + _scannerTable.ArgumentSeparator : "") +
                (argumentExecution.ToExecute ? _scannerTable.Execute : "") +
                _scannerTable.ArgumentExecutableStarter + argumentExecution.ExecutionSequence.Visit(this, obj) + _scannerTable.ArgumentExecutableStopper;
        }

        public object Visit(NestedWordsActual nest, object obj)
        {
            return nest.Nest.Spelling;
        }

        public object Visit(EnvironmentActual env, object obj)
        {
            return env.Word.Visit(this, obj);
        }

        public object Visit(WordMathSymbol word, object obj)
        {
            return word.Spelling;
        }

        public object Visit(ExecutionSequenceFollowEmpty execSeqF, object obj)
        {
            return "";
        }

        public object Visit(ExecutionSequenceActual exec, object obj)
        {
            return exec.Execution.Visit(this, false).ToString() + (exec.FollowingExecutions?.Visit(this, obj) ?? "");
        }

        public object Visit(ExecutionWithEnvironment comm, object obj)
        {
            var result = GetExecutionSeparator(comm.RelationToPrevious, (bool) obj);
            foreach (var env in comm.Environments)
            {
                result += env.Visit(this, obj) + _scannerTable.EnvironmentSeparator;
            }
            result += comm.CommName.Visit(this, obj);
            return result + comm.Args.Visit(this, obj);
        }

        private string GetExecutionSeparator(CommandRelation comm, bool obj)
        {
            string result = "";
            if (obj)
            {
                switch (comm)
                {
                    case CommandRelation.Concatenated:
                        result = _scannerTable.CommandConcatenator + " ";
                        break;
                    case CommandRelation.Piped:
                        result = " " + _scannerTable.Pipe + " ";
                        break;
                    case CommandRelation.Separated:
                        result = _scannerTable.CommandSeparator + " ";
                        break;
                }
            }

            return result;
        }

        public object Visit(ArgumentWord arg, object obj)
        {
            return (arg.HasName ? arg.WordName + _scannerTable.ArgumentSeparator : "") +
                (arg.ToExecute ? _scannerTable.Execute : "") +
                arg.Word.Visit(this, obj);
        }

        public object Visit(ArgumentSequenceActual argSeq, object obj)
        {
            return " " + argSeq.Argument.Visit(this, obj) + (argSeq.ArgumentSequence?.Visit(this, obj) ?? "");
        }

        public object Visit(ArgumentNested arg, object obj)
        {
            return (arg.HasName ? arg.WordName + _scannerTable.ArgumentSeparator : "") +
                (arg.ToExecute ? _scannerTable.Execute : "") +
                arg.Nested.Visit(this, obj);
        }
    }
}
