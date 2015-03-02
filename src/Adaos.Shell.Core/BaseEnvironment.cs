using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Common;
using Adaos.Shell.Core;
using Adaos.Common.Extenders;
using Adaos.Shell.Core.ArgumentLookup;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Core
{
    abstract public class BaseEnvironment : IEnvironment
    {
        private Dictionary<string, Command> _nameToCommandDictionary;
        private bool _allowUnbinding;

        public BaseEnvironment(bool allowUnbinding = false)
        {
            _nameToCommandDictionary = new Dictionary<string, Command>();
            _allowUnbinding = allowUnbinding;
        }

        abstract public string Name
        {
            get;
        }

        private IEnumerable<IArgument> _commandWrapper(SimpleCommand inner, IEnumerable<IArgument> args)
        {
            return Factory.CreateCachedEnumerable(inner(args));
        }

        private IEnumerable<IArgument> _commandWrapper(Command inner, IEnumerable<IArgument>[] args)
        {
            return Factory.CreateCachedEnumerable(inner(args));
        }

        public virtual void Bind(string commandName, Command command)
        {
            Command actualCommand = x => _commandWrapper(command,x);
            _nameToCommandDictionary.Add(commandName.ToLower(), actualCommand);
        }

        public virtual Command Retrieve(string commandName)
        {
            if (_nameToCommandDictionary.Keys.Contains(commandName.ToLower()))
            {
                return _nameToCommandDictionary[commandName.ToLower()];
            }
            return null;
        }

        public static IEnumerable<IEnvironment> operator +(IEnumerable<IEnvironment> container, BaseEnvironment addition)
        {
            foreach (var item in container)
            {
                yield return item;
            }
            yield return addition;
        }

        public virtual void Bind(Command command, params string[] commandNames)
        {
            if (commandNames.Count() < 1)
            {
                throw new Exception("There must be at least one name for the function to be bound to");
            }
            foreach (var commandName in commandNames)
            {
                Bind(commandName, command);
            }
        }

        public virtual void Bind(SimpleCommand command, params string[] commandNames)
        {
            if (commandNames.Count() < 1)
            {
                throw new Exception("There must be at least one name for the function to be bound to");
            }
            foreach (var commandName in commandNames)
            {
                Bind(commandName, x => command(x.Aggregate((y, z) => y.Then(z))));
            }
        }

        public virtual void Bind(ArgumentLookupCommand command,string commandTemplate)
        {
            var segments = ParseCommandTemplate(commandTemplate);
            if (segments.Count() < 1)
            {
                throw new Exception("There must be at least a name for the function to be bound to");
            }
            
            foreach (var commandName in segments.First().Names)
            {
                Bind(commandName, x =>  command(MapArguments(x.First(),segments.Skip(1).ToArray()), x.Skip(1).ToArray()));
            }
        }

        private IArgumentValueLookup MapArguments(IEnumerable<IArgument> args, ArgumentTemplateSegment[] segments)
        {
            List<KeyValuePair<string, IArgument>> list = new List<KeyValuePair<string, IArgument>>();
            List<IArgument> argsList = new List<IArgument>(args);
            bool hasCatchAll = segments.Last().IsCatchAll;
            foreach (var segment in segments)
            {
                if (segment.IsCatchAll)
                {
                    list.AddRange(argsList.Select(x => new KeyValuePair<string, IArgument>(segment.Names.First(), x)));
                    argsList.Clear();
                    break;
                }
                var value = argsList.SingleOrDefault(x => x.HasName && segment.Names.Contains(x.Name));
                if (value != null)
                {
                    list.Add(new KeyValuePair<string, IArgument>(segment.Names.First(), value));
                    argsList.Remove(value);
                }
                else
                {
                    if (segment.IsRequired || (argsList.Any(x => !x.HasName) && !hasCatchAll))
                    {
                        value = argsList.FirstOrDefault(x => !x.HasName);
                        if(value == null)
                            throw new ArgumentException("Too few arguments");
                        list.Add(new KeyValuePair<string, IArgument>(segment.Names.First(), value));
                        argsList.Remove(value);
                    }
                    else
                    {
                        if (segment.IsRequired)
                            throw new ArgumentException("Missing required argument: "+segment.Names.First());
                        list.Add(new KeyValuePair<string, IArgument>(segment.Names.First(), new DummyArgument( segment.DefaultValue)));
                    }
                }
            }
            if (argsList.Any())
            {
                throw new ArgumentException("Too many arguments");
            }
            return new ArgumentValueLookup(list);
        }

        private ArgumentTemplateSegment[] ParseCommandTemplate(string commandTemplate)
        {
            // Split the template string - it's whitespace separated. 
            var args = commandTemplate.Split(' ').Select(x => new ArgumentTemplateSegment(x.Trim())).ToList();

            // Check for at least one argument
            if (!args.Any())
                throw new ArgumentException(
                    "The commandTemplate must contain at least a commandname.");

            // Check for duplicate names
            if (args.SelectMany(x => x.Names).ContainsDuplicates())
                throw new ArgumentException(
                    "Duplicate keys in the commandTemplate is not allowed.");

            // Check for too many catch-all attributes.
            if (args.Count(x => x.IsCatchAll) > 1)
                throw new ArgumentException(
                    "Only one segment of the commandTemplate may be catch-all.");

            // Check that the catch-all attribute is the last.
            if (args.Any(x => x.IsCatchAll))
            {
                if (args.First() == args.SingleOrDefault(x => x.IsCatchAll))
                    throw new ArgumentException(
                        "The first element is the commandname and cannot be a catch-all segment.");

                // Check that the catch-all attribute is the last.
                if (args.Last() != args.SingleOrDefault(x => x.IsCatchAll))
                    throw new ArgumentException(
                        "The catch-all segment of the commandTemplate must be the last.");
            }
            // return as array.
            return args.ToArray();
        }

        public virtual IEnumerable<string> Commands
        {
            get { return _nameToCommandDictionary.Select(x => x.Key).OrderBy(x => x); }
        }

        public virtual void Unbind(string commandName)
        {
            if (AllowUnbinding)
            {
                _nameToCommandDictionary.Remove(commandName);
            }
            else
            {
                throw new NotSupportedException("This environment ("+Name+") does not support unbinding of commands");
            }
        }

        public bool AllowUnbinding
        {
            get { return _allowUnbinding; }
        }

        public IEnvironmentContext AsContext()
        {
            return new Environments.EnvironmentContext(this, null);
        }


        public string QualifiedName(string separator)
        {
            return Name;
        }

        public virtual IEnumerable<Type> Dependencies
        {
            get { yield break; }
        }
    }
}
