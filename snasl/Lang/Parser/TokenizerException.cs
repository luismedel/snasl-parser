using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Parser
{
    class TokenizerException
        : Exception
    {
        public int Line { get; private set; }

        public TokenizerException (int line, string message)
            : base ($"At {line}: {message}")
        {
            this.Line = line;
        }

        public TokenizerException (string message)
            : base (message)
        {
            this.Line = -1;
        }
    }
}
