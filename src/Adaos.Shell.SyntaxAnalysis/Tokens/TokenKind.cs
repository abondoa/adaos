using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.Tokens
{
    public enum TokenKind
    {
        WORD,
        NESTEDWORDS,
        EXECUTION_SEPARATOR,
        EXECUTION_CONCATENATOR,
        EXECUTION_PIPE,
        ENVIRONMENT_SEPARATOR,
        EXECUTE,
        MATH_SYMBOL,
        ARGUMENT_SEPARATOR,
        ARGUMENT_EXECUTABLE_START,
        ARGUMENT_EXECUTABLE_STOP,
        EOF
    }
}
