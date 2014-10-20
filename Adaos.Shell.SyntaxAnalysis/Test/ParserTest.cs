using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Shell.Interface;
using Adaos.Shell.SyntaxAnalysis.Exceptions;

namespace Adaos.Shell.SyntaxAnalysis.Test
{
    [TestFixture]
    public class ParserTest
    {
        private Parsing.Parser _parser;
        static internal string legalInput = "system.user create ale ' 1234 4321' @\"random number '0' '100'\"";

        [SetUp]
        public void SetUp()
        { }

        [TearDown]
        public void TearDown()
        {
            _parser = null;
        }

        [Test]
        public void ConstructLegalParserNoArgument()
        {
            _parser = new Parsing.Parser();
        }

        [Test]
        public void ConstructLegalParserArgument()
        {
            _parser = new Parsing.Parser(false);
        }

        [Test]
        public void ParseLegal()
        {
            _parser = new Parsing.Parser();
            IProgramSequence result = _parser.Parse(legalInput);
            Assert.AreEqual(0, result.Errors.Count());
            try
            {
                (result as ProgramSequence).Visit(new VisitUserCreateAle1234_4321ProgSeq(), null);
            }
            catch (NotImplementedException e)
            {
                Assert.Fail("NotImplementedException thrown: " + e.Message);
                throw;
            }
            catch (Exception e)
            {
                Assert.Fail("Exception thrown: " + e.Message);
                throw;
            }
        }

        [Test]
        public void ParseSlash()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("echo /n");
            Assert.AreEqual(1, result.Errors.Count());

        }

        [Test]
        public void ParseSlashExtensive()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("echo /n");
            Assert.AreEqual(1, result.Errors.Count());

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());

        }

        [Test]
        public void ParseMultipleEnvironments()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("env1.env2.command");
            Assert.That(result.Errors, Has.Count.EqualTo(0));
            Assert.That(result.Commands.First().EnvironmentNames.ToList(), Has.Count.EqualTo(2));
        }

        [Test]
        public void ParseMultipleEnvironmentsNoCommand()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("env1.env1.");
            Assert.That(result.Errors, Has.Count.AtLeast(1));
        }

        [Test]
        public void ParseExecutable()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("echo @'view 1 si'");
            Assert.AreEqual(0, result.Errors.Count());

            Assert.IsTrue((result as ProgramSequenceActual).Command.Arguments.First().ToExecute);

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [Test]
        public void ParseSingleAtAndGetErrors()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("@");
            Assert.AreEqual(2, result.Errors.Count());
            Assert.IsInstanceOf<ParserException>(result.Errors.First()); // Illegal token received (not word)
            Assert.IsInstanceOf<ParserException>(result.Errors.Skip(1).First()); // Illegal EOF (due to panic mode)
        }

        [Test]
        public void ParseSingleAtAndGetErrorWithoutPanic()
        {
            _parser = new Parsing.Parser(false);

            IProgramSequence result = _parser.Parse("@");
            Assert.AreEqual(1, result.Errors.Count());
            Assert.IsInstanceOf<ParserException>(result.Errors.First()); // Illegal token received (not word)
        }

        [Test]
        public void ParseMultipleAtAndGetErrors()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("echo @@@@");
            Assert.AreEqual(2, result.Errors.Count());
            Assert.IsInstanceOf<ParserException>(result.Errors.First()); // Illegal token received (not word)
            Assert.IsInstanceOf<ParserException>(result.Errors.Skip(1).First()); // Illegal EOF (due to panic mode)
        }

        [Test]
        public void ParsePipedProgram()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("view 1 si | echo");
            Assert.AreEqual(0, result.Errors.Count());

            Assert.IsTrue((result as ProgramSequenceActual).Commands.Skip(1).First().IsPiped);

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [Test]
        public void ParseConcatenatedProgram()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("sum 1 2 , sum 3 4 | echo");
            Assert.AreEqual(0, result.Errors.Count());

            Assert.AreEqual((result as ProgramSequenceActual).Commands.First(),CommandRelation.CONCATENATED);
            Assert.AreEqual((result as ProgramSequenceActual).Commands.First(), CommandRelation.PIPED);
            Assert.AreEqual((result as ProgramSequenceActual).Commands.Skip(1).First(), CommandRelation.CONCATENATED);
            Assert.AreEqual((result as ProgramSequenceActual).Commands.Skip(2).First(), CommandRelation.PIPED);

            result = _parser.Parse("echo test");
            Assert.AreEqual(0, result.Errors.Count());

            result = _parser.Parse("user.create un a pass b email cdefg");
            Assert.AreEqual(0, result.Errors.Count());
        }

        [Test]
        public void ParseProgramSequence()
        {
            _parser = new Parsing.Parser();

            IProgramSequence result = _parser.Parse("echo hej; echo hej");
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

            public object Visit(CommandActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceActual prog, object obj)
            {
                Assert.AreEqual(1, prog.Position);
                prog.Command.Visit(new VisitUserCreateAle1234_4321Comm(), null);
                prog.FollowingCommands.Visit(new NullFollComm(), null);
                return null;
            }

            public object Visit(ProgramSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowEmpty progF, object obj)
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

            object IVisitor.Visit(CommandActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(CommandWithEnvironment comm, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ProgramSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ProgramSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(ProgramSequenceFollowEmpty progF, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(WordActual word, object obj)
            {
                throw new NotImplementedException();
            }

            object IVisitor.Visit(WordSymbol word, object obj)
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

            public object Visit(CommandActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowEmpty progF, object obj)
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


            public object Visit(WordSymbol word, object obj)
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

            public object Visit(CommandActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowEmpty progF, object obj)
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


            public object Visit(WordSymbol word, object obj)
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

            public object Visit(CommandActual comm, object obj)
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

            public object Visit(ProgramSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowEmpty progF, object obj)
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


            public object Visit(WordSymbol word, object obj)
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

            public object Visit(CommandActual comm, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(CommandNameActual commName, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceActual prog, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowActual progF, object obj)
            {
                throw new NotImplementedException();
            }

            public object Visit(ProgramSequenceFollowEmpty progF, object obj)
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


            public object Visit(WordSymbol word, object obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
