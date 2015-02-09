using System;
using Adaos.Shell.Interface;
using Adaos.Shell.Library.Standard;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;

namespace Adaos.Shell.Library
{
	public class ContextBuilder
	{
        static ContextBuilder _instance = null;

		public ContextBuilder ()
		{
		}

        public static ContextBuilder Instance 
        { 
            get 
            { 
                if(_instance == null)
                {
                    _instance = new ContextBuilder();
                }
                return _instance;
            } 
        }

		public IEnvironmentContext BuildStandardEnvironment(IVirtualMachine vm)
		{
			var std = new StandardEnvironment ();
            var stdContext = std.AsContext();
			
			var envEnv = new EnvironmentEnvironment(vm.Output, vm);
			stdContext.AddChild(envEnv);
			stdContext.AddChild(new IOEnvironment(vm.Output, vm.Log));
			stdContext.AddChild(new CustomEnvironment());
			stdContext.AddChild(new MathEnvironment(vm.Output));
			stdContext.AddChild(new ArgumentEnvironment());
			stdContext.AddChild(new CommandEnvironment(vm.Output, vm));
			stdContext.AddChild(new ModuleEnvironment(vm.Output, envEnv, vm));
			stdContext.AddChild(new SyntaxEnvironment(vm));
			stdContext.AddChild(new ControlStructureEnvironment());
			stdContext.AddChild(new BareWordsEnvironment()).Do(x => x.IsEnabled = false);
            return stdContext;
		}
	}
}

