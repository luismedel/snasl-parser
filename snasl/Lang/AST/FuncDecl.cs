using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class FuncDecl
        : InstrNode
    {
        public string Name { get; set; }
        public string[] Args { get; set; }
        public InstrList Body { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitFuncDecl (this);
    }
}
