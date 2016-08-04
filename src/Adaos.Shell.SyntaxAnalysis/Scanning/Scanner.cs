using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.SyntaxAnalysis.Tokens;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Scanning
{
    public class Scanner : IScanner<Token>
    {
        private string Pipe => _scannerTable.Pipe;
        private string Execute => _scannerTable.Execute;
        private string CommandSeparator => _scannerTable.CommandSeparator;
        private string CommandConcatenator => _scannerTable.CommandConcatenator;
        private string EnvironmentSeparator => _scannerTable.EnvironmentSeparator;
        private string Escaper => _scannerTable.Escaper;
        private string ArgumentSeparator => _scannerTable.ArgumentSeparator;
        private string ArgumentExecutableStarter => _scannerTable.ArgumentExecutableStarter;
        private string ArgumentExecutableStopper => _scannerTable.ArgumentExecutableStopper; 

        private string _workingString;
        private int _position;
        private StringBuilder _currentSpelling;
        private TokenKind _currentKind;
        private IScannerTable _scannerTable;
        private int _extraPostion;

        private char CurrentChar 
        {
            get 
            {
                if (_position < _workingString.Length)
                {
                    return _workingString[_position];
                }
                return (char)0;
            }
        }

        private string CurrentString(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("Non-positive length received: "+length.ToString());
            }
            if (length + _position > _workingString.Length)
            {
                return new string((char)0, length);
            }
            return _workingString.Substring(_position, length);
        }

        public Scanner(string input, IScannerTable scannerTable, int extraPosition = 0)
        {
            if (input == null)
            {
                throw new ScannerException("Input null");
            }
            if (input.Length == 0)
            {
                throw new ScannerException("Input length 0");
            }
            _workingString = input;
            _position = 0;
            _scannerTable = scannerTable;
            _extraPostion = extraPosition;

            _currentSpelling = new StringBuilder(); //Must be be done to allow strings to start with spaces
        }

        private void Take(char c)
        {
            if (CurrentChar == c)
            {
                TakeIt();
            }
            else
            {
                throw new IllegalCharacterException(c,CurrentChar);
            }
        }

        private void TakeIt(int num = 1)
        {
            _currentSpelling.Append(CurrentString(num));
            Skip(num);
        }

        private void Skip(int num = 1)
        {
            _position += num;
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private bool IsMathSymbol(char c)
        {
            switch(c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '=':
                    return true;
                default:
                    return false;
            }
        }

        private bool IsSeparator(char c)
        {
            switch (CurrentChar)
            {
                case ' ':
                case '\n':
                case '\r':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }

        public Token Scan()
        {
            while (IsSeparator(CurrentChar))
            {
                TakeIt();
            }

            _currentSpelling = new StringBuilder();
            int initPosition = _position+1;
            _currentKind = TokenKind.EOF;
            ScanToken();
            return new Token(initPosition + _extraPostion, _currentSpelling.ToString(), _currentKind);
        }

        private bool ScanSymbolTable()
        {
            if (CurrentString(EnvironmentSeparator.Length).Equals(EnvironmentSeparator))
            {
                _currentKind = TokenKind.ENVIRONMENT_SEPARATOR;
                TakeIt(EnvironmentSeparator.Length);
            }
            else if (CurrentString(CommandSeparator.Length).Equals(CommandSeparator))
            {
                _currentKind = TokenKind.EXECUTION_SEPARATOR;
                TakeIt(CommandSeparator.Length);
            }
            else if (CurrentString(CommandConcatenator.Length).Equals(CommandConcatenator))
            {
                _currentKind = TokenKind.EXECUTION_CONCATENATOR;
                TakeIt(CommandConcatenator.Length);
            }
            else if (CurrentString(Execute.Length).Equals(Execute))
            {
                _currentKind = TokenKind.EXECUTE;
                TakeIt(Execute.Length);
            }
            else if (CurrentString(Pipe.Length).Equals(Pipe))
            {
                _currentKind = TokenKind.EXECUTION_PIPE;
                TakeIt(Pipe.Length);
            }
            else if (CurrentString(ArgumentSeparator.Length).Equals(ArgumentSeparator))
            {
                _currentKind = TokenKind.ARGUMENT_SEPARATOR;
                TakeIt(ArgumentSeparator.Length);
            }
            else if (CurrentString(ArgumentSeparator.Length).Equals(ArgumentExecutableStarter))
            {
                _currentKind = TokenKind.ARGUMENT_EXECUTABLE_START;
                TakeIt(ArgumentExecutableStarter.Length);
            }
            else if (CurrentString(ArgumentSeparator.Length).Equals(ArgumentExecutableStopper))
            {
                _currentKind = TokenKind.ARGUMENT_EXECUTABLE_STOP;
                TakeIt(ArgumentExecutableStopper.Length);
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool CheckEscapedAndSkip()
        {
            if (CurrentString(Escaper.Length).Equals(Escaper))
            {
                Skip(Escaper.Length);
                if (CurrentChar == (char)0)
                {
                    throw new ScannerException(_position, "Trying to esacape EOF is not allowed.");
                }
                return true;
            }

            return false;
        }

        private void ScanToken()
        {
            if (ScanSymbolTable())
            {
            }
            else if (IsStartOfMathSymbol(CurrentChar))
            {
                ScanMathSymbol();
            }
            else if (IsStartOfWord(CurrentChar) || CheckEscapedAndSkip())
            {
                ScanWord();
            }
            else if (CurrentChar == '"' || CurrentChar == '\'')
            {
                ScanNestedWord();
            }
            else if (CurrentChar == (char)0)
            {
                _currentKind = TokenKind.EOF;
            }
            else
            {
                char unknown = CurrentChar;
                TakeIt();
                throw new UnknownCharacterException(unknown);
            }

            return;
        }

        private void ScanNestedWord()
        {
            _currentKind = TokenKind.NESTEDWORDS;
            char lastNester = CurrentChar;
            char nextNester = CurrentChar == '"' ? '\'' : '"';
            int nestingLevel = 1;
            TakeIt();
            while (nestingLevel >= 1)
            {
                if (CurrentChar == lastNester)
                {
                    nestingLevel--;
                    Swap(ref nextNester, ref lastNester);
                }
                else if (CurrentChar == nextNester)
                {
                    nestingLevel++;
                    Swap(ref nextNester, ref lastNester);
                }
                else if (CurrentChar == (char)0)
                {
                    throw new ScannerException("Premature EOF, current nesting level: " + nestingLevel);
                }
                CheckEscapedAndSkip();

                TakeIt();
            }
        }

        private void ScanMathSymbol()
        {
            _currentKind = TokenKind.MATH_SYMBOL;
            TakeIt();
            if (IsInsideOfMathSymbol(CurrentChar))
            {
                TakeIt();
            }
        }

        private void ScanWord()
        {
            _currentKind = TokenKind.WORD;
            TakeIt();
            while (IsInsideOfWord(CurrentChar) || CheckEscapedAndSkip())
            {
                TakeIt();
            }
        }

        private bool IsInsideOfMathSymbol(char c)
        {
            return IsMathSymbol(c);
        }

        private bool IsStartOfMathSymbol(char c)
        {
            return IsMathSymbol(c);
        }

        private bool IsStartOfWord(char c)
        {
            return IsDigit(c) || IsLetter(c) || c == '_' || c == '-';
        }
        
        private bool IsInsideOfWord(char c)
        {
            return IsDigit(c) || IsLetter(c) || c == '_' || c == '-';
        }

        private void Swap<T>(ref T one, ref T two)
        {
            T temp = one;
            one = two;
            two = temp;
        }
    }
}
