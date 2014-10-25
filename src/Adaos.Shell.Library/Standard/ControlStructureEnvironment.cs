using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.Core.Extenders;
using Adaos.Shell.Core;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Library.Standard
{
    class ControlStructureEnvironment : BaseEnvironment
    {
        public override string Name
        {
            get { return "controlstructure"; }
        }

        public ControlStructureEnvironment()
        {
            Bind(If, "if");
        }

        private IEnumerable<IArgument> If(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(2, x => { throw new SemanticException(-1, x); });
            args.VerifyArgumentMaxCount(3, x => { throw new SemanticException(args.Skip(3).First().Position,x); });
            bool res;
            args.First().TryParseTo(out res, x => { throw new SemanticException(args.First().Position, x); });
            if (res)
            {
                yield return args.Second();
            }
            else if (args.ThirdOrDefault() != null)
            {
                yield return args.Third();
            }
            yield break;
        }
    }
}
