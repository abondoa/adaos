using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Adaos.Shell.Interface;
using Adaos.Shell.Terminal;
using Adaos.Common.Extenders;

namespace Adaos.Shell.Machine
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader input = null;
            StreamWriter output = null;
            StreamWriter log = null;
            for (int i = 0; i < args.Length; ++i)
            { 
                var arg = args[i];
                if (arg.Length < 2 || arg.First() != '-')
                {
                    throw new ArgumentException("Unknown argument: "+arg);
                }
                i++;
                switch (arg.Second())
                { 
                    case 'i':
                        input = new StreamReader(args[i]);
                        break;
                    case 'l':
                        log = new StreamWriter(new FileStream(args[i],FileMode.Append));
                        break;
                    case 'o':
                        output = new StreamWriter(args[i]);
                        break;
                }
            }
            ITerminal shell;
            if (log == null)
            {
                log = new StreamWriter(new FileStream("log.txt", FileMode.Append));
            }
            if (output == null)
            {
                output = new StreamWriter(Console.OpenStandardOutput());
            }
            if (input == null)
            {
                shell = new ConsoleTerminal(log);
            }
            else
            {
                shell = new Terminal.Terminal(input, output, log);
            }
            shell.Start();
        }
    }
}