using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Adaos.Shell.Executer;
using Adaos.Shell.Executer.Extenders;
using Adaos.Shell.Interface;

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
            IShell shell;
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
                shell = new ConsoleShell(log);
            }
            else
            {
                shell = new Shell(input, output, log);
            }
            shell.Start();
        }
    }

    class DummyTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }
    }
}