using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Parser
{
    interface IMatcher
    {
        TokenType TokenType { get; }
        int Priority { get; }
        TokenMatch Match (string input, int offset);
    }
}
