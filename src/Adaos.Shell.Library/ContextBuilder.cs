using System;
using Adaos.Shell.Interface;
using Adaos.Shell.Library.Standard;

namespace Adaos.Shell.Library
{
	public class ContextBuilder
	{
		public ContextBuilder ()
		{
		}

		public IEnvironmentContext BuildStandardEnvironment(IVirtualMachine vm)
		{
			var std = new StandardEnvironment (vm);
			var stdContext = std.ToContext ();
			
			var envEnv = new EnvironmentEnvironment(vm.Output, vm);
			stdContext.AddEnvironment(envEnv);
			stdContext.AddEnvironment(new IOEnvironment(vm.Output, vm.Log));
			stdContext.AddEnvironment(new CustomEnvironment());
			stdContext.AddEnvironment(new MathEnvironment(vm.Output));
			stdContext.AddEnvironment(new ArgumentEnvironment());
			stdContext.AddEnvironment(new CommandEnvironment(vm.Output, vm));
			stdContext.AddEnvironment(new ModuleEnvironment(vm.Output, envEnv, vm));
			stdContext.AddEnvironment(new SyntaxEnvironment(vm));
			stdContext.AddEnvironment(new ControlStructureEnvironment());
			stdContext.AddEnvironment(new BareWordsEnvironment());
		}
	}
}

