using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Executer.Extenders;
using System.IO;
using Adaos.Shell.Core;

namespace Adaos.Shell.Executer.Environments
{
    public  class MathEnvironment : Environment
    {
        private StreamWriter _output;
        private Random _rand;

        public override string Name
        {
            get { return "math"; }
        }

        public MathEnvironment(StreamWriter output)
        {
            _output = output;
            _rand = new Random();
            Bind("sum",       Sum);
            Bind("multi",     Multiplication);
            Bind("random",    Random);
            Bind("sqrt",      Sqrt);
            Bind("pow",       Pow);
        }

        private IEnumerable<IArgument> Sum(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathSum"))
            {
                return new List<IArgument>();
            }
            List<IArgument> result = new List<IArgument>();
            long sum = 0;
            int temp;
            if (args.Count() > 0 && args.First().Value.ToLower().Equals("double"))
            { 
                return SumDouble(args.Skip(1));
            }
            if (args.Count() > 0 && (args.First().Value.ToLower().Equals("int") || args.First().Value.ToLower().Equals("integer")))
            {
                return Sum(args.Skip(1));
            }
            foreach (var arg in args)
            {
                if (int.TryParse(arg.Value, out temp))
                {
                    sum += temp;
                }
                else
                {
                    ReportError("Convertion of: '" + arg.Value + "' to integer failed");
                }
            }
            result.Add(new DummyArgument(false, sum.ToString()));
            return result;
        }

        private IEnumerable<IArgument> SumDouble(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathSum"))
            {
                return new List<IArgument>();
            }
            List<IArgument> result = new List<IArgument>();
            double sum = 0;
            double temp;
            if (args.Count() > 0 && (args.First().Value.ToLower().Equals("int") || args.First().Value.ToLower().Equals("integer")))
            {
                return Sum(args.Except(new IArgument[] { args.First() }));
            }
            if (args.Count() > 0 && args.First().Value.ToLower().Equals("double"))
            {
                return SumDouble(args.Except(new IArgument[] { args.First() }));
            }
            foreach (var arg in args)
            {
                if (double.TryParse(arg.Value, out temp))
                {
                    sum += temp;
                }
                else
                {
                    ReportError("Convertion of: '" + arg.Value + "' to double failed");
                }
            }
            result.Add(new DummyArgument(false, sum.ToString()));
            return result;
        }

        private IEnumerable<IArgument> Multiplication(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathMultiplication"))
            {
                return new List<IArgument>();
            }
            List<IArgument> result = new List<IArgument>();
            long multi = 1;
            int temp;
            if (args.Count() > 0 && args.First().Value.ToLower().Equals("double"))
            {
                return MultiplicationDouble(args.Except(new IArgument[] { args.First() }));
            }
            if (args.Count() > 0 && (args.First().Value.ToLower().Equals("int") || args.First().Value.ToLower().Equals("integer")))
            {
                return Multiplication(args.Except(new IArgument[] { args.First() }));
            }
            foreach (var arg in args)
            {
                if (int.TryParse(arg.Value, out temp))
                {
                    multi *= temp;
                }
                else
                {
                    ReportError("Convertion of: '" + arg.Value + "' to double failed");
                }
            }
            result.Add(new DummyArgument(false, multi.ToString()));
            return result;
        }

        private IEnumerable<IArgument> MultiplicationDouble(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathMultiplication"))
            {
                return new List<IArgument>();
            }
            List<IArgument> result = new List<IArgument>();
            double multi = 1;
            double temp;
            if (args.Count() > 0 && (args.First().Value.ToLower().Equals("int") || args.First().Value.ToLower().Equals("integer")))
            {
                return Multiplication(args.Except(new IArgument[] { args.First() }));
            }
            if (args.Count() > 0 && args.First().Value.ToLower().Equals("double"))
            {
                return MultiplicationDouble(args.Except(new IArgument[] { args.First() }));
            }
            foreach (var arg in args)
            {
                if (double.TryParse(arg.Value, out temp))
                {
                    multi *= temp;
                }
                else
                {
                    ReportError("Convertion of: '" + arg.Value + "' to double failed");
                }
            }
            result.Add(new DummyArgument(false, multi.ToString()));
            return result;
        }

