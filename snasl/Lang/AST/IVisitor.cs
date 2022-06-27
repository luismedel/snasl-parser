using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    interface IVisitor
    {
        void Visit (IASTNode node);

        void VisitArrayAccess (ArrayAccess node);
        void VisitArrayConst (ArrayConst node);
        void VisitArrayData (ArrayData node);
        void VisitAssignment (Assignment node);
        void VisitAtom (Atom node);
        void VisitBinExpr (BinExpr node);
        void VisitBreak (Break node);
        void VisitContinue (Continue node);
        void VisitEmptyInstr (EmptyInstr node);
        void VisitExprNode (ExprNode node);
        void VisitForeachInstr (ForeachInstr node);
        void VisitForInstr (ForInstr node);
        void VisitFuncCall (FuncCall node);
        void VisitFuncDecl (FuncDecl node);
        void VisitIdentifierAccess (IdentifierAccess node);
        void VisitIfInstr (IfInstr node);
        void VisitInclude (Include node);
        void VisitInstrList (InstrList node);
        void VisitInstrNode (InstrNode node);
        void VisitLoopInstr (LoopInstr node);
        void VisitModule (Module node);
        void VisitNamedArgExpr (NamedArgExpr node);
        void VisitRep (Rep node);
        void VisitRepeatInstr (RepeatInstr node);
        void VisitReturn (Return node);
        void VisitUnaryExpr (UnaryExpr node);
        void VisitVarDecl (VarDecl node);
        void VisitWhileInstr (WhileInstr node);
    }
}
