using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Parser
{
    class Token
    {
        public static readonly Token None = new Token (TokenType.None, string.Empty, -1);

        public TokenType Type { get; private set; }
        public string Value { get; private set; }
        public int Line { get; private set; }

        public Token (TokenMatch template, int line)
            : this (template.Type, template.Value, line)
        { }

        public Token (TokenType type, string value, int line)
        {
            this.Type = type;
            this.Value = value;
            this.Line = line;
        }
    }
}
