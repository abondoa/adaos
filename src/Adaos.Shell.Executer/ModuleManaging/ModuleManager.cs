using System;
using System.Reflection;
using Adaos.Shell.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Adaos.Shell.Executer.ModuleManaging
{
    class ModuleManager
    {
        private IVirtualMachine _vm;
        private List<Type> _instantiaters;

        public ModuleManager(IVirtualMachine vm)
        {
            _vm = vm;
            _instantiaters = new List<Type>();
            AddInstantiaters(this.GetType().Assembly.GetTypes(),false);
        }

        /// <summary>
        /// Add initializer types.
        /// </summary>
        /// <param name="initializers">IModuleInitializers to be added.</param>
        /// <param name="strict">If any of the initializers is not a IModuleInstantiater</param>
        void AddInstantiaters(IEnumerable<Type> initializers, bool strict = true)
        {
            var actualInitializers = initializers.Where(x => x.GetInterface("IModuleInstantiater") != null);
            if (strict && actualInitializers != initializers)
            {
                throw new ArgumentException("Trying to add non-IModuleInstantiater(s) '" + 
                    initializers.Where(x => x.GetInterface("IModuleInstantiater") == null).Select(x => x.ToString()).Aggregate((x,y) => x + ", " + y) + "' to set of instantiaters in strict mode");
            }
            foreach (var init in actualInitializers)
            {
                if (init.GetConstructor(new Type[] { typeof(IVirtualMachine) }) == null)
                {
                    if (init.GetConstructor(new Type[] { }) == null)
                    {
                        throw new ArgumentException("IModuleInstantiater " + init.ToString() + " found but no standard constructor or IVirualMachine constructor found when trying to register");
                    }
                }
            }
            _instantiaters.AddRange(actualInitializers);
        }

        void RemoveInstantiaters(IEnumerable<Type> initializersToRemove)
        {
            _instantiaters.RemoveAll(x => initializersToRemove.Contains(x));
        }

        public IModule GetInstance(string fileName)
        {
            try
            {
                Assembly moduleAssembly = Assembly.LoadFile(fileName);
                Type[] types = moduleAssembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.GetInterface("IModule") != null)
                    {
                        var constructors = type.GetConstructors();
                        var constructorInputTypes = constructors.FirstOrDefault().GetParameters().Select(x => x.ParameterType);
                        return Instantiate(type, constructorInputTypes.ToArray());
                    }
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new Adaos.Shell.Interface.ModuleMangingException("Unable to find module file: '" + fileName + "'", ex);
            }
            catch (System.IO.FileLoadException ex)
            {
                throw new Adaos.Shell.Interface.ModuleMangingException("Unable to load module file: '" + fileName + "'", ex);
            }

            throw new Adaos.Shell.Interface.ModuleMangingException("Module file: '" + fileName + "' does not contain a module with the interface 'IModule'");
        }

        public IEnumerable<IEnvironment> GetModuleEnvironments(string fileName)
        {
            IModule module = GetInstance(fileName);
            if (module == null)
            {
                yield break;
            }

            foreach (var env in module.Environments)
            {
                yield return env;
            }

            yield break;
        }

        private IModule Instantiate(Type module, params Type[] constructorTypes)
        {
            Type[] types = _instantiaters.ToArray();
            IModuleInstantiater moduleInstantiater;
            string typeString = "<No parameters>";
            if (constructorTypes.FirstOrDefault() != null) // It is not empty
            {
                typeString = constructorTypes.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y);
            }

            foreach (var type in types)
            {
                if (type.GetConstructor(constructorTypes) != null)
                {
                    if (type.GetConstructor(new Type[] { typeof(IVirtualMachine) }) == null)
                    {
                        if (type.GetConstructor(new Type[] { }) == null)
                        {
                            throw new InvalidOperationException("IModuleInstantiater " + type.ToString() + " found for constructor with parameters of types: '" + typeString +
                                "' But no standard constructor or IVirualMachine constructor found for this IModuleInstantiater");
                        }
                        else
                        {
                            moduleInstantiater = (IModuleInstantiater)Activator.CreateInstance(type, new object[] { });
                        }
                    }
                    else
                    {
                        moduleInstantiater = (IModuleInstantiater)Activator.CreateInstance(type, new object[] { _vm });
                    }
                    if (moduleInstantiater == null) // Absolutely last fail safe
                    {
                        throw new InvalidOperationException("IModuleInstantiater '" + type.ToString() + "' seem not to be a IModuleInstantiater after all and cannot be instantiated as such.");
                    }
                    return moduleInstantiater.Instantiate(module);
                }
            }
            throw new ArgumentException("No IModuleInstantiater found for constructor with parameters of types: '" + typeString + "'");
        }
    }
}
