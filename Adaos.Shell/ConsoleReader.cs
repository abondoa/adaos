using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Adaos.Shell
{
    public class ConsoleReader
    {
        Dictionary<ConsoleKey, Func<string,string>> _specialKeyMapper;
        private int _left;
        private int _top;
        private string _currentLine;

        public ConsoleReader(string lineInitializer = "> ")
        {
            LineInitializer = lineInitializer;
            _specialKeyMapper = new Dictionary<ConsoleKey, Func<string, string>>();
            _specialKeyMapper[ConsoleKey.Enter] = x => 
            {
                Console.WriteLine();
                throw new LoopBreakerException(); 
            };
            _specialKeyMapper[ConsoleKey.LeftArrow] = x =>
            {
                try
                {
                    CursorPosition--;
                }
                catch (ArgumentOutOfRangeException) { }
                return null;
            };
            _specialKeyMapper[ConsoleKey.RightArrow] = x =>
            {
                try
                {
                    CursorPosition++;
                }
                catch (ArgumentOutOfRangeException) { }
                return null;
            };
            _specialKeyMapper[ConsoleKey.Delete] = x =>
            {
                if (CursorPosition < x.Length)
                {
                    x = x.Remove(CursorPosition, 1);
                    _rewriteLine(x, CursorPosition);
                }
                return null;
            };
            _specialKeyMapper[ConsoleKey.Backspace] = x =>
            {
                if (CursorPosition > 0)
                {
                    CursorPosition--;
                    return _specialKeyMapper[ConsoleKey.Delete](x);
                }
                return null;
            };
        }

        private int CursorPosition
        {
            get
            {
                return (_top - Console.CursorTop) * Console.WindowWidth - _left - LineInitializer.Length + Console.CursorLeft;
            }
            set
            { 
                if(value > _currentLine.Length || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                Console.CursorLeft = value % Console.WindowWidth + LineInitializer.Length + _left;
                Console.CursorTop = (value + LineInitializer.Length + _left) / Console.WindowWidth + _top;
            }
        }

        public StreamWriter Output
        {
            get
            {
                return new StreamWriter(Console.OpenStandardOutput());
            }
            set
            {
                Console.SetOut(value);
            }
        }

        public StreamReader Input
        {
            get
            {
                return new StreamReader(Console.OpenStandardInput());
            }
            set
            {
                Console.SetIn(value);
            }
        }

        public string LineInitializer
        {
            get;
            set;
        }

        public void AddSpecialChar(ConsoleKey key, Func<string, string> func)
        {
            _specialKeyMapper[key] = func;
        }

        private void _rewriteLine(string currentLine = null,int curPos = -1)
        {
            if (currentLine == null)
            {
                currentLine = _currentLine;
            }

            Console.SetCursorPosition(_left, _top);
            Console.Write(("").PadRight(_currentLine.Length + LineInitializer.Length));
            Console.SetCursorPosition(_left, _top);
            Console.Write(LineInitializer + currentLine);
            _currentLine = currentLine;

            if (curPos >= 0)
            {
                CursorPosition = curPos;
            }
        }

        public string ReadLine(string currentLine = "")
        {
            _left = Console.CursorLeft;
            _top = Console.CursorTop;
            _currentLine = currentLine;

            _rewriteLine();
            ConsoleKeyInfo next;
            try
            {
                while (true)
                {
                    next = Console.ReadKey(true);
                    if (_specialKeyMapper.Keys.Contains(next.Key))
                    {
                        string temp = _specialKeyMapper[next.Key](_currentLine);
                        if (temp != null)
                        {
                            _rewriteLine(temp);
                        }
                    }
                    else
                    {
                        char ch = next.KeyChar;
                        if (ch > 0)
                        {
                            _currentLine = _currentLine.Insert(CursorPosition, ch.ToString());
                            _rewriteLine(null, CursorPosition+1);
                        }
                    }
                }
            }
            catch (LoopBreakerException)
            { }
            return _currentLine;
        }
    }
}
