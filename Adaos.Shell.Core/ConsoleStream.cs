using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Core
{
    public class ConsoleStream : StreamReader 
    {
        public ConsoleStream() : base(new MemoryStream()) 
        {
        }

        public override int Read() 
        {
            var key = Console.ReadKey(true);
            return (int) key.KeyChar;
        }

        public override Stream BaseStream
        {
            get
            {
                return Console.OpenStandardOutput();
            }
        }

        public override string ReadLine()
        {
            StringBuilder strBuilder = new StringBuilder();
            int next;
            while ((next = Read()) != '\n')
            {
                strBuilder.Append(next);
            }
            return strBuilder.ToString();
        }
    }
}
