using System;

namespace snasl.Lang.Parser
{
    class TokenMatch
    {
        public static readonly TokenMatch None = new TokenMatch (TokenType.None, string.Empty);

        public TokenType Type { get; private set; }
        public string RawValue { get; private set; }
        public string Value { get; private set; }

        TokenMatch () { }

        public TokenMatch (TokenType type, string rawValue, string value = null)
        {
            this.Type = type;
            this.RawValue = rawValue;
            this.Value = value ?? rawValue;
        }

        public override string ToString ()
        {
            return $"[{Type.ToString ()}] '{Value}'";
        }
    }
}
