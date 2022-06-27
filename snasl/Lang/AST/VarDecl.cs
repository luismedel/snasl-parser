using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class VarDecl
        : InstrNode
    {
        public bool IsGlobal { get; set; }
        public string[] Identifiers { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitVarDecl (this);
    }
}
