using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class ForInstr
        : LoopInstr
    {
        public ExprNode InitExpr { get; set; }
        public ExprNode TestExpr { get; set; }
        public ExprNode StepExpr { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitForInstr (this);
    }
}
