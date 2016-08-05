using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.SyntaxAnalysis.Scanning;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Shell.SyntaxAnalysis.Tokens;

namespace Adaos.Shell.SyntaxAnalysis.Test
{
    [TestClass]
    public class ScannerTest
    {
        private Scanner scanner;
        private string workingInput = "user create Ale '123456 654'; user delete \"Ale\"";
        private string nullInput = null;
        private string emptyInput = "";
        private string simpleWord = "a";
        private string complexWord = "_word-1_2_3";
        private string simpleNestedWord = "'a'";
        private string complexNestedWord = "\"user create Ale @'random number \"0\" \"9999\" width 4'\"";
        private string semicolon = ";";
        private string environement = ".";
        private string executeSymbol = "$";
        private string escaped = "\\'";
        private string escapeNothing = "\\";
        private string commandConcatenator = ",";
        private string neverEndingApostropheEmpty = "'";
        private string neverEndingApostropheContent = "'abc";
        private string neverEndingQuoteEmpty = "\"";
        private string neverEndingQuoteContent = "\"abc";
        private string echoTilde = "echo ~n";
        private readonly IScannerTable stdScannerTable = new ScannerTable();
        private IScannerTable scannerTable = new ScannerTable();
        private string escapedInWord = "let\\'sEscape\\!\\$TheMoment";
        private string escapedInNestedWord = "\"user create AleTheMale\\!\\! @'random number \"0\" \"9999\" width 4'\"";
        private string arguementExecutableStart = "(";
        private string arguementExecutableStop = ")";

        [TestInitialize]
        public void SetUp()
        {
        }

        [TestCleanup]
        public void TearDown()
        {
            scanner = null;
        }

        [TestMethod]
        public void ConstructorNullInput()
        {
            try
            {
                scanner = new Scanner(nullInput, scannerTable);
                Assert.Fail();
            }
            catch (ScannerException)
            {
                Assert.IsNull(scanner);
            }
        }

        [TestMethod]
        public void ConstructorEmptyInput()
        {
            try
            {
                scanner = new Scanner(emptyInput, scannerTable);
                Assert.Fail();
            }
            catch (ScannerException)
            {
                Assert.IsNull(scanner);
            }
        }

