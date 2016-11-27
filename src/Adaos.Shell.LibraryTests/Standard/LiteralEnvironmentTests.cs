using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Shell.Library.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface.Execution;
using Moq;

namespace Adaos.Shell.Library.Standard.Tests
{
    [TestClass()]
    public class LiteralEnvironmentTests
    {
        private Mock<IVirtualMachine> _vmMock;
        private LiteralEnvironment _literalEnvironment;

        [TestInitialize]
        public void Setup()
        {
            _vmMock = new Mock<IVirtualMachine>();
            _literalEnvironment = new LiteralEnvironment(_vmMock.Object);
        }

        [TestMethod()]
        public void Retrieve_SingleLiteral()
        {
            var result = _literalEnvironment.Retrieve("1");
            Assert.IsNotNull(result);
        }
    }
}