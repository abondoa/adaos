using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Shell.Interface;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Shell.SyntaxAnalysis;

namespace Adaos.Shell.SyntaxAnalysis.Test
{
    [TestClass]
    public class ParserTest
    {
        private Parser _parser;
        static internal string legalInput = "system.user create ale ' 1234 4321' $\"random number '0' '100'\"";

        [TestInitialize]
        public void SetUp()
        {
            _parser = new Parser(false);
        }

        [TestCleanup]
        public void TearDown()
        {
            _parser = null;
        }

        [TestMethod]
        public void ConstructLegalParserNoArgument()
        {
            _parser = new Parser();
        }

        [TestMethod]
        public void ConstructLegalParserArgument()
        {
            _parser = new Parser(false);
        }

        [TestMethod]
        public void ParseLegal()
        {
            _parser = new Parser();
            IExecutionSequence result = _parser.Parse(legalInput);
            Assert.AreEqual(0, result.Errors.Count());
            (result as ExecutionSequence).Visit(new VisitUserCreateAle1234_4321ProgSeq(), null);
        }

        [TestMethod]
        public void ParseSlash()
        {
            _parser = new Parsing.Parser();

            IExecutionSequence result = _parser.Parse("echo /n");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void ParseTilde()
        {
            _parser = new Parsing.Parser();

            IExecutionSequence result = _parser.Parse("echo ~n");
            Assert.AreEqual(1, result.Errors.Count());
        }

        [TestMethod]
        public void ParseTildeExtensive()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo ~n");
            Assert.AreEqual(1, result.Errors.Count());

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());

        }

