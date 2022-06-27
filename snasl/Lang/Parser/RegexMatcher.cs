using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace snasl.Lang.Parser
{
    class RegexMatcher
        : IMatcher
    {
        public TokenType TokenType { get; private set; }
        public int Priority { get; private set; }

        public RegexMatcher (string pattern, TokenType tokenType, int valueGroup = 0, int priority = 1)
        {
            this.TokenType = tokenType;
            this.Priority = priority;

            _valueGroup = valueGroup;
            _regex = new Regex (@"\G" + pattern);
        }

        public TokenMatch Match (string input, int offset)
        {
            var m = _regex.Match (input, offset);
            if (!m.Success)
                return TokenMatch.None;

            string value = m.Groups[_valueGroup].Value;
            return new TokenMatch (this.TokenType, m.Value, value);
        }

        readonly Regex _regex;
        readonly int _valueGroup;
    }
}
