using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.SyntaxAnalysis.ASTs;

namespace Adaos.Shell.SyntaxAnalysis
{
    public interface IVisitor
    {
        object Visit(ArgumentNested arg, object obj);
        object Visit(ArgumentSequenceActual argSeq, object obj);
        object Visit(ArgumentSequenceEmpty argSeq, object obj);
        object Visit(ArgumentWord arg, object obj);
        object Visit(ExecutionActual comm, object obj);
        object Visit(CommandWithEnvironment comm, object obj);
        object Visit(CommandNameActual commName, object obj);
        object Visit(ExecutionSequenceActual prog, object obj);
        object Visit(ExecutionSequenceFollowActual progF, object obj);
        object Visit(ExecutionSequenceFollowEmpty progF, object obj);
        object Visit(WordActual word, object obj);
        object Visit(WordMathSymbol word, object obj);
        object Visit(ArgumentExecutable argumentExecution, object obj);
        object Visit(EnvironmentActual env, object obj);
        object Visit(NestedWordsActual nest, object obj);
    }
}
