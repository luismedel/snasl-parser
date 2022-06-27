using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Parser
{
    class ParserException
        : Exception
    {
        public ParserException (int line, string message)
            : base ($"At {line}: {message}")
        { }
    }
}
