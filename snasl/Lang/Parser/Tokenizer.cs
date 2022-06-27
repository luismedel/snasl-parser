using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snasl.Lang.Parser
{
    class Tokenizer
    {
        public Tokenizer (IEnumerable<IMatcher> matchers)
        {
            _matchers = matchers;
        }

        public IEnumerable<Token> Tokenize (string input, TokenType[] skipList = null)
        {
            int offset = 0;
            int line = 1;
            bool applyIgnore = skipList != null && skipList.Length > 0;

            do
            {
                IMatcher lastMatcher = null;
                TokenMatch lastMatch = TokenMatch.None;

                try
                {
                    foreach (var mt in _matchers)
                    {
                        var m = mt.Match (input, offset);
                        if (m == TokenMatch.None)
                            continue;

                        if (lastMatch == TokenMatch.None
                            || m.RawValue.Length > lastMatch.RawValue.Length)
                        {
                            lastMatch = m;
                            lastMatcher = mt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new TokenizerException (line, ex.Message);
                }

                if (lastMatch.Type == TokenType.None)
                    throw new TokenizerException (line, $"Unexpected input '{input.Substring (offset, 5)}...'.");

                line += lastMatch.RawValue.Count (c => c == '\n');

                if (!applyIgnore || !skipList.Contains (lastMatch.Type))
                    yield return new Token (lastMatch, line);

                offset += lastMatch.RawValue.Length;
            } while (offset < input.Length);
        }

        readonly IEnumerable<IMatcher> _matchers;
    }
}
