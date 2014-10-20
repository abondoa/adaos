using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Executer.Extenders;
using Adaos.Shell.Core;

namespace Adaos.Shell.Executer.Environments
{
    class ArgumentEnvironment : Environment
    {
        public override string Name
        {
            get { return "argument"; }
        }

        public ArgumentEnvironment()
        {
            Bind(Return, "return", "ret");
            Bind(Compress, "compress", "comp");
            Bind(Decompress, "decompress", "dec");
            Bind(Head, "head");
            Bind(Tail, "tail");
            Bind(Skip, "skip");
            Bind(Reverse, "reverse", "rev");
            Bind(Sort, "sort");
            Bind(Count, "count","cnt");
            Bind(Length, "length", "len");
            Bind(Negate, "negate", "neg");
            Bind(Unique, "unique");
        }

        private IEnumerable<IArgument> Return(IEnumerable<IArgument> args)
        {
            foreach (var arg in args)
            {
                yield return arg;
            }
        }

        private IEnumerable<IArgument> Reverse(IEnumerable<IArgument> args)
        {
            foreach (var arg in args.Reverse())
            {
                yield return arg;
            }
        }

        private IEnumerable<IArgument> Compress(IEnumerable<IArgument> args)
        {
            StringBuilder str = new StringBuilder();
            var temp = args.FirstOrDefault();
            if (temp == null)
            {
                yield break;
            }
            str.Append(temp.Value);
            foreach (var arg in args.Skip(1))
            {
                str.Append(' ');
                str.Append(arg.Value);
            }
            yield return new DummyArgument(str.ToString(), temp.Position);
        }

        private IEnumerable<IArgument> Decompress(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentCount(1, x => { throw new SemanticException(-1, "Failure in Decompress. " + x); });
            var arg = args.First();

            string[] words = arg.Value.Split(new char[] { ' ', '\t', '\n', '\r' }, 2);
            int adder = 0;
            int startLength = arg.Value.Length;
            int length = startLength;
            for (; words.Length == 2; words = words[1].Split(new char[] { ' ', '\t', '\n', '\r' }, 2))
            {
                yield return new DummyArgument(words[0], arg.Position + adder);
                adder += words[0].Length + (length - words[1].Length);
                length = words[1].Length;
            }
            yield return new DummyArgument(words[0], arg.Position + adder);

            yield break;
        }

        private IEnumerable<IArgument> Head(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(1, x => { throw new SemanticException(-1, "Failure in Head. " + x); });
            var first = args.First();
            int numberOfArgs;

            first.TryParseTo(out numberOfArgs, x => { throw new SemanticException(-1, "Failure in Head. " + x); });
            return args.Skip(1).Take(numberOfArgs);
        }

        private IEnumerable<IArgument> Tail(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(1, x => { throw new SemanticException(-1, "Failure in Tail. " + x); });
            var first = args.First();
            int numberOfArgs;

            first.TryParseTo(out numberOfArgs, x => { throw new SemanticException(-1, "Failure in Tail. " + x); });
            return args.Skip(1).Tail(numberOfArgs);
        }

        private IEnumerable<IArgument> Skip(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(1, x => { throw new SemanticException(-1, "Failure in Skip. " + x); });
            var first = args.First();
            int numberOfArgs;

            first.TryParseTo(out numberOfArgs, x => { throw new SemanticException(-1, "Failure in Skip. " + x); });
            return args.Skip(1).Skip(numberOfArgs);
        }

        private IEnumerable<IArgument> Sort(IEnumerable<IArgument> args)
        {
            foreach (var arg in args.OrderBy(x => x.Value))
            {
                yield return arg;
            }
        }

        private IEnumerable<IArgument> Count(IEnumerable<IArgument> args)
        {
            yield return new DummyArgument(args.Count().ToString());
        }

        private IEnumerable<IArgument> Length(IEnumerable<IArgument> args)
        {
            foreach (var arg in args)
            {
                yield return new DummyArgument(arg.Value.Length.ToString());
            }
        }

        private IEnumerable<IArgument> Negate(IEnumerable<IArgument> args)
        {
            bool b;
            int i;
            double d;
            foreach (var arg in args)
            {
                if (arg.TryParseTo(out d))
                {
                    yield return new DummyArgument((-d).ToString());
                }
                else if (arg.TryParseTo(out i))
                {
                    yield return new DummyArgument((-i).ToString());
                }
                else if (arg.TryParseTo(out b))
                {
                    yield return new DummyArgument((!b).ToString());
                }
                else
                {
                    throw new SemanticException(arg.Position, "Unable to negate. Can only negate booleans, integer, and double precision floats.");
                }
            }
            yield break;
        }

        private IEnumerable<IArgument> Unique(IEnumerable<IArgument> args)
        {
            foreach (var unique in Sort(args).Distinct(new Comparer()))
            {
                yield return unique;
            }
            yield break;
        }
        private class Comparer : IEqualityComparer<IArgument>
        {

            public bool Equals(IArgument x, IArgument y)
            {
                return x.Value.Equals(y.Value);
            }

            public int GetHashCode(IArgument obj)
            {
                return obj.Value.GetHashCode();
            }
        }
    }
}