        [TestMethod]
        public void ParseMultipleEnvironments()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("env1.env2.command");
            Assert.AreEqual(result.Errors.Count(), 0);
            Assert.AreEqual(result.Executions.First().EnvironmentNames.ToList().Count, 2);
        }

        [TestMethod]
        public void ParseMultipleEnvironmentsNoCommand()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("env1.env1.");
            Assert.AreEqual(result.Errors.Count(), 1);
        }

        [TestMethod]
        public void ParseExecutable()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo $'view 1 si'");
            Assert.AreEqual(0, result.Errors.Count());

            Assert.IsTrue((result as ExecutionSequenceActual).Command.Arguments.First().ToExecute);

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void ParseSingleExecuteSymbolAndGetErrors()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("$");
            Assert.AreEqual(2, result.Errors.Count());
            Assert.IsInstanceOfType(result.Errors.First(),typeof(ParserException)); // Illegal token received (not word)
            Assert.IsInstanceOfType(result.Errors.Skip(1).First(), typeof(ParserException)); // Illegal EOF (due to panic mode)
        }

        [TestMethod]
        public void ParseSingleExecuteSymbolAndGetErrorWithoutPanic()
        {
            _parser = new Parser(false);

            IExecutionSequence result = _parser.Parse("$");
            Assert.AreEqual(1, result.Errors.Count());
            Assert.IsInstanceOfType(result.Errors.First(),typeof(ParserException)); // Illegal token received (not word)
        }

        [TestMethod]
        public void ParseMultipleExecuteSymbolAndGetErrors()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo $$$$");
            Assert.AreEqual(2, result.Errors.Count());
            Assert.IsInstanceOfType(result.Errors.First(),typeof(ParserException)); // Illegal token received (not word)
            Assert.IsInstanceOfType(result.Errors.Skip(1).First(), typeof(ParserException)); // Illegal EOF (due to panic mode)
        }

        [TestMethod]
        public void ParsePipedProgram()
        {
            _parser = new Parsing.Parser();

            IExecutionSequence result = _parser.Parse("view 1 si | echo");
            Assert.AreEqual(0, result.Errors.Count());

            Assert.IsTrue((result as ExecutionSequenceActual).Commands.Skip(1).First().IsPipeRecipient());

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_ConcatenatedProgram()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("sum 1 2 , sum 3 4 | echo");
            Assert.AreEqual(0, result.Errors.Count());

            Assert.AreEqual(CommandRelation.Separated,(result as ExecutionSequenceActual).Commands.First().RelationToPrevious);
            Assert.AreEqual(CommandRelation.Concatenated,(result as ExecutionSequenceActual).Commands.Skip(1).First().RelationToPrevious);
            Assert.AreEqual(CommandRelation.Piped,(result as ExecutionSequenceActual).Commands.Skip(2).First().RelationToPrevious);

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_ProgramSequenceWithExecutionSeparation_NoErrors()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo hej; echo hej");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_WithExecutableArgument_NoErrors()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo (hej)");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_WithEndingWithParenthesis_Error()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo )");
            Assert.AreEqual(1, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_EmptyExecutableArgument_Errors()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo ()");
            Assert.AreEqual(2, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_ExecutableArgumentWithoutEnd_Errors()
        {
            _parser = new Parser();

            IExecutionSequence result = _parser.Parse("echo (hej");
            Assert.AreEqual(2, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_NestedExecutableArgument_NoErrors()
        {
            IExecutionSequence result = _parser.Parse("cmd (env.cmd (env.nest))");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [TestMethod]
        public void Parse_MathSymbol_NoErrors()
        {
            IExecutionSequence result = _parser.Parse("cmd + - * / =");
            Assert.AreEqual(0, result.Errors.Count());
        }

        private class VisitUserCreateAle1234_4321ProgSeq : IVisitor
        {

            public object Visit(ArgumentNested arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceActual argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceEmpty argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentWord arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceActual prog, object obj)
            {
                Assert.AreEqual(1, prog.Position);
                prog.Command.Visit(new VisitUserCreateAle1234_4321Comm(), null);
                prog.FollowingCommands.Visit(new NullFollComm(), null);
                return null;
            }

            public object Visit(ExecutionSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowEmpty progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(EnvironmentActual env, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(NestedWordsActual nest, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentExecutable argumentExecution, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandWithEnvironment comm, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ArgumentNested arg, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ArgumentSequenceActual argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ArgumentSequenceEmpty argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ArgumentWord arg, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ExecutionActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(CommandWithEnvironment comm, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ExecutionSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ExecutionSequenceFollowEmpty progF, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(WordMathSymbol word, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(EnvironmentActual env, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(NestedWordsActual nest, object obj)
            {
                throw new NotImplementedException();
            }
        }

        private class VisitUserCreateAle1234_4321Comm : IVisitor
        {

            public object Visit(ArgumentNested arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceActual argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceEmpty argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentWord arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentExecutable argumentExecution, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowEmpty progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(EnvironmentActual env, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(NestedWordsActual nest, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(CommandWithEnvironment comm, object obj)
            {
                Assert.AreEqual(1, comm.Position);
                Assert.AreEqual("system", comm.Environment.Name);
                Assert.AreEqual(1, comm.Environment.Position);
                comm.CommName.Visit(new VisitUserCommName(), null);
                comm.Args.Visit(new VisitCreateAle1234_4321Args(), null);

                return null;
            }


            public object Visit(WordMathSymbol word, object obj)
            {
                throw new NotImplementedException();
            }
        }

        private class NullFollComm : IVisitor
        {

            public object Visit(ArgumentNested arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceActual argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceEmpty argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentWord arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentExecutable argumentExecution, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowEmpty progF, object obj)
            {
                Assert.AreEqual(63, progF.Position,"Legal input length: " + legalInput.Length + "  " + legalInput);
                return null;
            }

            public object Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(EnvironmentActual env, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(NestedWordsActual nest, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(CommandWithEnvironment comm, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(WordMathSymbol word, object obj)
            {
                throw new NotImplementedException();
            }
        }

        private class VisitUserCommName : IVisitor
        {

            public object Visit(ArgumentNested arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceActual argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceEmpty argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentWord arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                Assert.AreEqual(1+7,commName.Position);
                if (commName.Name.Spelling.Equals("user"))
                {
                    return null;
                }
                throw new Exception("Fail on comm name");
            }

            public object Visit(ArgumentExecutable argumentExecution, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowEmpty progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(EnvironmentActual env, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(NestedWordsActual nest, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(CommandWithEnvironment comm, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(WordMathSymbol word, object obj)
            {
                throw new NotImplementedException();
            }
        }

        private class VisitCreateAle1234_4321Args : IVisitor
        {

            public object Visit(ArgumentNested arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentSequenceActual argSeq, object obj)
            {
                Assert.AreEqual(("create"), argSeq.Argument.Value);
                Assert.AreEqual(13, argSeq.Argument.Position);

                Assert.AreEqual(("ale"), (argSeq.ArgumentSequence as ArgumentSequenceActual).Argument.Value);    
                Assert.AreEqual(20,(argSeq.ArgumentSequence as ArgumentSequenceActual).Argument.Position);

                List<Argument> temp = argSeq.Arguments.ToList();
                Assert.AreEqual((" 1234 4321"), temp[2].Value);        
                Assert.AreEqual(26, temp[2].Position);

                Assert.AreEqual("random number '0' '100'",temp[3].Value);
                Assert.AreEqual(39, temp[3].Position);

                return null;
                        
            }

            public object Visit(ArgumentSequenceEmpty argSeq, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentWord arg, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ArgumentExecutable argumentExecution, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ExecutionSequenceFollowEmpty progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(EnvironmentActual env, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(NestedWordsActual nest, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(CommandWithEnvironment comm, object obj)
            {
                throw new NotImplementedException();
            }


            public object Visit(WordMathSymbol word, object obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
