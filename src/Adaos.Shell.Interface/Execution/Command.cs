using System.Collections.Generic;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// A delegate for passing and storing executable commands in Adaos environments.
    /// </summary>
    /// <param name="arguments">The command arguments passed to the delegate function.</param>
    /// <returns>A list of results that may be passed on to any subsequent commands 
    /// through pipelining or similar feature.</returns>
    public delegate IEnumerable<IArgument> Command(params IEnumerable<IArgument>[] arguments);

    /// <summary>
    /// A delegate for passing and storing executable commands in Adaos environments.
    /// </summary>
    /// <param name="arguments">The command arguments passed to the delegate function.</param>
    /// <returns>A list of results that may be passed on to any subsequent commands 
    /// through pipelining or similar feature.</returns>
    public delegate IEnumerable<IArgument> SimpleCommand(IEnumerable<IArgument> arguments);

    /// <summary>
    /// A delegate for passing and storing executable commands in Adaos environments, 
    /// using the <see cref="IArgumentValueLookup"/> for passing arguments to the delegate.
    /// </summary>
    /// <param name="lookup">A lookup table containing the command argument values.</param>
    /// <param name="arguments">The command arguments passed to the delegate function.</param>
    /// <returns>A list of results that may be passed on to any subsequent commands 
    public delegate IEnumerable<IArgument> ArgumentLookupCommand(IArgumentValueLookup lookup, params IEnumerable<IArgument>[] arguments);
}
