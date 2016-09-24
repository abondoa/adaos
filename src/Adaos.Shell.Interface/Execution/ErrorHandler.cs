namespace Adaos.Shell.Interface.Execution
{
    /// <summary>
    /// A delegate for handling erros in Adoas. 
    /// </summary>
    /// <param name="e">The adaos exception being handled.</param>
    public delegate void ErrorHandler(Exceptions.AdaosException e);
}
