using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Shell.Library.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Adaos.Shell.Interface;
using Adaos.Shell.Core;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Library.Standard.Tests
{
    [TestClass()]
    public class ControlStructureEnvironmentTests
    {
        private Mock<IVirtualMachine> _vmMock;
        private ControlStructureEnvironment _controlStructureEnvironment;

        [TestInitialize]
        public void Setup()
        {
            _vmMock = new Mock<IVirtualMachine>();
            _controlStructureEnvironment = new ControlStructureEnvironment(_vmMock.Object);
        }

        #region Helpers
        private IArgument Else => new DummyArgument("else");
        private IArgument If => new DummyArgument("if");
        private IArgument True => DummyArgument.True;
        private IArgument False => DummyArgument.False;
        private IArgument Arg1 => new DummyArgument("1");
        private IArgument Arg2 => new DummyArgument("2");
        private IArgument Arg3 => new DummyArgument("3");
        private IArgument Arg4 => new DummyArgument("4");
        private IEnumerable<IArgument> EmptyArgs => new IArgument[0];
        private IEnumerable<IArgument> Args(params IArgument[] args) => args;

        private void AssertContent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());
            foreach (var pair in expected.Zip(actual, (x, y) => Tuple.Create(x, y)))
            {
                Assert.AreEqual(pair.Item1, pair.Item2);
            }
        }
        #endregion // Helpers

        [TestMethod()]
        public void If_TrueNoElse_ThenValueReturned()
        {
            var res = _controlStructureEnvironment.If(new[] {
                True,
                Arg1
            });

            AssertContent(Args(Arg1), res);
        }

        [TestMethod()]
        public void If_TrueNoElse_ThenCommandCalled()
        {
            var res = _controlStructureEnvironment.If(new[] {
                True,
                Arg1
            });

            AssertContent(Args(Arg1), res);
        }

        [TestMethod()]
        public void If_FalseNoElse_NothingReturned()
        {
            var res = _controlStructureEnvironment.If(new[] {
                False,
                Arg1
            });

            AssertContent(EmptyArgs, res);
        }

        [TestMethod()]
        public void If_TrueWithElse_ThenValueReturned()
        {
            var res = _controlStructureEnvironment.If(new[] {
                True,
                Arg1,
                Else,
                Arg2,
            });

            AssertContent(Args(Arg1), res);
        }

        [TestMethod()]
        public void If_FalseWithElse_ElseValueReturned()
        {
            var res = _controlStructureEnvironment.If(new[] {
                False,
                Arg1,
                Else,
                Arg2,
            });

            AssertContent(Args(Arg2), res);
        }

        [TestMethod()]
        public void If_ElseWithNoValue_ExceptionThrown()
        {
            try
            {
                _controlStructureEnvironment.If(new[] {
                    True,
                    Arg1,
                    Else
                });
                Assert.Fail($"{nameof(SemanticException)} should have been thrown");
            }
            catch(SemanticException e)
            {
                Assert.IsTrue(e.Message.Contains("else"));
            }
        }

        [TestMethod()]
        public void If_ThirdArgumentNotElse_ExceptionThrown()
        {
            try
            {
                _controlStructureEnvironment.If(new[] {
                    True,
                    Arg1,
                    new DummyArgument("derp")
                });
                Assert.Fail($"{nameof(SemanticException)} should have been thrown");
            }
            catch (SemanticException e)
            {
                Assert.IsTrue(e.Message.Contains("else"));
            }
        }

        [TestMethod()]
        public void If_FirstArgumentNotBoolean_ExceptionThrown()
        {
            try
            {
                _controlStructureEnvironment.If(new[] {
                        new DummyArgument("derp"),
                        Arg1
                    });
                Assert.Fail($"{nameof(SemanticException)} should have been thrown");
            }
            catch (SemanticException e)
            {
                Assert.IsTrue(e.Message.Contains("bool"));
            }
        }

        [TestMethod()]
        public void If_TrueExecutionNoElse_ThenValueExecuted()
        {
            var argExecutableMock = new Mock<IArgumentExecutable>();
            var executionSequenceMock = new Mock<IExecutionSequence>();
            argExecutableMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceMock.Object);
            var expectedOutput = new IArgument[] { };
            _vmMock.Setup(x => x.Execute(executionSequenceMock.Object)).Returns(expectedOutput).Verifiable();

            var res = _controlStructureEnvironment.If(new IArgument[] {
                True,
                argExecutableMock.Object
            });

            _vmMock.Verify();
            AssertContent(expectedOutput, res);
        }

        [TestMethod()]
        public void If_TrueExecutionNoElse_ThenValueExecutedWithValues()
        {
            var argExecutableMock = new Mock<IArgumentExecutable>();
            var executionSequenceMock = new Mock<IExecutionSequence>();
            argExecutableMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceMock.Object);
            var expectedOutput = new IArgument[] { Arg1, Arg2 };
            _vmMock.Setup(x => x.Execute(executionSequenceMock.Object)).Returns(expectedOutput.ToList()).Verifiable();

            var res = _controlStructureEnvironment.If(new IArgument[] {
                True,
                argExecutableMock.Object
            });

            _vmMock.Verify();
            AssertContent(expectedOutput, res);
        }

        [TestMethod()]
        public void If_FalseExecutionNoElse_ThenValueNotExecuted()
        {
            var argExecutableMock = new Mock<IArgumentExecutable>();
            var executionSequenceMock = new Mock<IExecutionSequence>();
            argExecutableMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceMock.Object);
            var output = new IArgument[] { Arg1, Arg2 };
            _vmMock.Setup(x => x.Execute(It.IsAny<IExecutionSequence>())).Throws(new Exception("Execute should not be called"));

            var res = _controlStructureEnvironment.If(new IArgument[] {
                False,
                argExecutableMock.Object
            });

            AssertContent(EmptyArgs, res);
        }

        [TestMethod()]
        public void If_FalseExecutionElseExecution_ElseValueExecuted()
        {
            var argExecutableMock = new Mock<IArgumentExecutable>();
            var executionSequenceMock = new Mock<IExecutionSequence>();
            argExecutableMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceMock.Object);
            var thenOutput = new IArgument[] { Arg1, Arg2 };
            var argExecutableElseMock = new Mock<IArgumentExecutable>();
            var executionSequenceElseMock = new Mock<IExecutionSequence>();
            argExecutableElseMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceElseMock.Object);
            var expectedOutput = new IArgument[] { };
            _vmMock.Setup(x => x.Execute(executionSequenceElseMock.Object)).Returns(expectedOutput).Verifiable();
            _vmMock.Setup(x => x.Execute(executionSequenceMock.Object)).Throws(new Exception("Execute should not be called"));

            var res = _controlStructureEnvironment.If(new IArgument[] {
                False,
                argExecutableMock.Object,
                Else,
                argExecutableElseMock.Object
            });

            _vmMock.Verify();
            AssertContent(expectedOutput, res);
        }

        [TestMethod()]
        public void If_FalseExecutionElseExecution_ElseValueExecutedWithValues()
        {
            var argExecutableMock = new Mock<IArgumentExecutable>();
            var executionSequenceMock = new Mock<IExecutionSequence>();
            argExecutableMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceMock.Object);
            var thenOutput = Args(Arg1, Arg2 );
            var argExecutableElseMock = new Mock<IArgumentExecutable>();
            var executionSequenceElseMock = new Mock<IExecutionSequence>();
            argExecutableElseMock.SetupGet(x => x.ExecutionSequence).Returns(executionSequenceElseMock.Object);
            var expectedOutput = Args(Arg3, Arg4);
            _vmMock.Setup(x => x.Execute(executionSequenceMock.Object)).Throws(new Exception("Execute should not be called with this execution sequence"));
            _vmMock.Setup(x => x.Execute(executionSequenceElseMock.Object)).Returns(expectedOutput).Verifiable();

            var res = _controlStructureEnvironment.If(new IArgument[] {
                False,
                argExecutableMock.Object,
                Else,
                argExecutableElseMock.Object
            });

            _vmMock.Verify();
            AssertContent(expectedOutput, res);
        }

        [TestMethod()]
        public void If_TrueElseIfTrue_FirstThenValueReturned()
        {
            var res = _controlStructureEnvironment.If(new[] {
                True,
                Arg1,
                Else,
                If,
                True,
                Arg2
            });

            AssertContent(Args(Arg1), res);
        }

        [TestMethod()]
        public void If_FalseElseIfTrue_SecondThenValueReturned()
        {
            var res = _controlStructureEnvironment.If(new[] {
                False,
                Arg1,
                Else,
                If,
                True,
                Arg2
            });

            AssertContent(Args(Arg2), res);
        }
    }
}