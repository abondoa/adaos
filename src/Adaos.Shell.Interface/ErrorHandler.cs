using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// A delegate for handling erros in Adoas. 
    /// </summary>
    /// <param name="e">The adaos exception being handled.</param>
    public delegate void ErrorHandler(Exceptions.AdaosException e);
}
