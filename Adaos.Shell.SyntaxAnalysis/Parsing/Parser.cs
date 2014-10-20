using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Shell.SyntaxAnalysis.Tokens;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.SyntaxAnalysis.Scanning;

namespace Adaos.Shell.SyntaxAnalysis.Parsing
{
    public class Parser : IShellParser
    {
        private IScanner<Token> _scanner;
        private Token _currentToken;
        private List<ParserException> Errors {get; set;}

        public bool Panic { get; private set; }

        IScannerFactory<Token> ScannerFactory { get; set; }

        public IScannerTable ScannerTable { get { return ScannerFactory.ScannerTable; } set { ScannerFactory.ScannerTable = value; } }

        public Parser(bool panic = true)
        {
            Panic = panic;
            ScannerFactory = new ScannerFactory(new ScannerTable());
        }

        public IProgramSequence Parse(string input)
        {
            return Parse(input, 0);
        }

        public IProgramSequence Parse(string input, int initialPosition)
        {
            Errors = new List<ParserException>();
            ProgramSequence result = new ProgramSequenceActual(null, null);
            try
            {
                _scanner = ScannerFactory.Create(input, initialPosition);
                _currentToken = _scanner.Scan();
                try
                {
                    result = ParseProgramSequence();
                }
                catch (ParserException e)
                {
                    Errors.Add(e);
                }

            }
            catch(ScannerException e)
            {
                Errors.Add(new ParserException("Invalid input, discarded by scanner. See inner exception",e));
            }
            catch (Exception e)
            {
                Errors.Add(new ParserException("Unknown Exception. See inner exception", e));
                throw;
            }

            result.Errors = Errors;
            return result;
        }

        private void Accept(TokenKind expectedKind)
        {
            if (_currentToken.Kind != expectedKind)
            {
                Errors.Add(new IllegalTokenException(_currentToken,expectedKind));
                do
                {
                    AcceptIt();
                }
                while ((_currentToken.Kind != expectedKind) && Panic);
            }
            AcceptIt();
        }

        private void AcceptIt()
        {
            try
            {
                if (_currentToken.Kind == TokenKind.EOF)
                {
                    throw new IllegalTokenException(_currentToken, TokenKind.NESTEDWORDS, TokenKind.ENVIRONMENT_SEPARATOR, TokenKind.COMMAND_SEPARATOR, TokenKind.WORD, TokenKind.EXECUTE, TokenKind.MATH_SYMBOL);
                }
                _currentToken = _scanner.Scan();
            }
            catch (ScannerException e)
            {
                if (Panic)
                {
                    Errors.Add(new ParserException("Invalid input, discarded by scanner. See inner exception", e));
                }
                else
                {
                    throw;
                }
            }
        }

        private ProgramSequence ParseProgramSequence()
        {
            ASTs.Command comm = ParseCommand();
            ProgramSequenceFollow fol = ParseProgramSequenceFollow();

            if (_currentToken.Kind != TokenKind.EOF)
            { 
                Errors.Add(new IllegalTokenException(_currentToken,"Should have encountered end of file",TokenKind.EOF));
            }

            return new ProgramSequenceActual(comm, fol);
        }

        private ProgramSequenceFollow ParseProgramSequenceFollow()
        {
            
            ProgramSequenceFollow result = null;
            if (_currentToken.Kind == TokenKind.COMMAND_SEPARATOR)
            {
                Accept(TokenKind.COMMAND_SEPARATOR);
                ASTs.Command comm = ParseCommand();
                ProgramSequenceFollow fol = ParseProgramSequenceFollow();
                result = new ProgramSequenceFollowActual(comm, fol);

            }
            else if (_currentToken.Kind == TokenKind.COMMAND_PIPE)
            {
                Accept(TokenKind.COMMAND_PIPE);
                ASTs.Command comm = ParseCommand();
                comm.RelationToPrevious = CommandRelation.PIPED;
                ProgramSequenceFollow fol = ParseProgramSequenceFollow();
                result = new ProgramSequenceFollowActual(comm, fol);
            }
            else if (_currentToken.Kind == TokenKind.COMMAND_CONCATENATOR)
            {
                Accept(TokenKind.COMMAND_CONCATENATOR);
                ASTs.Command comm = ParseCommand();
                comm.RelationToPrevious = CommandRelation.CONCATENATED;
                ProgramSequenceFollow fol = ParseProgramSequenceFollow();
                result = new ProgramSequenceFollowActual(comm, fol);
            }
            else
            {
                result = new ProgramSequenceFollowEmpty(_currentToken.Position);
            }

            return result;
        }

