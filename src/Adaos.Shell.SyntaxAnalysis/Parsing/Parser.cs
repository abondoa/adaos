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
            ExecutionSequence result = new ExecutionSequenceActual(null, null);
            try
            {
                _scanner = ScannerFactory.Create(input, initialPosition);
                _currentToken = _scanner.Scan();
                try
                {
                    result = ParseExecutionSequence();
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
                    throw new IllegalTokenException(_currentToken, TokenKind.NESTEDWORDS, TokenKind.ENVIRONMENT_SEPARATOR, TokenKind.EXECUTION_SEPARATOR, TokenKind.WORD, TokenKind.EXECUTE, TokenKind.MATH_SYMBOL);
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

        private ExecutionSequence ParseExecutionSequence(bool innerParsing = false)
        {
            ASTs.Execution comm = ParseCommand();
            ExecutionSequenceFollow fol = ParseProgramSequenceFollow();

            if (_currentToken.Kind != TokenKind.EOF)
            {
                if (innerParsing && _currentToken.Kind == TokenKind.ARGUMENT_EXECUTABLE_STOP)
                { } // OK
                else
                {
                    Errors.Add(new IllegalTokenException(_currentToken, "Should have encountered end of file", TokenKind.EOF));
                }
            }

            return new ExecutionSequenceActual(comm, fol);
        }

        private ExecutionSequenceFollow ParseProgramSequenceFollow()
        {
            
            ExecutionSequenceFollow result = null;
            if (_currentToken.Kind == TokenKind.EXECUTION_SEPARATOR)
            {
                Accept(TokenKind.EXECUTION_SEPARATOR);
                ASTs.Execution comm = ParseCommand();
                ExecutionSequenceFollow fol = ParseProgramSequenceFollow();
                result = new ExecutionSequenceFollowActual(comm, fol);

            }
            else if (_currentToken.Kind == TokenKind.EXECUTION_PIPE)
            {
                Accept(TokenKind.EXECUTION_PIPE);
                ASTs.Execution comm = ParseCommand();
                comm.RelationToPrevious = CommandRelation.Piped;
                ExecutionSequenceFollow fol = ParseProgramSequenceFollow();
                result = new ExecutionSequenceFollowActual(comm, fol);
            }
            else if (_currentToken.Kind == TokenKind.EXECUTION_CONCATENATOR)
            {
                Accept(TokenKind.EXECUTION_CONCATENATOR);
                ASTs.Execution comm = ParseCommand();
                comm.RelationToPrevious = CommandRelation.Concatenated;
                ExecutionSequenceFollow fol = ParseProgramSequenceFollow();
                result = new ExecutionSequenceFollowActual(comm, fol);
            }
            else
            {
                result = new ExecutionSequenceFollowEmpty(_currentToken.Position);
            }

            return result;
        }

        private ASTs.Execution ParseCommand()
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
            if (CommandEnd())
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
                _currentToken.Kind == TokenKind.EOF || 
                _currentToken.Kind == TokenKind.ARGUMENT_EXECUTABLE_STOP || 
                _currentToken.Kind == TokenKind.EXECUTION_SEPARATOR ||
                _currentToken.Kind == TokenKind.EXECUTION_PIPE || 
                _currentToken.Kind == TokenKind.EXECUTION_CONCATENATOR;
        }

        private Argument ParseArgument()
        {
            if (_currentToken.Kind == TokenKind.EXECUTE)
            {
                return ParseValue(null);
            }

            switch (_currentToken.Kind)
            { 
                case TokenKind.WORD:
                    var word = ParseWord();
                    if (_currentToken.Kind == TokenKind.ARGUMENT_SEPARATOR)
                    {
                        AcceptIt();
                        return ParseValue(word);
                    }
                    else
                    {
                        return new ArgumentWord(false, word);
                    }
                case TokenKind.NESTEDWORDS:
                case TokenKind.ARGUMENT_EXECUTABLE_START:
                    return ParseValue(null);
                default:
                    Errors.Add(new IllegalTokenException(_currentToken, TokenKind.WORD, TokenKind.NESTEDWORDS, TokenKind.ARGUMENT_EXECUTABLE_START));
                    while(
                        _currentToken.Kind != TokenKind.WORD &&
                        _currentToken.Kind != TokenKind.NESTEDWORDS &&
                        _currentToken.Kind != TokenKind.ARGUMENT_EXECUTABLE_START)
                    {
                        AcceptIt();
                    }
                    return ParseArgument();
            }
        }

        private Argument ParseValue(Word wordName)
        {
            bool exec = _currentToken.Kind == TokenKind.EXECUTE;
            if (exec)
            {
                AcceptIt();
            }

            switch (_currentToken.Kind)
            {
                case TokenKind.NESTEDWORDS:
                    var nest = ParseNestedWord();
                    return new ArgumentNested(exec, nest, wordName);
                case TokenKind.WORD:
                    var word = ParseWord();
                    return new ArgumentWord(exec, word, wordName);
                case TokenKind.ARGUMENT_EXECUTABLE_START:
                    int position = _currentToken.Position;
                    AcceptIt();
                    var executionSequence = ParseExecutionSequence(true);
                    var result = new ArgumentExecutable(executionSequence, position, exec, wordName);
                    Accept(TokenKind.ARGUMENT_EXECUTABLE_STOP);
                    return result;
                default:
                    Errors.Add(new IllegalTokenException(_currentToken, TokenKind.WORD, TokenKind.NESTEDWORDS, TokenKind.ARGUMENT_EXECUTABLE_START));
                    while (
                        _currentToken.Kind != TokenKind.WORD &&
                        _currentToken.Kind != TokenKind.NESTEDWORDS &&
                        _currentToken.Kind != TokenKind.ARGUMENT_EXECUTABLE_START)
                    {
                        AcceptIt();
                    }
                    return ParseArgument();
            }
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
                if (!Panic || _currentToken.Kind == TokenKind.EOF) throw new IllegalTokenException(_currentToken, TokenKind.WORD);
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
