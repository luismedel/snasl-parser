using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace snasl.Lang.Parser
{
    class LiteralMatcher
        : IMatcher
    {
        public TokenType TokenType { get; private set; }
        public int Priority { get; private set; }

        public LiteralMatcher (string value, TokenType tokenType, bool ignoreCase = false, int priority = 1)
        {
            this.TokenType = tokenType;
            this.Priority = priority;
            _value = value;
            _ignoreCase = ignoreCase;
        }

        public TokenMatch Match (string input, int offset)
        {
            if (offset + _value.Length > input.Length)
                return TokenMatch.None;

            var comp = _ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                   : StringComparison.InvariantCulture;

            return input.Substring (offset, _value.Length).Equals (_value, comp)
                 ? new TokenMatch (this.TokenType, _value)
                 : TokenMatch.None;
        }

        readonly string _value;
        readonly bool _ignoreCase;
    }
}
