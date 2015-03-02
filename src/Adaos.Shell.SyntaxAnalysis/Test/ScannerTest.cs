using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface.SyntaxAnalysis;
using NUnit.Framework;
using Adaos.Shell.SyntaxAnalysis.Scanning;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Test
{
    [TestFixture]
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
        private string at = "@";
        private string escaped = "\\'";
        private string escapeNothing = "\\";
        private string commandConcatenator = ",";
        private string neverEndingApostropheEmpty = "'";
        private string neverEndingApostropheContent = "'abc";
        private string neverEndingQuoteEmpty = "\"";
        private string neverEndingQuoteContent = "\"abc";
        private string echoSlash = "echo /n";
        private readonly IScannerTable stdScannerTable = new ScannerTable();
        private IScannerTable scannerTable = new ScannerTable();
        private string escapedInWord = "let\\'sEscape\\!\\@TheMoment";
        private string escapedInNestedWord = "\"user create AleTheMale\\!\\! @'random number \"0\" \"9999\" width 4'\"";

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            scanner = null;
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void ScanWordSimple()
        {
            _test(simpleWord, Tokens.TokenKind.WORD);
        }

        [Test]
        public void ScanWordComplex()
        {
            _test(complexWord, Tokens.TokenKind.WORD);
        }

        [Test]
        public void ScanSemicolon()
        {
            _test(semicolon, Tokens.TokenKind.COMMAND_SEPARATOR);
        }

        [Test]
        public void ScanAt()
        {
            _test(at, Tokens.TokenKind.EXECUTE);
        }

        [Test]
        public void ScanConcatenator()
        {
            _test(commandConcatenator, Tokens.TokenKind.COMMAND_CONCATENATOR);
        }

        [Test]
        public void ScanEnvironment()
        {
            _test(environement, Tokens.TokenKind.ENVIRONMENT_SEPARATOR);
        }

        [Test]
        public void ScanNestedWordSimple()
        {
            _test(simpleNestedWord, Tokens.TokenKind.NESTEDWORDS);
        }

        [Test]
        public void ScanNestedWordComplex()
        {
            _test(complexNestedWord, Tokens.TokenKind.NESTEDWORDS);
        }

        [Test]
        public void ScanEOF()
        {
            _test(";",Tokens.TokenKind.EOF,"",2,new int[]{4,4});
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void SlashTest()
        {
            scanner = new Scanner(echoSlash, scannerTable);
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
                Assert.AreEqual(echoSlash.Length, result.Position);
            }
        }

        [Test]
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

        [Test]
        public void NewScanTablePipe()
        {
            scannerTable.Pipe = "||";
            _test("||", Tokens.TokenKind.COMMAND_PIPE);

            //Clean up
            scannerTable = stdScannerTable;
        }

        [Test]
        public void NewScanTableLongWeirdExecute()
        {
            scannerTable.Execute = "!!EXECUTE??";
            _test("!!EXECUTE??", Tokens.TokenKind.EXECUTE);

            //Clean up
            scannerTable = stdScannerTable;
        }

        [Test]
        public void EscapedCharacter()
        {
            _test(escaped, Tokens.TokenKind.WORD, escaped.Replace("\\", ""));
        }

        [Test]
        public void EscapedCharacterInTheMiddleOfWord()
        {
            _test(escapedInWord, Tokens.TokenKind.WORD, escapedInWord.Replace("\\",""));
        }

        [Test]
        public void EscapedCharacterInTheMiddleOfNestedWord()
        {
            _test(escapedInNestedWord, Tokens.TokenKind.NESTEDWORDS, escapedInNestedWord.Replace("\\", ""));
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
            scanner = new Scanner(input + " " + input, scannerTable);
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
