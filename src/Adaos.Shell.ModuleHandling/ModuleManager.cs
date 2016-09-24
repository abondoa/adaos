using System;
using System.Reflection;
using Adaos.Shell.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.ModuleHandling
{
    public class ModuleManager : IModuleManager
    {
        public IModule GetInstance(string fileName, IVirtualMachine virtualMachine)
        {
            try
            {
                var moduleType = Assembly.LoadFile(fileName).GetTypes().FirstOrDefault(
                    x => x.GetInterfaces().Contains(typeof(Adaos.Shell.Interface.IModule))
                );
                if (moduleType == null)
                {
                    throw new ModuleMangingException(
                        "Module file: '" + fileName + "' does not contain a module with the interface 'Adaos.Shell.Interface.IModule'");
                }
                return Instantiate(moduleType, virtualMachine);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new ModuleMangingException("Unable to find module file: '" + fileName + "'", ex);
            }
            catch (System.IO.FileLoadException ex)
            {
                throw new ModuleMangingException("Unable to load module file: '" + fileName + "'", ex);
            }
        }

        private IModule Instantiate(Type module, IVirtualMachine virtualMachine)
        {
            if (module.GetConstructor(new Type[] { typeof(IVirtualMachine) }) != null)
            {
                return (IModule)Activator.CreateInstance(module, new object[] { virtualMachine });
            }
            else if (module.GetConstructor(new Type[] { }) != null)
            {
                return (IModule)Activator.CreateInstance(module, new object[] { });
            }
            else
            {
                throw new InvalidOperationException(
                    "Unable to instantiate IModule of type: '" + module.ToString() + "'. It must have a default constructor or a constructor taking an IVritualMachine");
            }
        }
    }
}
