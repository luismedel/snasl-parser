using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class WhileInstr
        : LoopInstr
    {
        public ExprNode Test { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitWhileInstr (this);
    }
}
