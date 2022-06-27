using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snasl.Lang.Parser
{
    class TokenReader
    {
        public TokenReader (IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToArray ();
            _offset = 0;

            Read ();
        }

        public bool Consume (TokenType type)
        {
            if (current.Type != type)
                return false;

            Read ();
            return true;
        }

        public Token ExpectAny (params TokenType[] types)
        {
            if (Array.IndexOf (types, current.Type) == -1)
                throw new TokenizerException (current.Line, $"Unexpected '{current.Type}'.");

            return Read ();
        }

        public Token Expect (TokenType type)
        {
            if (current.Type != type)
                throw new TokenizerException (current.Line, $"Expected '{type}'. Found '{current.Type}'");

            return Read ();
        }

        public Token Read ()
        {
            if (current != null && current.Type == TokenType.EOF)
                return current;

            var result = current;
            current = (_offset >= _tokens.Length) ? new Token (TokenType.EOF, string.Empty, -1) : _tokens[_offset++];
            return result;
        }

        public Token Peek () => current;

        int _offset;
        Token current;

        readonly Token[] _tokens;
    }
}
