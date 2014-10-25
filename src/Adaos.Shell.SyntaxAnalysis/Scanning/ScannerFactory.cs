using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Scanning
{
    class ScannerFactory : IScannerFactory<Tokens.Token>
    {
        public IScannerTable ScannerTable { get; set; }

        public ScannerFactory(IScannerTable scannerTable)
        {
            ScannerTable = scannerTable;
        }

        public IScanner<Tokens.Token> Create(string input, int extraPosition)
        {
            return new Scanner(input, ScannerTable, extraPosition);
        }

        public IScanner<Tokens.Token> Create(string input)
        {
            return Create(input, 0);
        }
    }
}
