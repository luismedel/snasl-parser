using System;
using System.Collections.Generic;
using System.Text;
using snasl.Lang.Emitters;

namespace snasl.Lang.AST
{
    abstract class InstrNode
        : IASTNode
    {
        public override string ToString ()
        {
            return new NaslEmitter (4).EmitNode (this);
        }

        abstract public void Visit (IVisitor visitor);
    }
}
