using System;
using System.Collections.Generic;
using System.Text;
using snasl.Lang.Parser;

namespace snasl.Lang.AST
{
    class Atom
        : ExprNode
    {
        public AtomType Type { get; set; }
        public Token Raw { get; set; }

        public int GetIntValue ()
        {
            if (Raw.Type == TokenType.Number)
                return int.Parse (Raw.Value);
            else if (Raw.Type == TokenType.HexNumer)
                return int.Parse (Raw.Value, System.Globalization.NumberStyles.HexNumber);
            // TODO: fix
            else if (Raw.Type == TokenType.OctNumber)
                throw new NotImplementedException ();
            // TODO: fix
            else if (Raw.Type == TokenType.BinNumber)
                throw new NotImplementedException ();
            else
                throw new InvalidOperationException ();
        }

        public override void Visit (IVisitor visitor) => visitor.VisitAtom (this);

        public enum AtomType
        {
            Number,
            String,
            PureString,
            IPAddress
        }
    }
}
