using System;
using System.Collections.Generic;
using snasl.Lang.AST;
using snasl.Lang.Parser;

namespace snasl.Lang.Emitters
{
    class OpcodeEmitter
        : IVisitor
    {
        public OpcodeEmitter ()
        {
        }

        public byte[] EmitNode (IASTNode node)
        {
            _strings.Clear ();
            _code.Clear ();
            Visit (node);
            return _code.ToArray ();
        }

        public void Visit (IASTNode node) => node.Visit (this);

        public void VisitArrayAccess (ArrayAccess node)
        {
            Visit (node.Left);
            Visit (node.IndexExpr);
            EmitOpcode (Opcode.ArrayAccess);
        }

        public void VisitArrayConst (ArrayConst node)
        {
            foreach (var expr in node.ExprItems)
                Visit (expr);
        }

        public void VisitArrayData (ArrayData node)
        {
            //Emit (node.Key);
            //Emit (node.Inner.Raw.Value);
        }

        public void VisitAssignment (Assignment node)
        {
            Visit (node.Left);
            Visit (node.Right);

            switch (node.Op.Type)
            {
                case TokenType.OpAssign: break;
                case TokenType.OpAssignPlus: EmitOpcode (Opcode.Add); break;
                case TokenType.OpAssignMinus: EmitOpcode (Opcode.Sub); break;
                case TokenType.OpAssignMul: EmitOpcode (Opcode.Mul); break;
                case TokenType.OpAssignDiv: EmitOpcode (Opcode.Div); break;
                case TokenType.OpAssignMod: EmitOpcode (Opcode.Mod); break;
                case TokenType.OpAssignRShiftSign: EmitOpcode (Opcode.BitRShiftSign); break;
                case TokenType.OpAssignLShift: EmitOpcode (Opcode.BitLShift); break;
                case TokenType.OpAssignRShift: EmitOpcode (Opcode.BitRShift); break;
                default: throw new InvalidProgramException ($"Unexpected operator '{node.Op.Type}'");
            }

            EmitOpcode (Opcode.Store);
        }

        public void VisitAtom (Atom node)
        {
            if (node.Raw.Type == TokenType.PureString)
            {
                EmitOpcode (Opcode.Push, InternString (node.Raw.Value));
                EmitOpcode (Opcode.PureString);
            }
            else if (node.Raw.Type == TokenType.String)
                EmitOpcode (Opcode.Push, InternString (node.Raw.Value));
            else if (node.Raw.Type == TokenType.Number
                  || node.Raw.Type == TokenType.HexNumer
                  || node.Raw.Type == TokenType.OctNumber
                  || node.Raw.Type == TokenType.BinNumber)
                EmitOpcode (Opcode.Push, node.GetIntValue ());
            else
                throw new InvalidOperationException ();
        }

        public void VisitBinExpr (BinExpr node)
        {
            Visit (node.Left);
            Visit (node.Right);

            switch (node.Op.Type)
            {
                case TokenType.OpPlus: EmitOpcode (Opcode.Add); break;
                case TokenType.OpMinus: EmitOpcode (Opcode.Sub); break;
                case TokenType.OpMul: EmitOpcode (Opcode.Mul); break;
                case TokenType.OpDiv: EmitOpcode (Opcode.Div); break;
                case TokenType.OpMod: EmitOpcode (Opcode.Mod); break;
                case TokenType.OpPow: EmitOpcode (Opcode.Pow); break;

                case TokenType.OpLogAnd: EmitOpcode (Opcode.LogAnd); break;
                case TokenType.OpLogNot: EmitOpcode (Opcode.LogNot); break;
                case TokenType.OpLogOr: EmitOpcode (Opcode.LogOr); break;

                case TokenType.OpBitNeg: EmitOpcode (Opcode.BitNeg); break;
                case TokenType.OpBitAnd: EmitOpcode (Opcode.BitAnd); break;
                case TokenType.OpBitOr: EmitOpcode (Opcode.BitOr); break;
                case TokenType.OpBitXor: EmitOpcode (Opcode.BitXor); break;
                case TokenType.OpRShift: EmitOpcode (Opcode.BitRShift); break;
                case TokenType.OpLShift: EmitOpcode (Opcode.BitLShift); break;
                case TokenType.OpRShiftSign: EmitOpcode (Opcode.BitRShiftSign); break;

                case TokenType.OpInstr: EmitOpcode (Opcode.StrInstr); break;
                case TokenType.OpNotInstr: EmitOpcode (Opcode.StrNotInstr); break;
                case TokenType.OpRegexMatch: EmitOpcode (Opcode.StrRegexMatch); break;
                case TokenType.OpNotRegexMatch: EmitOpcode (Opcode.StrNotRegexMatch); break;

                case TokenType.OpEq: EmitOpcode (Opcode.CompEQ); break;
                case TokenType.OpNotEq: EmitOpcode (Opcode.CompNEQ); break;
                case TokenType.OpLT: EmitOpcode (Opcode.CompLT); break;
                case TokenType.OpGT: EmitOpcode (Opcode.CompGT); break;
                case TokenType.OpLET: EmitOpcode (Opcode.CompLET); break;
                case TokenType.OpGET: EmitOpcode (Opcode.CompGET); break;

                case TokenType.LSquare:
                    break;
                case TokenType.RSquare:
                    break;
                case TokenType.OpArrow:
                    break;
                case TokenType.Dot:
                    break;
            }
        }

        public void VisitBreak (Break node)
        {
        }

        public void VisitContinue (Continue node)
        {
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
            Visit (node.Identifier);
            Visit (node.Array);
            EmitBlock (node.Body.Instructions);
        }

        public void VisitForInstr (ForInstr node)
        {
            Visit (node.InitExpr);
            Visit (node.TestExpr);
            Visit (node.StepExpr);
            EmitBlock (node.Body.Instructions);
        }

        public void VisitFuncCall (FuncCall node)
        {
            Visit (node.Left);
            foreach (var expr in node.Args)
                Visit (expr);
        }

        public void VisitFuncDecl (FuncDecl node)
        {
            EmitBlock (node.Body.Instructions);
        }

        public void VisitIdentifierAccess (IdentifierAccess node)
        {
            EmitOpcode (Opcode.Push, InternString (node.Identifier));
        }

        public void VisitIfInstr (IfInstr node)
        {
            Visit (node.TestExpr);

            EmitOpcode (Opcode.Jnz);
            var trueOffset = CurrentOffset;
            EmitInt (0); //

            EmitOpcode (Opcode.Jmp);
            var falseOffset = CurrentOffset;
            EmitInt (0);

            PatchInt (trueOffset, CurrentOffset - trueOffset);
            EmitBlock (node.Body.Instructions);

            PatchInt (falseOffset, CurrentOffset - falseOffset);
            if (node.ElseBody != null && node.ElseBody.Instructions.Length > 0)
                EmitBlock (node.ElseBody.Instructions);
        }

        public void VisitInclude (Include node)
        {
            Visit (node.Path);
            EmitOpcode (Opcode.LoadModule);
        }

        public void VisitInstrList (InstrList node)
        {
            EmitBlock (node.Instructions);
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
            EmitBlock (node.Instructions);
        }

        public void VisitNamedArgExpr (NamedArgExpr node)
        {
            //Emit ($"{node.Identifier}:");
            //Visit (node.Expr);
        }

        public void VisitRep (Rep node)
        {
            Visit (node.Times);
            Visit (node.Expr);
        }

        public void VisitRepeatInstr (RepeatInstr node)
        {
            throw new NotImplementedException ();
        }

        public void VisitReturn (Return node)
        {
            if (node.Expr == null)
                EmitOpcode (Opcode.Push, 0);
            else
                Visit (node.Expr);

            EmitOpcode (Opcode.Ret);
        }

        public void VisitUnaryExpr (UnaryExpr node)
        {
            if (!node.IsPostOperator)
            {
                //Emit (node.Op.Value);
                //Visit (node.Expr);
            }
            else
            {
                //Visit (node.Expr);
                //Emit (node.Op.Value);
            }
        }

        public void VisitVarDecl (VarDecl node)
        {
            var op = node.IsGlobal ? Opcode.AddGlobal : Opcode.AddLocal;

            foreach (var id in node.Identifiers)
                EmitOpcode (op, InternString (id));
        }

        public void VisitWhileInstr (WhileInstr node)
        {
            throw new NotImplementedException ();
        }

        void EmitBlock (InstrNode[] instructions)
        {
            foreach (var instr in instructions)
                Visit (instr);
        }

        void EmitOpcode (Opcode opcode)
        {
            _code.Add ((byte) opcode);
        }

        void EmitOpcode (Opcode opcode, int argument)
        {
            EmitOpcode (opcode);
            EmitInt (argument);
        }

        void EmitInt (int value)
        {
            _code.Add ((byte) ((value & 0xff000000u) >> 24));
            _code.Add ((byte) ((value & 0x00ff0000u) >> 16));
            _code.Add ((byte) ((value & 0x0000ff00u) >> 8));
            _code.Add ((byte)  (value & 0x000000ffu));
        }

        void Patch (int offset, byte value) => _code[offset] = value;

        void PatchInt (int offset, int value)
        {
            _code[offset++] = (byte) ((value & 0xff000000u) >> 24);
            _code[offset++] = (byte) ((value & 0x00ff0000u) >> 16);
            _code[offset++] = (byte) ((value & 0x0000ff00u) >> 8);
            _code[offset]   = (byte)  (value & 0x000000ffu);
        }

        int InternString (string s)
        {
            int id = _strings.IndexOf (s);
            if (id == -1)
            {
                id = _strings.Count;
                _strings.Add (s);
            }

            return (int)id;
        }

        int CurrentOffset => _code.Count;

        readonly List<string> _strings = new List<string> ();
        readonly List<byte> _code = new List<byte> ();

        enum Opcode: byte
        {
            ArrayAccess,
            Store,
            Jnz,
            AddGlobal,
            AddLocal,
            Push,
            Ret,
            PureString,
            LoadModule,
            Add,
            Sub,
            Mul,
            Div,
            Mod,
            Pow,
            Inc,
            Dec,
            LogAnd,
            LogNot,
            LogOr,
            BitNeg,
            BitAnd,
            BitOr,
            BitRShift,
            BitRShiftSign,
            BitLShift,
            StrInstr,
            StrRegexMatch,
            StrNotInstr,
            StrNotRegexMatch,
            CompEQ,
            CompNEQ,
            CompLT,
            CompGT,
            CompLET,
            CompGET,
            BitXor,
            Jmp
        }
    }
}
