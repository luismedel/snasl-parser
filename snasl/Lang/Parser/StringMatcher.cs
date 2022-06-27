using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Parser
{
    class StringMatcher
        : IMatcher
    {
        public TokenType TokenType { get; }
        public int Priority { get; }

        public StringMatcher (char delimiter, TokenType tokenType, bool escapeChars, int priority = 1)
        {
            this.TokenType = tokenType;
            this.Priority = priority;

            _delimiter = delimiter;
            _escapeChars = escapeChars;
        }

        public TokenMatch Match (string input, int offset)
        {
            if (offset >= input.Length - 1)
                return TokenMatch.None;

            if (input[offset++] != _delimiter)
                return TokenMatch.None;

            StringBuilder raw = new StringBuilder (512);
            raw.Append (_delimiter);

            StringBuilder value = new StringBuilder (512);

            while (true)
            {
                var c = input[offset++];
                raw.Append (c);

                if (_escapeChars && c == '\\')
                {
                    c = input[offset++];
                    raw.Append (c);

                    if (c == 'x')
                    {
                        string val = new string (new char[] { input[offset++], input[offset++] });
                        raw.Append (val);

                        c = (char) int.Parse (val, System.Globalization.NumberStyles.HexNumber);
                    }
                    else
                    {
                        switch (c)
                        {
                            case 'n': c = '\n'; break;
                            case '\'': c = '\''; break;
                            case '\\': c = '\\'; break;
                            case '"': c = '"'; break;
                            case 'r': c = '\r'; break;
                            case 't': c = '\t'; break;
                            case 'v': c = '\v'; break;
                            case '0': c = '\0'; break;
                            default: throw new TokenizerException ($"Unknown scape character '{c}'");
                        }
                    }
                }
                else if (c == _delimiter)
                    break;

                value.Append (c);
            }

            return new TokenMatch (this.TokenType, raw.ToString (), value.ToString ());
        }

        bool _escapeChars;
        readonly char _delimiter;
    }
}
