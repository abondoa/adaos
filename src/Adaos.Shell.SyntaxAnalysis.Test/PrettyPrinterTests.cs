using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Shell.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.SyntaxAnalysis.Parsing;

namespace Adaos.Shell.SyntaxAnalysis.Tests
{
    [TestClass()]
    public class PrettyPrinterTests
    {
        [TestMethod()]
        public void Visit_CommandWithEnvironmentAndArgument()
        {
            Parser parser = new Parser();
            var input = "env.comm arg";
            var executionSequence = parser.Parse(input,0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_WithMultipleExecutionsWithSeparation()
        {
            Parser parser = new Parser();
            var input = "env1.comm1 arg1; env2.comm2 arg2";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_WithMultipleExecutionsWithConcatenator()
        {
            Parser parser = new Parser();
            var input = "env1.comm1 arg1, env2.comm2 arg2";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_WithMultipleExecutionsWithPipe()
        {
            Parser parser = new Parser();
            var input = "env1.comm1 arg1 | env2.comm2 arg2";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_CommandName()
        {
            Parser parser = new Parser();
            var input = "comm";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_MultipleArguments()
        {
            Parser parser = new Parser();
            var input = "comm arg1 arg2";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_MultipleEnironments()
        {
            Parser parser = new Parser();
            var input = "env1.env2.comm";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_ExecutableArgument()
        {
            Parser parser = new Parser();
            var input = "comm (i)";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_ExecuteArgument()
        {
            Parser parser = new Parser();
            var input = "comm $i";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }

        [TestMethod()]
        public void Visit_NamedArgument()
        {
            Parser parser = new Parser();
            var input = "comm name:arg";
            var executionSequence = parser.Parse(input, 0);
            PrettyPrinter printer = new PrettyPrinter(parser.ScannerTable);

            var result = executionSequence.Visit(printer, null);

            Assert.AreEqual(input, result);
        }
    }
}