        [TestMethod]
        public void ConstructorWorkingInput()
        {
            try
            {
                scanner = new Scanner(workingInput, scannerTable);
                Assert.IsNotNull(scanner);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ScanWordSimple()
        {
            _test(simpleWord, Tokens.TokenKind.WORD);
        }

        [TestMethod]
        public void ScanWordComplex()
        {
            _test(complexWord, Tokens.TokenKind.WORD);
        }

        [TestMethod]
        public void ScanSemicolon()
        {
            _test(semicolon, Tokens.TokenKind.EXECUTION_SEPARATOR);
        }

        [TestMethod]
        public void ScanExecutionSymbol()
        {
            _test(executeSymbol, Tokens.TokenKind.EXECUTE);
        }

        [TestMethod]
        public void ScanArgumentExecutableStart()
        {
            _test(arguementExecutableStart, Tokens.TokenKind.ARGUMENT_EXECUTABLE_START);
        }

        [TestMethod]
        public void ScanArgumentExecutableStop()
        {
            _test(arguementExecutableStop, Tokens.TokenKind.ARGUMENT_EXECUTABLE_STOP);
        }

        [TestMethod]
        public void ScanConcatenator()
        {
            _test(commandConcatenator, Tokens.TokenKind.EXECUTION_CONCATENATOR);
        }

        [TestMethod]
        public void ScanEnvironment()
        {
            _test(environement, Tokens.TokenKind.ENVIRONMENT_SEPARATOR);
        }

        [TestMethod]
        public void ScanNestedWordSimple()
        {
            _test(simpleNestedWord, Tokens.TokenKind.NESTEDWORDS);
        }

        [TestMethod]
        public void ScanNestedWordComplex()
        {
            _test(complexNestedWord, Tokens.TokenKind.NESTEDWORDS);
        }

        [TestMethod]
        public void ScanEOF()
        {
            _test(";",Tokens.TokenKind.EOF,"",2,new int[]{4,4});
        }

        [TestMethod]
        public void NeverEndingQuoteEmpty()
        {
            scanner = new Scanner(neverEndingQuoteEmpty, scannerTable);
            try
            {
                scanner.Scan();
                Assert.Fail();
            }
            catch (ScannerException)
            {
                //Verify that the next token is EOF
                Tokens.Token result = scanner.Scan();
                Assert.AreEqual(Tokens.TokenKind.EOF, result.Kind);
                Assert.AreEqual("", result.Spelling);
                Assert.AreEqual(1 + neverEndingQuoteEmpty.Length, result.Position);
            }
        }

        [TestMethod]
        public void NeverEndingQuoteContent()
        {
            scanner = new Scanner(neverEndingQuoteContent, scannerTable);
            try
            {
                scanner.Scan();
                Assert.Fail();
            }
            catch (ScannerException)
            {
                //Verify that the next token is EOF
                Tokens.Token result = scanner.Scan();
                Assert.AreEqual(Tokens.TokenKind.EOF, result.Kind);
                Assert.AreEqual("", result.Spelling);
                Assert.AreEqual(1 + neverEndingQuoteContent.Length, result.Position);
            }
        }

        [TestMethod]
        public void NeverEndingApostropheEmpty()
        {
            scanner = new Scanner(neverEndingApostropheEmpty, scannerTable);
            try
            {
                scanner.Scan();
                Assert.Fail();
            }
            catch (ScannerException)
            {
                //Verify that the next token is EOF
                Tokens.Token result = scanner.Scan();
                Assert.AreEqual(Tokens.TokenKind.EOF, result.Kind);
                Assert.AreEqual("", result.Spelling);
                Assert.AreEqual(1 + neverEndingApostropheEmpty.Length, result.Position);
            }
        }

        [TestMethod]
        public void NeverEndingApostropheContent()
        {
            scanner = new Scanner(neverEndingApostropheContent, scannerTable);
            try
            {
                scanner.Scan();
                Assert.Fail();
            }
            catch (ScannerException)
            {
                //Verify that the next token is EOF
                Tokens.Token result = scanner.Scan();
                Assert.AreEqual(Tokens.TokenKind.EOF, result.Kind);
                Assert.AreEqual("", result.Spelling);
                Assert.AreEqual(1 + neverEndingApostropheContent.Length, result.Position);
            }
        }

        [TestMethod]
        public void TildeTest()
        {
            scanner = new Scanner(echoTilde, scannerTable);
            try
            {
                scanner.Scan();
                scanner.Scan();
                Assert.Fail();
            }
            catch (ScannerException)
            {
                //Verify that the next token is WORD (n)
                Tokens.Token result = scanner.Scan();
                Assert.AreEqual(Tokens.TokenKind.WORD, result.Kind);
                Assert.AreEqual("n", result.Spelling);
                Assert.AreEqual(echoTilde.Length, result.Position);
            }
        }

        [TestMethod]
        public void EscapeNothing()
        {
            scanner = new Scanner(escapeNothing, scannerTable);
            try
            {
                scanner.Scan();
                Assert.Fail("Scanner exception should have been thrown");
            }
            catch (ScannerException)
            {
                //Verify that the next token is EOF
                Tokens.Token result = scanner.Scan();
                Assert.AreEqual(Tokens.TokenKind.EOF, result.Kind);
                Assert.AreEqual(escapeNothing.Length + 1, result.Position);
            }
        }

        [TestMethod]
        public void NewScanTablePipe()
        {
            scannerTable.Pipe = "||";
            _test("||", Tokens.TokenKind.EXECUTION_PIPE);

            //Clean up
            scannerTable = stdScannerTable;
        }

        [TestMethod]
        public void NewScanTableLongWeirdExecute()
        {
            scannerTable.Execute = "!!EXECUTE??";
            _test("!!EXECUTE??", Tokens.TokenKind.EXECUTE);

            //Clean up
            scannerTable = stdScannerTable;
        }

        [TestMethod]
        public void EscapedCharacter()
        {
            _test(escaped, Tokens.TokenKind.WORD, escaped.Replace("\\", ""));
        }

        [TestMethod]
        public void EscapedCharacterInTheMiddleOfWord()
        {
            _test(escapedInWord, Tokens.TokenKind.WORD, escapedInWord.Replace("\\",""));
        }

        [TestMethod]
        public void EscapedCharacterInTheMiddleOfNestedWord()
        {
            _test(escapedInNestedWord, Tokens.TokenKind.NESTEDWORDS, escapedInNestedWord.Replace("\\", ""));
        }

        [TestMethod]
        public void Scan_PlusMathSymbol()
        {
            _test("+", Tokens.TokenKind.MATH_SYMBOL);
        }

        [TestMethod]
        public void Scan_PlusPlusMathSymbol()
        {
            _test("++", Tokens.TokenKind.MATH_SYMBOL);
        }

        [TestMethod]
        public void Scan_MinusMathSymbol()
        {
            _test("-", Tokens.TokenKind.MATH_SYMBOL);
        }

        [TestMethod]
        public void Scan_MinusMinusMathSymbol()
        {
            _test("-", Tokens.TokenKind.MATH_SYMBOL);
        }

        [TestMethod]
        public void Scan_MultiplyMathSymbol()
        {
            _test("*", Tokens.TokenKind.MATH_SYMBOL);
        }

        [TestMethod]
        public void Scan_DivideMathSymbol()
        {
            _test("/", Tokens.TokenKind.MATH_SYMBOL);
        }

        [TestMethod]
        public void Scan_PlusMinusMathSymbols()
        {
            scanner = new Scanner("+-", scannerTable);
            var token1 = scanner.Scan();
            var token2 = scanner.Scan();
            var token3 = scanner.Scan();
            Assert.AreEqual(TokenKind.MATH_SYMBOL, token1.Kind);
            Assert.AreEqual("+", token1.Spelling);
            Assert.AreEqual(TokenKind.MATH_SYMBOL, token2.Kind);
            Assert.AreEqual("-", token2.Spelling);
            Assert.AreEqual(TokenKind.EOF, token3.Kind);
        }

        private void _test(string input, Tokens.TokenKind expectedKind)
        {
            _test(input, expectedKind, input);
        }

        private void _test(string input, Tokens.TokenKind expectedKind, string expectedSpelling)
        {
            _test(input, expectedKind, expectedSpelling, 0);
        }

        private void _test(string input, Tokens.TokenKind expectedKind, string expectedSpelling, int initialScans)
        {
            _test(input, expectedKind, expectedSpelling, initialScans, new int[] { 1, 2 + input.Length });
        }

        private void _test(string input, Tokens.TokenKind expectedKind, int initialScans)
        {
            _test(input, expectedKind, initialScans, new int[] { 1, 2 + input.Length });
        }

        private void _test(string input, Tokens.TokenKind expectedKind, int initialScans, int[] expectedPositions)
        {
            _test(input, expectedKind, input, initialScans, expectedPositions);
        }

        private void _test(string input, Tokens.TokenKind expectedKind, int[] expectedPositions)
        {
            _test(input, expectedKind, input, 0, expectedPositions);
        }

        private void _test(string input, Tokens.TokenKind expectedKind, string expectedSpelling, int[] expectedPositions)
        {
            _test(input, expectedKind, expectedSpelling, 0, expectedPositions);
        }

        private void _test(string input, Tokens.TokenKind expectedKind, string expectedSpelling, int initialScans, int[] expectedPositions)
        {
            if (input == null || input.Length == 0 || initialScans < 0 || expectedPositions.Length < 2)
            {
                Assert.Inconclusive("_test method received bad inputs");
            }
            scanner = new Scanner(input + " " + input, scannerTable, 0);
            for (int i = 0; i < initialScans; ++i)
            {
                scanner.Scan();
            }
            Tokens.Token result = scanner.Scan();
            Assert.AreEqual(expectedKind, result.Kind,"Failed on kind");
            Assert.AreEqual(expectedSpelling, result.Spelling,"Failed on spelling");
            Assert.AreEqual(expectedPositions[0], result.Position,"Failed on first position");
            result = scanner.Scan();
            Assert.AreEqual(expectedPositions[1], result.Position,"Failed on second position");
        }
    }
}
