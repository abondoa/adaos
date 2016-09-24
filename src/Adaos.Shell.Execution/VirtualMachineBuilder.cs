using Adaos.Shell.Interface;
using Adaos.Shell.Interface.SyntaxAnalysis;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.ModuleHandling;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Execution
{
    public class VirtualMachineBuilder
    {
        private StreamWriter _logStream;
        private StreamWriter _ouputStream;
        private IEnvironmentContainer _container;
        private IShellParser _parser;
        private IResolver _resolver;
        private IModuleManager _moduleManager;
        private IShellExecutor _shellExecutor;
        private IList<IContextBuilder> _contextBuilders;

        public VirtualMachineBuilder()
        {
            _parser = new Parser();
            _resolver = new Resolver();
            _moduleManager = new ModuleManager();
            _shellExecutor = new ShellExecutor();
            _contextBuilders = new List<IContextBuilder> { new Environments.SystemEnvironmentContextBuilder() };
            _container = new EnvironmentContainer();
        }

        public VirtualMachineBuilder SetLogStream(StreamWriter logStream)
        {
            _logStream = logStream;
            return this;
        }

        public VirtualMachineBuilder SetOutputStream(StreamWriter ouputStream)
        {
            _ouputStream = ouputStream;
            return this;
        }

        public VirtualMachineBuilder SetParser(IShellParser parser)
        {
            _parser = parser;
            return this;
        }

        public VirtualMachineBuilder SetResolver(IResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

        public VirtualMachineBuilder SetModuleManager(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
            return this;
        }

        public VirtualMachineBuilder SetExecutor(IShellExecutor shellExecutor)
        {
            _shellExecutor = shellExecutor;
            return this;
        }

        public VirtualMachineBuilder SetEnvironmentContainer(IEnvironmentContainer container)
        {
            _container = container;
            return this;
        }

        public VirtualMachineBuilder AddContextBuilder(IContextBuilder contextBuilder)
        {
            _contextBuilders.Add(contextBuilder);
            return this;
        }

        public IVirtualMachine Build()
        {
            if (_ouputStream == null)
            {
                throw new InvalidOperationException($"{_ouputStream} not set");
            }
            if (_logStream == null)
            {
                throw new InvalidOperationException($"{_logStream} not set");
            }
            if (_container == null)
            {
                throw new InvalidOperationException($"{_container} not set");
            }
            var vm = new VirtualMachine(_ouputStream, _logStream, _container);
            foreach (var contextBuilder in _contextBuilders)
                _container.LoadEnvironments(contextBuilder.BuildEnvironments(vm));

            return vm;
        }
    }
}
