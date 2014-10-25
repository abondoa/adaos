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
        object Visit(CommandActual comm, object obj);
        object Visit(CommandWithEnvironment comm, object obj);
        object Visit(CommandNameActual commName, object obj);
        object Visit(ProgramSequenceActual prog, object obj);
        object Visit(ProgramSequenceFollowActual progF, object obj);
        object Visit(ProgramSequenceFollowEmpty progF, object obj);
        object Visit(WordActual word, object obj);
        object Visit(WordSymbol word, object obj);
        object Visit(EnvironmentActual env, object obj);
        object Visit(NestedWordsActual nest, object obj);
    }
}
