﻿using Adaos.Shell.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentExecutable : Argument
    {
        public ExecutionSequence ExecutionSequence { get; }

        private PrettyPrinter _printer = null;
        private PrettyPrinter Printer
        {
            get
            {
                if (_printer == null) _printer = new PrettyPrinter(_scannerTable);
                return _printer;
            }
        }
        private IScannerTable _scannerTable;

        public ArgumentExecutable(ExecutionSequence executionSequence, int position, bool execute, IScannerTable scannerTable, Word wordName = null) : base(position, execute, wordName)
        {
            ExecutionSequence = executionSequence;
            _scannerTable = scannerTable;
        }


        public override string Value => Visit(Printer, null).ToString();

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
