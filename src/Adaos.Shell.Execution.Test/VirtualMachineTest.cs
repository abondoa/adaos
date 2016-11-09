using System;
using System.Linq;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Library.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adaos.Shell.Execution.Exceptions;

namespace Adaos.Shell.Execution.Test
{
    [TestClass]
    public class VirtualMachineTest
    {
        VirtualMachine vm;
        StreamWriter systemOut;
        StreamWriter systemLog;
        IVirtualMachine systemMachine;

        private string simpleEcho = "hejsa";
        private string complexEcho = "\"hejsa denne streng er mere complex '      '\"";
        private int logStartLength = ("[" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "] : ").Length;

        [TestInitialize]
        public void SetUp()
        {
            systemLog = new StreamWriter(new MemoryStream());
            systemLog.AutoFlush = true;
            systemOut = new StreamWriter(new MemoryStream());
            systemOut.AutoFlush = true;
            systemMachine = new VirtualMachineBuilder()
                .SetOutputStream(systemOut)
                .SetLogStream(systemLog)
                .AddContextBuilder(Library.StandardLibraryContextBuilder.Instance)
                .Build();
        }

        [TestCleanup]
        public void TearDown()
        {
            vm = null;
        }

        [TestMethod]
        public void ConstructVirtualMachineNoEnvironments()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            Assert.IsNotNull(vm);
            Assert.AreEqual(13, vm.EnvironmentContainer.EnabledEnvironments.Count());
        }

