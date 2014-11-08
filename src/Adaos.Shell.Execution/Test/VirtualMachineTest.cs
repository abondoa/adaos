using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Library.Standard;

namespace Adaos.Shell.Execution.Test
{
    [TestFixture]
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

        [SetUp]
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

        [TearDown]
        public void TearDown()
        {
            vm = null;
        }

        [Test]
        public void ConstructVirtualMachineNoEnvironments()
        {
            vm = new VirtualMachine(systemOut, systemLog);
            Assert.IsNotNull(vm);
            Assert.AreEqual(10,vm.LoadedEnvironments.Count());
        }

        [Test]
        public void ConstructVirtualMachineOneEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            Assert.IsNotNull(vm);
            Assert.IsNotEmpty(vm.LoadedEnvironments.ToArray());
            Assert.AreSame(math, vm.LoadedEnvironments.ToArray()[0]);
        }

        [Test]
        public void SumWithoutEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("sum 1 2 3 4");
            Assert.AreEqual(10, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void SumWithEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("math.sum 1 2 3 4");
            Assert.AreEqual(10, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void MultiWithoutEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("multi 1 2 3 4");
            Assert.AreEqual(24, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void MultiWithEnvironment()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("math.multi 1 2 3 4");
            Assert.AreEqual(24, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void MultiNoArguments()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("math.multi");
            Assert.AreEqual(1, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void SumDouble()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("math.sum double 1 2 3 4");
            Assert.AreEqual(10, double.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void SumNoArguments()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("math.sum");
            Assert.AreEqual(0, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
        public void SumHelp()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            long temp = mathOut.BaseStream.Position;
            var res = vm.InternExecute("math.sum help");
            foreach (var a in res)
            {
                a.ToString();//Now why the hell are we doing this?! I'll tell you! Because InternExecute is lazy :)
            }

            Assert.AreNotEqual(temp, mathOut.BaseStream.Position); //make sure that the position of stream has changed
        }

        [Test]
        public void RandomInt()
        {
            vm = new VirtualMachine(systemOut, systemLog, math);
            var res = vm.InternExecute("math.random 0 1");
            Assert.GreaterOrEqual(1, int.Parse(res.ToArray()[0].Value));
            Assert.LessOrEqual(0, int.Parse(res.ToArray()[0].Value));
        }

        [Test]
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


        [Test]
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


        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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


        [Test]
        public void Redo()
        {
            systemMachine.Execute("echo " + simpleEcho);
            long temp = systemOut.BaseStream.Position;
            systemMachine.Execute("redo");
            Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
            byte[] buffer = new byte[simpleEcho.Length];
            string str = "";
            systemOut.BaseStream.Seek(simpleEcho.Length + 1, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, simpleEcho.Length);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(simpleEcho, str);
        }


        [Test]
        public void RedoWithoutEnvironment()
        {
            systemMachine.Execute("echo " + simpleEcho);
            long temp = systemOut.BaseStream.Position;
            systemMachine.Execute("redo");
            Assert.AreNotEqual(temp, systemOut.BaseStream.Position);
            byte[] buffer = new byte[simpleEcho.Length];
            string str = "";
            systemOut.BaseStream.Seek(simpleEcho.Length + 1, SeekOrigin.Begin);
            systemOut.BaseStream.Read(buffer, 0, simpleEcho.Length);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(buffer);
            Assert.AreEqual(simpleEcho, str);
        }


        [Test]
        public void RedoWithArgumentsCASEinsensiTIVE()
        {
            _redoHelper("1#1", simpleEcho, 1, 1, 1);
        }


        [Test]
        public void RedoWithWrongArguments()
        {
            systemMachine.Execute("echo " + simpleEcho);
            long temp = systemOut.BaseStream.Position;
            try
            {
                systemMachine.Execute("redo back 1 limit 1 count 1");
                Assert.Fail();
                
            }
            catch (AdaosException)
            {
                Assert.AreEqual(temp, systemOut.BaseStream.Position);
            }
        }


        [Test]
        public void RedoWithArgumentsGoFurtherBackThanSizeOfExecutedCache()
        {
            _redoHelper("1#1", simpleEcho, 1000, 1, 1);
        }

        [Test]
        public void RedoSingleLogManyATime()
        {
            _redoHelper("2#1", simpleEcho, 2, 2, 1);
        }

        [Test]
        public void RedoStressTest()
        {
            _redoHelper("5#6", simpleEcho, 324234, 5, 6);
        }

        [Test]
        public void RedoStressTestDifferentStrings()
        {
            _redoHelper("2#6", new string[]{simpleEcho,complexEcho},
                new string[]{simpleEcho,complexEcho.Substring(1,complexEcho.Length-2)}, 324234, 5, 6);
        }

        [Test]
        public void RedoStressTest100()
        {
            _redoHelper("1#100", simpleEcho, 1, 1, 1);
        }

        private void _redoHelper(string sequence, string echoString)
        {
            _redoHelper(sequence, echoString, 1, 1, 1);
        }

        private void _redoHelper(string sequence, string echoString, int back, int limit, int times)
        {
            string[] exploded = sequence.Split('#');
            int initialLogs = int.Parse(exploded[0]);
            string[] strings = new string[initialLogs];
            for(int i = 0 ; i < initialLogs ; ++i)
            {
                strings[i] = echoString;
            }
            _redoHelper(sequence, strings, strings, back, limit, times);
        }

        private void _redoHelper(string sequence, string[] echoStrings, string[] expectedStrings, int back, int limit, int times)
        {
            if (echoStrings.Length != expectedStrings.Length)
            {
                Assert.Inconclusive();
            }
            int initialLogs;
            int initialRedos;
            int logCounter = 0;
            int expectedCounter = 0;
            string[] exploded = sequence.Split('#');
            initialLogs = int.Parse(exploded[0]);
            initialRedos = int.Parse(exploded[1]);
            if(echoStrings.Length != initialLogs)
            {
                Assert.Inconclusive();
            }
            for (int i = 0; i < initialLogs; ++i)
            {
                systemMachine.Execute("echo " + echoStrings[logCounter]);
                logCounter++;
            }
            long temp = systemOut.BaseStream.Position;
            long currentPos = temp;
            
            for (int i = 0; i < initialRedos; ++i)
            {
                systemMachine.Execute("redo back " + back + " Limit " + limit + " TIMES " + times + "");
                back = Math.Min(initialLogs, back);
                systemOut.BaseStream.Seek(temp, SeekOrigin.Begin);

                for (int j = 0; j < times; ++j )
                {
                    int end = Math.Min(initialLogs - back + limit,initialLogs);
                    for (expectedCounter = initialLogs - back; expectedCounter < end; expectedCounter++)
                    {
                        currentPos += expectedStrings[expectedCounter].Length +1;

                        byte[] buffer = new byte[expectedStrings[expectedCounter].Length];
                        string str = "";
                        systemOut.BaseStream.Read(buffer, 0, expectedStrings[expectedCounter].Length);
                        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                        str = enc.GetString(buffer);
                        Assert.AreEqual(expectedStrings[expectedCounter], str);
                        systemOut.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                    }
                }
                systemOut.BaseStream.Seek(0,SeekOrigin.End);
                Assert.AreEqual(systemOut.BaseStream.Position,currentPos);
            }
        }


        [Test]
        public void TryToAddANewEnvironmentWithTheSameNameAsAnExistingOne()
        {
            int number = systemMachine.LoadedEnvironments.Count();
            var temp = new CustomEnvironment();
            try
            {
                systemMachine.LoadEnvironment( temp);
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.AreEqual(number, systemMachine.LoadedEnvironments.Count());
            }
        }

        [Test]
        public void GetPrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            var res = vm.InternExecute("primaryenvironment").ToArray();
            Assert.AreEqual(vm.LoadedEnvironments.First().Name, res.First().Value);
            Assert.AreEqual(vm.PrimaryEnvironment.Name, res.First().Value);
        }

        [Test]
        public void SetNewPrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("primaryenvironment custom").ToArray();
            var res = vm.InternExecute("primaryenvironment").ToArray();
            Assert.AreEqual(vm.LoadedEnvironments.First().Name, res.First().Value);
            Assert.AreEqual(vm.PrimaryEnvironment.Name, res.First().Value);
            Assert.AreEqual("custom", res.First().Value);
        }

        [Test]
        public void GetPrimaryEnvironmentVerbose()
        {
            vm = (systemMachine as VirtualMachine);
            var res = vm.InternExecute("primaryenvironment -verbose").ToArray();
            Assert.AreEqual(vm.LoadedEnvironments.First().Name, res.First().Value);
            Assert.AreEqual(vm.PrimaryEnvironment.Name, res.First().Value);
        }

        [Test]
        public void SetNewPrimaryEnvironmentVerbose()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("primaryenvironment custom").ToArray();
            var res = vm.InternExecute("primaryenvironment -v").ToArray();
            Assert.AreEqual(vm.LoadedEnvironments.First().Name, res.First().Value);
            Assert.AreEqual(vm.PrimaryEnvironment.Name, res.First().Value);
            Assert.AreEqual("custom", res.First().Value);
        }

        [Test]
        public void SetMultiplePrimaryEnvironment()
        {
            vm = (systemMachine as VirtualMachine);
            vm.InternExecute("primaryenvironment custom system").ToArray();
            var res = vm.InternExecute("primaryenvironment").ToArray();
            Assert.AreEqual(vm.LoadedEnvironments.First().Name, res.First().Value);
            Assert.AreEqual(vm.PrimaryEnvironment.Name, res.First().Value);
            Assert.AreEqual("custom", res.First().Value);
            Assert.AreEqual("system",vm.LoadedEnvironments.Second().Name);
        }

        [Test]
        public void LoadNewModule()
        {
            vm = (systemMachine as VirtualMachine);
            long temp = systemOut.BaseStream.Position;
            vm.Parser.ScannerTable.Escaper = "###";
            vm.InternExecute("module.load \"C:\\Users\\aba\\Documents\\MOPHPEZ\\TeamFoundation\\Mophpez\\Source\\MophpezModel\\ADAOS\\ModuleA\\bin\\Debug\\ModuleA.dll\"").ToArray();

            Assert.That(temp, Is.EqualTo(mathOut.BaseStream.Position));
        }
    }
}
