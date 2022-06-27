using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class InstrList
        : InstrNode
    {
        public InstrNode[] Instructions { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitInstrList (this);
    }
}