        [TestMethod]
        public void SumWithoutEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("sum 1 2 3 4");
            Assert.AreEqual(10, int.Parse(res.ToArray()[0].Value));
        }

        [TestMethod]
        public void SumWithEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("math.sum 1 2 3 4");
            Assert.AreEqual(10, int.Parse(res.ToArray()[0].Value));
        }

        [TestMethod]
        public void MultiWithoutEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("multi 1 2 3 4");
            Assert.AreEqual(24, int.Parse(res.ToArray()[0].Value));
        }

        [TestMethod]
        public void MultiWithEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("math.multi 1 2 3 4");
            Assert.AreEqual(24, int.Parse(res.ToArray()[0].Value));
        }

        [TestMethod]
        public void MultiNoArguments()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("math.multi");
            Assert.AreEqual(1, int.Parse(res.ToArray()[0].Value));
        }

        [TestMethod]
        public void SumDouble()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("math.sum double 1 2 3 4");
            Assert.AreEqual(10, double.Parse(res.ToArray()[0].Value));
        }

        [TestMethod]
        public void SumNoArguments()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("math.sum");
            Assert.AreEqual(0, int.Parse(res.ToArray()[0].Value));
        }

        //[TestMethod]
        //public void SumHelp()
        //{
        //    vm = new VirtualMachine(systemOut, systemLog);
        //    long temp = mathOut.BaseStream.Position;
        //    var res = vm.InternExecute("math.sum help");
        //    foreach (var a in res)
        //    {
        //        a.ToString();//Now why the hell are we doing this?! I'll tell you! Because InternExecute is lazy :)
        //    }

        //    Assert.AreNotEqual(temp, mathOut.BaseStream.Position); //make sure that the position of stream has changed
        //}

        [TestMethod]
        public void RandomInt()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            var res = vm.InternExecute("math.random 0 1");
            Assert.IsTrue(int.Parse(res.ToArray()[0].Value) >= 0);
            Assert.IsTrue(int.Parse(res.ToArray()[0].Value) <= 1);
        }

        [TestMethod]
        public void EchoSimple()
        {
            long initialStreamPosition = systemOut.BaseStream.Position;
            systemMachine.Execute("echo " + simpleEcho);
            Assert.AreNotEqual(initialStreamPosition, systemOut.BaseStream.Position);
            Assert.AreEqual(simpleEcho, GetOutputString(simpleEcho.Length));
        }


        [TestMethod]
        public void ReturnPipedToEcho()
        {
            long initialStreamPosition = systemOut.BaseStream.Position;
            systemMachine.Execute("return " + simpleEcho + " | echo");
            Assert.AreNotEqual(initialStreamPosition, systemOut.BaseStream.Position);
            Assert.AreEqual(simpleEcho, GetOutputString(simpleEcho.Length));
        }


        [TestMethod]
        public void ReturnConcatenatedPipedToEcho()
        {
            long initialStreamPosition = systemOut.BaseStream.Position;
            int len = simpleEcho.Length * 2 + 1;
            systemMachine.Execute("return " + simpleEcho + ", return " + simpleEcho + " | echo");
            Assert.AreNotEqual(initialStreamPosition, systemOut.BaseStream.Position);
            Assert.AreEqual(simpleEcho + " " + simpleEcho, GetOutputString(len));
        }

        [TestMethod]
        public void EchoComplex()
        {
            long initialStreamPosition = systemOut.BaseStream.Position;
            systemMachine.Execute("echo " + complexEcho);
            Assert.AreNotEqual(initialStreamPosition, systemOut.BaseStream.Position);
            Assert.AreEqual("\"" + GetOutputString(complexEcho.Length - 2) + "\"", complexEcho);
        }

        private string GetOutputString(int length)
        {
            byte[] buffer = new byte[length];
            systemOut.BaseStream.Seek(0, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, length);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            return enc.GetString(buffer);
        }

        [TestMethod]
        public void LogSimple()
        {
            long initialStreamPosition = systemLog.BaseStream.Position;
            systemMachine.Execute("log " + simpleEcho);
            Assert.AreNotEqual(initialStreamPosition, systemLog.BaseStream.Position);
            byte[] buffer = new byte[simpleEcho.Length];
            string str = "";
            systemLog.BaseStream.Seek(logStartLength, SeekOrigin.Begin);
            systemLog.BaseStream.Read(buffer, 0, simpleEcho.Length);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(simpleEcho, str);
        }

        [TestMethod]
        public void LogComplex()
        {
            long initialStreamPosition = systemLog.BaseStream.Position;
            systemMachine.Execute("log " + complexEcho);
            Assert.AreNotEqual(initialStreamPosition, systemLog.BaseStream.Position);
            byte[] buffer = new byte[complexEcho.Length - 2];
            string str = "";
            systemLog.BaseStream.Seek(logStartLength, SeekOrigin.Begin);
            systemLog.BaseStream.Read(buffer, 0, complexEcho.Length - 2);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(complexEcho, "\"" + str + "\"");
        }

        [TestMethod]
        public void TryToAddANewEnvironmentWithTheSameNameAsAnExistingOne()
        {
            int number = systemMachine.EnvironmentContainer.EnabledEnvironments.Count();
            var temp = new StandardEnvironment();
            try
            {
                systemMachine.EnvironmentContainer.LoadEnvironment( temp);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(number, systemMachine.EnvironmentContainer.EnabledEnvironments.Count());
            }
        }

        [TestMethod]
        public void GetPrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            var res = vm.InternExecute("environments").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.EnabledEnvironments.First().Name, res.First().Value);
        }

        [TestMethod]
        public void SetNewPrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("promoteenvironments custom").ToArray();
            var res = vm.InternExecute("environments").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.EnabledEnvironments.First().AsContext().QualifiedName("."), res.First().Value);
            Assert.AreEqual("std.custom", res.First().Value);
        }

        [TestMethod]
        public void GetPrimaryEnvironmentSilent()
        {
            vm = (systemMachine as VirtualMachine);
            var res = vm.InternExecute("environments silent:true").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.EnabledEnvironments.First().Name, res.First().Value);
        }

        [TestMethod]
        public void SetNewPrimaryEnvironmentSilent()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("promoteenvironments custom").ToArray();
            var res = vm.InternExecute("environments silent:true").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.EnabledEnvironments.First().AsContext().QualifiedName("."), res.First().Value);
            Assert.AreEqual("std.custom", res.First().Value);
        }

        [TestMethod]
        public void SetMultiplePrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("promoteenvironments custom system").ToArray();
            var res = vm.InternExecute("environments").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.EnabledEnvironments.First().AsContext().QualifiedName("."), res.First().Value);
            Assert.AreEqual("std.custom", res.First().Value);
            Assert.AreEqual("system", res.Second().Value);
        }

        [TestMethod]
        public void LoadNewModule()
        {
            vm = (systemMachine as VirtualMachine);
            long initialStreamPosition = systemOut.BaseStream.Position;
            vm.Parser.ScannerTable.Escaper = "###";
            vm.InternExecute(@"module.load ""C:\Users\AlexBondo\code-playground\adaos\src\Adaos.Shell.ModuleA\bin\Debug\Adaos.Shell.ModuleA.dll""").ToArray();

            Assert.AreEqual(initialStreamPosition, systemOut.BaseStream.Position);
        }

        [TestMethod]
        public void Execute_ExitInsideSubExecution_ShouldNotThrowExistException()
        {
            long initialStreamPosition = systemOut.BaseStream.Position;
            systemMachine.Execute("echo $exit"); // Should not throw exit terminal exception
            Assert.AreNotEqual(initialStreamPosition, systemOut.BaseStream.Position); // echo should print linebreak
        }

        [TestMethod]
        public void Execute_WhileLoopWithIncrmentingVariable()
        {
            systemMachine.Execute("var i = 0; while (i < 10) (echo $i; i = $(i+1))");
            Assert.AreEqual(GetOutputString(30), @"0
1
2
3
4
5
6
7
8
9
");
        }

        [TestMethod]
        public void Execute_WhileLoopWithIncrmentingVariableInFunction()
        {
            systemMachine.Execute("var func = (var i = 0; while (i < 10) (echo $i; i = $(i+1))); func");
            Assert.AreEqual(GetOutputString(30), @"0
1
2
3
4
5
6
7
8
9
");
        }

        [TestMethod]
        public void Scoping_ScopedVariableDiesWithScope()
        {
            systemMachine.Execute("eval (var i = 0; echo $i)");
            Assert.AreEqual(GetOutputString(3), @"0
");
            try
            {
                systemMachine.Execute("i");
                Assert.Fail("'i' could be executed even though it should be out of scope");
            }
            catch(VMException)
            {

            }
        }

        [TestMethod]
        public void Scoping_CreateScopedVariableThatExistsGlobally()
        {
            systemMachine.Execute("var i = 0; eval (echo $i; var i = 5; echo $i)");
            Assert.AreEqual(GetOutputString(6), @"0
5
");
        }

        [TestMethod]
        public void Scoping_CreateScopedVariableThatExistsOuterScope()
        {
            systemMachine.Execute("eval (var i = 0; eval (echo $i; var i = 5; echo $i))");
            Assert.AreEqual(GetOutputString(6), @"0
5
");
        }
    }
}
