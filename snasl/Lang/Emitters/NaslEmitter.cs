using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using snasl.Lang.AST;
using snasl.Lang.Parser;

namespace snasl.Lang.Emitters
{
    class NaslEmitter
        : IVisitor
    {
        public NaslEmitter (int indentSize)
        {
            _indentSize = indentSize;
            for (int i = 0; i < 10; i++)
                _indent.Add (new string (' ', _indentSize * i));
        }

        public string EmitNode (IASTNode node)
        {
            _buffer.Clear ();
            Visit (node);
            return _buffer.ToString ();
        }

        public void Visit (IASTNode node) => node.Visit (this);

        public void VisitArrayAccess (ArrayAccess node)
        {
            Visit (node.Left);
            Emit ("[");
            Visit (node.IndexExpr);
            Emit ("]");
        }

        public void VisitArrayConst (ArrayConst node)
        {
            Emit ("[");
            foreach (var expr in node.ExprItems)
                Visit (expr);
            Emit ("]");
        }

        public void VisitArrayData (ArrayData node)
        {
            Emit (node.Key);
            Emit (" => ");
            Emit (node.Inner.Raw.Value);
        }

        public void VisitAssignment (Assignment node)
        {
            Visit (node.Left);
            Emit (" = ");
            Visit (node.Right);
        }

        public void VisitAtom (Atom node)
        {
            if (node.Raw.Type == TokenType.PureString)
                Emit ($"'{node.Raw.Value}'");
            else if (node.Raw.Type == TokenType.String)
                Emit ($"\"{node.Raw.Value}\"");
            else if (node.Raw.Type == TokenType.Number)
                Emit (node.Raw.Value);
            else if (node.Raw.Type == TokenType.BinNumber)
                Emit ($"0b{node.Raw.Value}");
            else if (node.Raw.Type == TokenType.HexNumer)
                Emit ($"0x{node.Raw.Value}");
            else if (node.Raw.Type == TokenType.OctNumber)
                Emit ($"0{node.Raw.Value}");
            else
                throw new InvalidOperationException ();
        }

        public void VisitBinExpr (BinExpr node)
        {
            Visit (node.Left);
            Emit ($" {node.Op.Value} ");
            Visit (node.Right);
        }

        public void VisitBreak (Break node)
        {
            Emit ("break");
        }

        public void VisitContinue (Continue node)
        {
            Emit ("continue");
        }

        public void VisitEmptyInstr (EmptyInstr node)
        {
        }

        public void VisitExprNode (ExprNode node)
        {
            throw new NotImplementedException ();
        }

        public void VisitForeachInstr (ForeachInstr node)
        {
            Emit ("foreach ");
            Visit (node.Identifier);
            Emit ("(");
            Visit (node.Array);
            EmitLine (")");
            EmitBlock (node.Body.Instructions);
        }

        public void VisitForInstr (ForInstr node)
        {
            Emit ("for (");
            Visit (node.InitExpr);
            Emit ("; ");
            Visit (node.TestExpr);
            Emit ("; ");
            Visit (node.StepExpr);
            EmitLine (")");
            EmitBlock (node.Body.Instructions);
        }

        public void VisitFuncCall (FuncCall node)
        {
            Visit (node.Left);
            Emit (" (");

            bool first = true;
            foreach (var expr in node.Args)
            {
                if (!first)
                    Emit (", ");

                Visit (expr);
                first = false;
            }

            Emit (")");
        }

        public void VisitFuncDecl (FuncDecl node)
        {
            Emit ($"function {node.Name} (");
            Emit (string.Join (", ", node.Args));
            EmitLine (")");
            EmitBlock (node.Body.Instructions, Braces.Always);
        }

        public void VisitIdentifierAccess (IdentifierAccess node)
        {
            Emit (node.Identifier);
        }

        public void VisitIfInstr (IfInstr node)
        {
            Emit ($"if (");
            Visit (node.TestExpr);
            EmitLine (")");
            EmitBlock (node.Body.Instructions);
            if (node.ElseBody != null && node.ElseBody.Instructions.Length > 0)
            {
                EmitLine ("else");
                EmitBlock (node.ElseBody.Instructions);
            }
        }

        public void VisitInclude (Include node)
        {
            Emit ($"include (");
            Visit (node.Path);
            Emit (")");
        }

        public void VisitInstrList (InstrList node)
        {
            EmitBlock (node.Instructions, Braces.Always);
        }

        public void VisitInstrNode (InstrNode node)
        {
            throw new NotImplementedException ();
        }

        public void VisitLoopInstr (LoopInstr node)
        {
            throw new NotImplementedException ();
        }

        public void VisitModule (Module node)
        {
            foreach (var instr in node.Instructions)
                EmitInstr (instr);
        }

        public void VisitNamedArgExpr (NamedArgExpr node)
        {
            Emit ($"{node.Identifier}:");
            Visit (node.Expr);
        }

        public void VisitRep (Rep node)
        {
            Visit (node.Expr);
            Emit (" x ");
            Visit (node.Times);
        }

        public void VisitRepeatInstr (RepeatInstr node)
        {
            throw new NotImplementedException ();
        }

        public void VisitReturn (Return node)
        {
            Emit ("return");
            if (node.Expr != null)
            {
                Emit (" ");
                Visit (node.Expr);
            }
        }

        public void VisitUnaryExpr (UnaryExpr node)
        {
            if (!node.IsPostOperator)
            {
                Emit (node.Op.Value);
                Visit (node.Expr);
            }
            else
            {
                Visit (node.Expr);
                Emit (node.Op.Value);
            }
        }

        public void VisitVarDecl (VarDecl node)
        {
            if (node.IsGlobal)
                Emit (node.IsGlobal ? "global_var " : "local_var ");
            Emit (string.Join (", ", node.Identifiers));
        }

        public void VisitWhileInstr (WhileInstr node)
        {
            throw new NotImplementedException ();
        }

        void EmitInstr (InstrNode instr)
        {
            Visit (instr);
            if (instr is RepeatInstr || !(instr is LoopInstr || instr is IfInstr || instr is InstrList))
                EmitLine (";");
        }

        void EmitBlock (InstrNode[] instructions, Braces braces=Braces.Default)
        {
            if (braces == Braces.Default)
                braces = _defaultBracesPolicy;

            if (instructions.Length == 0 && braces != Braces.Always)
                EmitLine (";");
            else
            {
                if (braces == Braces.Always || (braces == Braces.Auto && instructions.Length > 1))
                {
                    EmitLine ("{");
                    Indent ();
                }

                foreach (var instr in instructions)
                    EmitInstr (instr);

                if (braces == Braces.Always || (braces == Braces.Auto && instructions.Length > 1))
                {
                    Unindent ();
                    EmitLine ("}");
                }
            }
        }

        void Emit (string text)
        {
            if (_startOfLine)
                _buffer.Append (_indent[_indentLevel]);
            _buffer.Append (text);
            _startOfLine = false;
        }

        void EmitLine (string text)
        {
            if (_startOfLine)
                _buffer.Append (_indent[_indentLevel]);
            _buffer.AppendLine (text);
            _startOfLine = true;
        }

        void EmitLine () => EmitLine (string.Empty);

        void Indent () => _indentLevel++;
        void Unindent () => _indentLevel--;

        readonly List<string> _indent = new List<string> ();
        int _indentLevel = 0;

        bool _startOfLine = true;

        Braces _defaultBracesPolicy = Braces.Always;

        readonly int _indentSize;
        readonly StringBuilder _buffer = new StringBuilder ();

        enum Braces
        {
            Default,
            Auto,
            Always
        }
    }
}
