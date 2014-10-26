using System;
using System.Linq;
using Adaos.Shell.Interface;
using Adaos.Shell.Core;
using Adaos.Shell.Core.Extenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adaos.Shell.Core.Extenders.Test
{
    [TestClass]
    public class EnvironmentExtenderTest
    {
        [TestMethod]
        public void FamilyEnvironmentsContexts()
        {
            IEnvironment env1 = new TestEnvironment("1");
            IEnvironment env2 = new TestEnvironment("2");
            IEnvironment env3 = new TestEnvironment("3");
            IEnvironment env4 = new TestEnvironment("4");
            IEnvironment env5 = new TestEnvironment("5");
            env1.AddEnvironment(env2);
            env2.AddEnvironment(env3);
            env3.AddEnvironment(env4);
            env4.AddEnvironment(env5);
            string result = env1.FamilyEnvironments().Last().QualifiedName(".");
            Assert.AreEqual("1.2.3.4.5", result);
        }

        private class TestEnvironment : BaseEnvironment
        {
            private string _name;

            public TestEnvironment(string name)
            {
                _name = name;
            }

            public override string Name
            {
                get { return _name; }
            }
        }
    }
}
