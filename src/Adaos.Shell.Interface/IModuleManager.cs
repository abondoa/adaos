using System;
namespace Adaos.Shell.Interface
{
    public interface IModuleManager
    {
        IModule GetInstance(string fileName);
    }
}
