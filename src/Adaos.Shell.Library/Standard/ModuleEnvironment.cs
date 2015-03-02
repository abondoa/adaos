using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.Core;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Library.Standard
{
    class ModuleEnvironment : BaseEnvironment
    {
        public override string Name
        {
            get { return "module"; }
        }

        private Dictionary<IEnumerable<string>, IModule> _loadedModules;
        private IModuleManager _moduleManager;
        private IVirtualMachine _vm;
        virtual protected StreamWriter _output { get; private set; }
        private string _stdPath;
        private EnvironmentEnvironment _envEnv;

        public override IEnumerable<Type> Dependencies { get { yield return _envEnv.GetType(); } }

        public ModuleEnvironment(StreamWriter output, EnvironmentEnvironment envEnv, IVirtualMachine vm)
        {
            _vm = vm;
            _loadedModules = new Dictionary<IEnumerable<string>, IModule>();
            _moduleManager = _vm.ModuleManager;
            _envEnv = envEnv;
            _output = output;
            
            Bind(Load, "load");
            Bind(Unload, "unload");
            Bind(Path, "path");
        }

        private IEnumerable<IArgument> Load(IEnumerable<IArgument> args)
        {
            List<IEnvironment> envs = new List<IEnvironment>();
            foreach (var arg in args)
            {
                string fileName = arg.Value;
                IModule module = _moduleManager.GetInstance(fileName);
                envs.AddRange(module.Environments);
                _loadedModules.Add(new List<string> { fileName, module.Name }, module);
            }

			foreach(var env in envs)
            	_vm.EnvironmentContainer.LoadEnvironment(env);

            return envs.Select(x => new DummyArgument(x.Name));
        }

        private IEnumerable<IArgument> Unload(IEnumerable<IArgument> args)
        {
            //TODO: implement
            foreach (var arg in args)
            {
                yield return arg;
            }
        }

        private IEnumerable<IArgument> Path(IEnumerable<IArgument> args)
        {
            //TODO: implement
            foreach (var arg in args)
            {
                yield return arg;
            }
        }
    }
}