        private IEnumerable<IArgument> Random(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathRandom"))
            {
                return new List<IArgument>();
            }
            List<IArgument> result = new List<IArgument>();
            bool intMode = false;
            bool charMode = false;
            bool doubleMode = false;
            int width = 0;
            bool moding = false;
            bool widthing = false;
            bool firstParsed = false;
            int min = 0;
            int max = 0;

            foreach (var arg in args)
            {
                if (firstParsed)
                {
                    if (!int.TryParse(arg.Value, out max))
                    {
                        ReportError("Unable to parse: '" + arg.Value + "' to integer");
                    }
                    break;
                } 
                else if (moding)
                {
                    moding = false;
                    if (arg.Value.ToLower().Equals("int") || arg.Value.ToLower().Equals("integer"))
                    {
                        intMode = true;
                    }
                    else if (arg.Value.ToLower().Equals("double"))
                    {
                        doubleMode = true;
                    }
                    else if (arg.Value.ToLower().Equals("char"))
                    {
                        charMode = true;
                    }
                    else
                    {
                        ReportError("Invalid mode: '" + arg.Value + "'");
                    }
                    continue;
                }
                else if (widthing)
                {
                    widthing = false;

                    if (!int.TryParse(arg.Value, out width))
                    {
                        ReportError("Unable to parse: '" + arg.Value + "' to width integer");
                    }
                    continue;
                }
                else if (arg.Value.ToLower().Equals("mode"))
                {
                    if (intMode || charMode || doubleMode)
                    {
                        ReportError("Mode occured twice");
                    }
                    moding = true;
                    continue;
                }
                else if (arg.Value.ToLower().Equals("width"))
                {
                    widthing = true;
                    continue;
                }
                else
                {
                    if (!int.TryParse(arg.Value, out min))
                    {
                        ReportError("Unable to parse: '" + arg.Value + "' to integer");
                    }
                    firstParsed = true;
                }
            }

            if (charMode)
            {
                if (intMode || doubleMode)
                {
                    ReportError("Mode occured twice");
                }
                string str = "";
                int len = _rand.Next(min, max);
                while (str.Length < len)
                {
                    str += GenerateRandomChar();
                }
                while (str.Length < width)
                {
                    str = "0" + str;
                }
                result.Add(new DummyArgument(false, str));
            }
            else if (doubleMode)
            {
                if (intMode || charMode)
                {
                    ReportError("Mode occured twice");
                }
                double temp = _rand.NextDouble() * (max - min) + min;
                string str = temp.ToString();
                while (str.Length < width)
                {
                    str = "0" + str;
                }
                result.Add(new DummyArgument(false, temp.ToString()));
            }
            else // int is default
            {
                if (charMode || doubleMode)
                {
                    ReportError("Mode occured twice");
                }
                if (min > max && max == 0)
                {
                    max = min;
                    min = 0;
                }
                int temp = _rand.Next(min, max);
                string str = temp.ToString();
                while (str.Length < width)
                {
                    str = "0" + str;
                }
                result.Add(new DummyArgument(false, temp.ToString()));
            }
            return result;
        }

        private IEnumerable<IArgument> Sqrt(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathSqrt"))
            {
                yield break;
            }
            double temp;
            foreach (var arg in args)
            {
                if (double.TryParse(arg.Value, out temp))
                {
                    DummyArgument result = null;
                    try
                    {
                         result = new DummyArgument(System.Math.Sqrt(temp).ToString());
                    }
                    catch (Exception e)
                    {
                        ReportError(e.Message);
                    }
                    yield return result;
                }
                else
                {
                    ReportError("Unable to parse: '" + arg.Value + "' to a double");
                }
            }
        }

        private IEnumerable<IArgument> Pow(IEnumerable<IArgument> args)
        {
            if (CheckShowHelp(args, "MathPow"))
            {
                yield break;
            }
            List<IArgument> list = args.ToList();
            if(list.Count != 2)
            {
                ReportError("Supposed to receive 2 arguments, received: " + list.Count + ". Try 'help' for more information");
            }
            double b;
            double e;
            if(list[0].TryParseTo(out b, ReportError) && list[1].TryParseTo(out e,ReportError))
            {
                double powered = 0;
                try
                {
                    powered = System.Math.Pow(b,e);
                }
                catch(Exception ex)
                {
                    ReportError(ex.Message);
                }
                yield return new DummyArgument(powered.ToString());
            }
        }

        private bool CheckShowHelp(IEnumerable<IArgument> args, string helpSetting)
        {
            if (args == null || 
                args.Count() == 0 || 
                args.First() == null || 
                args.First().Value == null ||
                !args.First().Value.ToLower().Equals("help")
               )
            {
                return false;
            }
            ShowHelp(helpSetting);
            //throw new Exception(help.Default[helpSetting].ToString());
            return true;
        }

        private void ShowHelp(string helpSetting)
        {
            _output.WriteLine(help.Default[helpSetting]);
        }

        private void ReportError(string errorMessage)
        {
            throw new SemanticException(-1, errorMessage);
        }

        private char GenerateRandomChar()
        {
            return (char)_rand.Next('a', 'z');
        }
    }
}
