using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.Tokens
{
    enum TokenKind
    {
        WORD,
        COMMAND_SEPARATOR,
        COMMAND_CONCATENATOR,
        ENVIRONMENT_SEPARATOR,
        NESTEDWORDS,
        EXECUTE,
        COMMAND_PIPE,
        MATH_SYMBOL,
        ARGUMENT_SEPARATOR,
        EOF
    }
}
