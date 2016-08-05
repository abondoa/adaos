using System;
using System.Linq;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Library.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adaos.Shell.Execution.Test
{
    [TestClass]
    public class VirtualMachineTest
    {
        VirtualMachine vm;
        IEnvironment math;
        StreamWriter mathOut;
        StreamWriter systemOut;
        StreamWriter systemLog;
        IVirtualMachine systemMachine;

        private string simpleEcho = "hejsa";
        private string complexEcho = "\"hejsa denne streng er mere complex '      '\"";
        private int logStartLength = ("[" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "] : ").Length;

        [TestInitialize]
        public void SetUp()
        {
            mathOut = new StreamWriter(new MemoryStream());
            mathOut.AutoFlush = true;
            math = new MathEnvironment(mathOut);
            systemLog = new StreamWriter(new MemoryStream());
            systemLog.AutoFlush = true;
            systemOut = new StreamWriter(new MemoryStream());
            systemOut.AutoFlush = true;
            systemMachine = new VirtualMachine(systemOut, systemLog);
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
            Assert.AreEqual(12, vm.EnvironmentContainer.LoadedEnvironments.Count());
        }

        //TODO: We are not using the environment input for anything as it is...
        //[TestMethod]
        //public void ConstructVirtualMachineOneEnvironment()
        //{
        //    vm = new VirtualMachine(systemOut, systemLog, math);
        //    Assert.IsNotNull(vm);
        //    Assert.AreEqual(vm.EnvironmentContainer.LoadedEnvironments.ToArray().Length,0);
        //    Assert.AreSame(math, vm.EnvironmentContainer.LoadedEnvironments.ToArray()[0]);
        //}

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
            long temp = systemOut.BaseStream.Position;
            systemMachine.Execute("echo " + simpleEcho);
            Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
            byte[] buffer = new byte[simpleEcho.Length];
            string str = "";
            systemOut.BaseStream.Seek(0, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, simpleEcho.Length);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(simpleEcho, str);
        }


        [TestMethod]
        public void ReturnPipedToEcho()
        {
            long temp = systemOut.BaseStream.Position;
            systemMachine.Execute("return " + simpleEcho + " | echo");
            Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
            byte[] buffer = new byte[simpleEcho.Length];
            string str = "";
            systemOut.BaseStream.Seek(0, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, simpleEcho.Length);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(simpleEcho, str);
        }


        [TestMethod]
        public void ReturnConcatenatedPipedToEcho()
        {
            long temp = systemOut.BaseStream.Position;
            int len = simpleEcho.Length * 2 + 1;
            systemMachine.Execute("return " + simpleEcho + ", return " + simpleEcho + " | echo");
            Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
            byte[] buffer = new byte[len];
            string str = "";
            systemOut.BaseStream.Seek(0, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, len);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(simpleEcho + " " + simpleEcho, str);
        }

        [TestMethod]
        public void EchoComplex()
        {
            long temp = systemOut.BaseStream.Position;
            systemMachine.Execute("echo " + complexEcho);
            Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
            byte[] buffer = new byte[complexEcho.Length-2];
            string str = "";
            systemOut.BaseStream.Seek(0, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, complexEcho.Length-2);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(complexEcho, "\"" + str + "\"");
        }

        [TestMethod]
        public void LogSimple()
        {
            long temp = systemLog.BaseStream.Position;
            systemMachine.Execute("log " + simpleEcho);
            Assert.AreNotEqual(temp, systemLog.BaseStream.Position);
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
            long temp = systemLog.BaseStream.Position;
            systemMachine.Execute("log " + complexEcho);
            Assert.AreNotEqual(temp, systemLog.BaseStream.Position);
            byte[] buffer = new byte[complexEcho.Length - 2];
            string str = "";
            systemLog.BaseStream.Seek(logStartLength, SeekOrigin.Begin);
            systemLog.BaseStream.Read(buffer, 0, complexEcho.Length - 2);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(complexEcho, "\"" + str + "\"");
        }

        //TODO: Do we want the redo command at all?
        //[TestMethod]
        //public void Redo()
        //{
        //    systemMachine.Execute("echo " + simpleEcho);
        //    long temp = systemOut.BaseStream.Position;
        //    systemMachine.Execute("redo");
        //    Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
        //    byte[] buffer = new byte[simpleEcho.Length];
        //    string str = "";
        //    systemOut.BaseStream.Seek(simpleEcho.Length + 1, SeekOrigin.Begin);
        //    systemOut.BaseStream.Read(buffer, 0, simpleEcho.Length);
        //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //    str = enc.GetString(buffer);
        //    Assert.AreEqual(simpleEcho, str);
        //}


        //[TestMethod]
        //public void RedoWithoutEnvironment()
        //{
        //    systemMachine.Execute("echo " + simpleEcho);
        //    long temp = systemOut.BaseStream.Position;
        //    systemMachine.Execute("redo");
        //    Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
        //    byte[] buffer = new byte[simpleEcho.Length];
        //    string str = "";
        //    systemOut.BaseStream.Seek(simpleEcho.Length + 1, SeekOrigin.Begin);
        //    systemOut.BaseStream.Read(buffer, 0, simpleEcho.Length);
        //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //    str = enc.GetString(buffer);
        //    Assert.AreEqual(simpleEcho, str);
        //}


        //[TestMethod]
        //public void RedoWithArgumentsCASEinsensiTIVE()
        //{
        //    _redoHelper("1#1", simpleEcho, 1, 1, 1);
        //}


        //[TestMethod]
        //public void RedoWithWrongArguments()
        //{
        //    systemMachine.Execute("echo " + simpleEcho);
        //    long temp = systemOut.BaseStream.Position;
        //    try
        //    {
        //        systemMachine.Execute("redo back 1 limit 1 count 1");
        //        Assert.Fail();
                
        //    }
        //    catch (AdaosException)
        //    {
        //        Assert.AreEqual(temp, systemOut.BaseStream.Position);
        //    }
        //}


        //[TestMethod]
        //public void RedoWithArgumentsGoFurtherBackThanSizeOfExecutedCache()
        //{
        //    _redoHelper("1#1", simpleEcho, 1000, 1, 1);
        //}

        //[TestMethod]
        //public void RedoSingleLogManyATime()
        //{
        //    _redoHelper("2#1", simpleEcho, 2, 2, 1);
        //}

        //[TestMethod]
        //public void RedoStressTest()
        //{
        //    _redoHelper("5#6", simpleEcho, 324234, 5, 6);
        //}

        //[TestMethod]
        //public void RedoStressTestDifferentStrings()
        //{
        //    _redoHelper("2#6", new string[]{simpleEcho,complexEcho},
        //        new string[]{simpleEcho,complexEcho.Substring(1,complexEcho.Length-2)}, 324234, 5, 6);
        //}

        //[TestMethod]
        //public void RedoStressTest100()
        //{
        //    _redoHelper("1#100", simpleEcho, 1, 1, 1);
        //}

        //private void _redoHelper(string sequence, string echoString)
        //{
        //    _redoHelper(sequence, echoString, 1, 1, 1);
        //}

        //private void _redoHelper(string sequence, string echoString, int back, int limit, int times)
        //{
        //    string[] exploded = sequence.Split('#');
        //    int initialLogs = int.Parse(exploded[0]);
        //    string[] strings = new string[initialLogs];
        //    for(int i = 0 ; i < initialLogs ; ++i)
        //    {
        //        strings[i] = echoString;
        //    }
        //    _redoHelper(sequence, strings, strings, back, limit, times);
        //}

        //private void _redoHelper(string sequence, string[] echoStrings, string[] expectedStrings, int back, int limit, int times)
        //{
        //    if (echoStrings.Length != expectedStrings.Length)
        //    {
        //        Assert.Inconclusive();
        //    }
        //    int initialLogs;
        //    int initialRedos;
        //    int logCounter = 0;
        //    int expectedCounter = 0;
        //    string[] exploded = sequence.Split('#');
        //    initialLogs = int.Parse(exploded[0]);
        //    initialRedos = int.Parse(exploded[1]);
        //    if(echoStrings.Length != initialLogs)
        //    {
        //        Assert.Inconclusive();
        //    }
        //    for (int i = 0; i < initialLogs; ++i)
        //    {
        //        systemMachine.Execute("echo " + echoStrings[logCounter]);
        //        logCounter++;
        //    }
        //    long temp = systemOut.BaseStream.Position;
        //    long currentPos = temp;
            
        //    for (int i = 0; i < initialRedos; ++i)
        //    {
        //        systemMachine.Execute("redo back " + back + " Limit " + limit + " TIMES " + times + "");
        //        back = Math.Min(initialLogs, back);
        //        systemOut.BaseStream.Seek(temp, SeekOrigin.Begin);

        //        for (int j = 0; j < times; ++j )
        //        {
        //            int end = Math.Min(initialLogs - back + limit,initialLogs);
        //            for (expectedCounter = initialLogs - back; expectedCounter < end; expectedCounter++)
        //            {
        //                currentPos += expectedStrings[expectedCounter].Length +1;

        //                byte[] buffer = new byte[expectedStrings[expectedCounter].Length];
        //                string str = "";
        //                systemOut.BaseStream.Read(buffer, 0, expectedStrings[expectedCounter].Length);
        //                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //                str = enc.GetString(buffer);
        //                Assert.AreEqual(expectedStrings[expectedCounter], str);
        //                systemOut.BaseStream.Seek(currentPos, SeekOrigin.Begin);
        //            }
        //        }
        //        systemOut.BaseStream.Seek(0,SeekOrigin.End);
        //        Assert.AreEqual(systemOut.BaseStream.Position,currentPos);
        //    }
        //}


        [TestMethod]
        public void TryToAddANewEnvironmentWithTheSameNameAsAnExistingOne()
        {
            int number = systemMachine.EnvironmentContainer.LoadedEnvironments.Count();
            var temp = new StandardEnvironment();
            try
            {
                systemMachine.EnvironmentContainer.LoadEnvironment( temp);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(number, systemMachine.EnvironmentContainer.LoadedEnvironments.Count());
            }
        }

        [TestMethod]
        public void GetPrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            var res = vm.InternExecute("environments").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.LoadedEnvironments.First().Name, res.First().Value);
        }

        [TestMethod]
        public void SetNewPrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("promoteenvironments custom").ToArray();
            var res = vm.InternExecute("environments").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.LoadedEnvironments.First().AsContext().QualifiedName("."), res.First().Value);
            Assert.AreEqual("std.custom", res.First().Value);
        }

        [TestMethod]
        public void GetPrimaryEnvironmentSilent()
        {
            vm = (systemMachine as VirtualMachine);
            var res = vm.InternExecute("environments silent:true").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.LoadedEnvironments.First().Name, res.First().Value);
        }

        [TestMethod]
        public void SetNewPrimaryEnvironmentSilent()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("promoteenvironments custom").ToArray();
            var res = vm.InternExecute("environments silent:true").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.LoadedEnvironments.First().AsContext().QualifiedName("."), res.First().Value);
            Assert.AreEqual("std.custom", res.First().Value);
        }

        [TestMethod]
        public void SetMultiplePrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("promoteenvironments custom system").ToArray();
            var res = vm.InternExecute("environments").ToArray();
            Assert.AreEqual(vm.EnvironmentContainer.LoadedEnvironments.First().AsContext().QualifiedName("."), res.First().Value);
            Assert.AreEqual("std.custom", res.First().Value);
            Assert.AreEqual("system", res.Second().Value);
        }

        [TestMethod]
        public void LoadNewModule()
        {
            vm = (systemMachine as VirtualMachine);
            long temp = systemOut.BaseStream.Position;
            vm.Parser.ScannerTable.Escaper = "###";
            vm.InternExecute(@"module.load ""C:\Users\AlexBondo\code-playground\adaos\src\Adaos.Shell.ModuleA\bin\Debug\Adaos.Shell.ModuleA.dll""").ToArray();

            Assert.AreEqual(temp, mathOut.BaseStream.Position);
        }

        [TestMethod]
        public void Execute_ExitInsideSubExecution_ShouldNotThrowExistException()
        {
            long temp = systemOut.BaseStream.Position;
            systemMachine.Execute("echo $exit"); // Should not throw exit terminal exception
            Assert.AreEqual(temp, mathOut.BaseStream.Position);
        }
    }
}