        private ASTs.Command ParseCommand()
        {
            IList<Adaos.Shell.SyntaxAnalysis.ASTs.Environment> envs = new List<Adaos.Shell.SyntaxAnalysis.ASTs.Environment>();
            Word w = null;
            CommandName name = null;
            ArgumentSequence args = null;
            w = ParseWord();
            while (_currentToken.Kind == TokenKind.ENVIRONMENT_SEPARATOR)
            {
                envs.Add(new EnvironmentActual(w));
                AcceptIt();
                w = ParseWord();
            }

            name = new CommandNameActual(w);
            args = ParseArgumentSequence();

            return new ASTs.CommandWithEnvironment(envs, name, args);
        }

        private ArgumentSequence ParseArgumentSequence()
        {
            ArgumentSequence result = null;
            if (_currentToken.Kind == TokenKind.EOF)
            {
                result = new ArgumentSequenceEmpty(_currentToken.Position);
            }
            else if (CommandEnd())
            {
                result = new ArgumentSequenceEmpty(_currentToken.Position);
            }
            else
            {
                Argument arg = ParseArgument();
                ArgumentSequence argSeq = ParseArgumentSequence();
                result = new ArgumentSequenceActual(arg, argSeq);
            }

            return result;
        }

        private bool CommandEnd()
        {
            return 
                _currentToken.Kind == TokenKind.COMMAND_SEPARATOR || 
                _currentToken.Kind == TokenKind.COMMAND_PIPE || 
                _currentToken.Kind == TokenKind.COMMAND_CONCATENATOR;
        }

        private Argument ParseArgument()
        {
            Argument result = null;
            NestedWords nest = null;
            Word w = null;
            bool exec = _currentToken.Kind == TokenKind.EXECUTE;
            if (exec)
            {
                Accept(TokenKind.EXECUTE);
            }

            switch (_currentToken.Kind)
            { 
                case TokenKind.NESTEDWORDS:
                    nest = ParseNestedWord();
                    result = new ArgumentNested(exec, nest);
                    break;
                case TokenKind.WORD:
                    w = ParseWord();
                    result = new ArgumentWord(exec, w);
                    break;
                case TokenKind.MATH_SYMBOL:
                    w = ParseSymbol();
                    result = new ArgumentWord(exec,w);
                    break;
                default:
                    Errors.Add(new IllegalTokenException(_currentToken, TokenKind.WORD, TokenKind.NESTEDWORDS, TokenKind.MATH_SYMBOL));
                    while(
                        _currentToken.Kind != TokenKind.WORD &&
                        _currentToken.Kind != TokenKind.NESTEDWORDS &&
                        _currentToken.Kind != TokenKind.MATH_SYMBOL)
                    {
                        AcceptIt();
                    }
                    result = ParseArgument();
                    break;
            }
            return result;
        }

        private NestedWords ParseNestedWord()
        {
            NestedWords result = null;
            if (_currentToken.Kind != TokenKind.NESTEDWORDS)
            {
                if (!Panic) throw new IllegalTokenException(_currentToken, TokenKind.NESTEDWORDS);
                Errors.Add(new IllegalTokenException(_currentToken, TokenKind.NESTEDWORDS));
                do
                {
                    AcceptIt();
                }
                while ((_currentToken.Kind != TokenKind.NESTEDWORDS) && Panic);
            }

            result = new NestedWordsActual(_currentToken);
            AcceptIt();
            return result;
        }

        private Word ParseWord()
        {
            Word result = null;
            if(_currentToken.Kind != TokenKind.WORD)
            {
                if (!Panic) throw new IllegalTokenException(_currentToken, TokenKind.WORD);
                Errors.Add(new IllegalTokenException(_currentToken, TokenKind.WORD));
                do
                {
                    AcceptIt();
                }
                while ((_currentToken.Kind != TokenKind.WORD) && Panic);
            }
            result = new WordActual(_currentToken);
            AcceptIt();
            return result;
        }

        private Word ParseSymbol()
        {
            Word result = null;
            if (_currentToken.Kind != TokenKind.MATH_SYMBOL)
            {
                if (!Panic) throw new IllegalTokenException(_currentToken, TokenKind.MATH_SYMBOL);
                Errors.Add(new IllegalTokenException(_currentToken, TokenKind.MATH_SYMBOL));
                do
                {
                    AcceptIt();
                }
                while ((_currentToken.Kind != TokenKind.MATH_SYMBOL) && Panic);
            }
            result = new WordSymbol(_currentToken);
            AcceptIt();
            return result;
        }

        private CommandName ParseCommandName()
        {
            Word w = ParseWord();
            return new CommandNameActual(w);
        }
    }
}
