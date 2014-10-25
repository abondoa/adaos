using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Adaos.Shell.Executer;
using Adaos.Shell.Interface;

namespace Adaos.Shell
{
    public class ConsoleShell : IShell
    {
        private List<string> _commands;
        private int _commandPointer;
        public ConsoleShell(StreamWriter log = null)
        {
            if (log == null) log = new StreamWriter(Stream.Null);
            VirtualMachine = new VirtualMachine(new StreamWriter(Console.OpenStandardOutput()), log);

            _commands = new List<string>();
            Reader = new ConsoleReader();
            Reader.AddSpecialChar(ConsoleKey.Tab, x => 
            {
                return VirtualMachine.SuggestCommand(x);
            });
            Reader.AddSpecialChar(ConsoleKey.UpArrow, x =>
            {
                _commandPointer--;
                if (_commandPointer < 0)
                {
                    _commandPointer = _commands.Count - 1;
                }
                return _commands[_commandPointer];
            });
            Reader.AddSpecialChar(ConsoleKey.DownArrow, x =>
            {
                _commandPointer++;
                if (_commandPointer >= _commands.Count)
                {
                    _commandPointer = _commands.Count;
                    return "";
                }
                return _commands[_commandPointer];
            });
        }

        public ConsoleShell(IVirtualMachine virtualMachine)
        {
            if (virtualMachine == null) throw new ArgumentNullException("virtualMachine");
            VirtualMachine = virtualMachine;
            _commands = new List<string>();
        }

        private ConsoleReader Reader
        {
            get;
            set;
        }

        public StreamReader Input
        {
            get
            {
                return Reader.Input;
            }
            set
            {
                Reader.Input = value;
            }
        }

        public StreamWriter Output
        {
            get
            {
                return VirtualMachine.Output;
            }
            set
            {
                VirtualMachine.Output = value;
            }
        }

        public StreamWriter Log
        {
            get
            {
                return VirtualMachine.Log;
            }
            set
            {
                VirtualMachine.Log = value;
            }
        }


        public IVirtualMachine VirtualMachine
        {
            get;
            set;
        }

        public void Start()
        {
            string cmd;
            bool firstException = true;
            Running = true;
            Output.AutoFlush = true;
            VirtualMachine.HandleError = x =>
            {
                if (x.Position >= 0)
                {
                    if (firstException)
                    {
                        string toWrite = "-";
                        for (int i = 0; i < x.Position; ++i)
                        {
                            toWrite += '-';
                        }
                        Output.WriteLine(toWrite + '^');
                    }
                    else
                    {
                        Output.WriteLine("Error occured at position: " + x.Position);
                    }
                }
                Output.WriteLine(x.Message);
                firstException = false;
            };
            try
            {
                Running = true;
                while ((cmd = Reader.ReadLine()) != null && Running)
                {
                    if (string.Copy(cmd).Trim() != string.Empty)
                    {
                        firstException = true;
                        VirtualMachine.Execute(cmd);
                        _commands.Add(cmd);
                        _commandPointer = _commands.Count;
                    }
                }
            }
            catch (ExitShellException)
            { }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            Running = false;
            Log.Flush();
        }

        public bool Running
        {
            get;
            private set;
        }
    }
}
