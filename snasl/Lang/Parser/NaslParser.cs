using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using snasl.Lang.AST;

namespace snasl.Lang.Parser
{
    class NaslParser
    {
        public NaslParser (IEnumerable<Token> tokens)
        {
            _index = 0;
            _tokens = tokens.Where (tok => tok.Type != TokenType.Blank && tok.Type != TokenType.EOL)
                            .Append (new Token (TokenType.EOF, string.Empty, -1))
                            .ToArray ();

            _module = new Module ();
        }

        public Module Parse ()
        {
            if (_module.Instructions == null)
                _module.Instructions = ParseInstrList ();

            return _module;
        }

        InstrNode ParseFuncDecl ()
        {
            if (!TryConsume (TokenType.KeywordFunction))
                return null;

            var result = new FuncDecl ();

            result.Name = Expect (TokenType.Identifier, "Function name expected").Value;
            Expect (TokenType.LParen);
            if (!TryConsume (TokenType.RParen))
            {
                result.Args = ParseFuncArgDecl ();
                Expect (TokenType.RParen);
            }

            result.Body = ParseInstrBlock ();

            return result;
        }

        string[] ParseFuncArgDecl ()
        {
            if (!TryConsume (TokenType.Identifier, out Token identifier))
                return new string[0];

            List<string> result = new List<string> ();

            do { result.Add (Expect (TokenType.Identifier).Value); }
            while (TryConsume (TokenType.Comma));

            return result.ToArray ();
        }

        InstrNode ParseInstr ()
        {
            // empty instruction
            if (TryConsume (TokenType.Semicolon))
                return new EmptyInstr ();

            var result = ParseRep ()
                      ?? ParseExpression ()
                      //?? ParseAssignment()
                      //?? ParseFuncCall()
                      ?? ParseReturn ()
                      ?? ParseInclude ()
                      ?? ParseLocalVarDecl ()
                      ?? ParseGlobalVarDecl ()
                      ?? ParseBreak ()
                      ?? ParseContinue ()
                      ?? ParseIfInstr ()
                      ?? ParseLoop ()
                      ?? ParseInstrBlock (forceCurlys:true);

            if (result != null && (result is RepeatInstr || !(result is IfInstr || result is LoopInstr || result is InstrList)))
                Expect (TokenType.Semicolon);

            return result;
        }

        InstrNode ParseRep ()
        {
            int tindex = TokenIndex;

            var expr = ParseExpression ();
            if (expr == null)
            {
                TokenIndex = tindex;
                return null;
            }

            if (!TryConsume (TokenType.Identifier, out Token id) || id.Value != "x")
            {
                TokenIndex = tindex;
                return null;
            }

            return new Rep {
                Expr = expr,
                Times = ExpectResult (ParseExpression (), "Expression")
            };
        }

        InstrNode ParseReturn ()
        {
            if (!TryConsume (TokenType.KeywordReturn))
                return null;

            return new Return { Expr = ParseExpression () };
        }

        InstrNode ParseInclude ()
        {
            if (!TryConsume (TokenType.KeywordInclude))
                return null;

            Expect (TokenType.LParen);
            var result = new Include { Path = ExpectResult (ParseAtom (), "Path") };
            Expect (TokenType.RParen);
            return result;
        }

        InstrNode ParseBreak ()
        {
            if (!TryConsume (TokenType.KeywordBreak))
                return null;

            return new Break ();
        }

        InstrNode ParseContinue ()
        {
            if (!TryConsume (TokenType.KeywordContinue))
                return null;

            return new Continue ();
        }

        InstrList ParseInstrBlock (bool forceCurlys = false)
        {
            if (TryConsume (TokenType.LCurly))
            {
                var result = ParseInstrList ();
                Expect (TokenType.RCurly);
                return new InstrList { Instructions = result.ToArray () };
            }
            else if (!forceCurlys)
                return new InstrList { Instructions = new InstrNode[] { ExpectResult (ParseInstr (), "Instruction") } };

            return null;
        }

        InstrNode[] ParseInstrList ()
        {
            List<InstrNode> result = new List<InstrNode> ();

            InstrNode instr;
            while ((instr = ParseInstr ()) != null)
                result.Add (instr);

            return result.ToArray ();
        }

        ExprNode ParseFuncCall ()
        {
            int tindex = TokenIndex;

            var id = ParseIdentifier ();
            if (id == null)
            {
                TokenIndex = tindex;
                return null;
            }

            if (!TryConsume (TokenType.LParen))
            {
                TokenIndex = tindex;
                return null;
            }

            var result = new FuncCall {
                Left = id,
                Args = ParseArgExprList ()
            };

            Expect (TokenType.RParen);

            return result;
        }

        ExprNode ParseLValue ()
        {
            return (ExprNode) ParseArrayElem ()
                           ?? ParseIdentifier ();
        }

        InstrNode ParseLoop ()
        {
            return ParseForInstr ()
                ?? ParseForeachInstr ()
                ?? ParseWhileInstr ()
                ?? ParseRepeatInstr ();
        }

        InstrNode ParseForInstr ()
        {
            if (!TryConsume (TokenType.KeywordFor))
                return null;

            var result = new ForInstr ();

            Expect (TokenType.LParen);

            result.InitExpr = ParseExpression ();
            Expect (TokenType.Semicolon);
            result.TestExpr = ParseExpression ();
            Expect (TokenType.Semicolon);
            result.StepExpr = ParseExpression ();

            Expect (TokenType.RParen);

            result.Body = ParseInstrBlock ();

            return result;
        }

        InstrNode ParseForeachInstr ()
        {
            if (!TryConsume (TokenType.KeywordForeach))
                return null;

            var result = new ForeachInstr ();

            result.Identifier = ExpectResult (ParseIdentifier (), "Identifier");
            Expect (TokenType.LParen);
            result.Array = ExpectResult (ParseExpression (), "Array expression");
            Expect (TokenType.RParen);
            result.Body = ParseInstrBlock ();

            return result;
        }

        InstrNode ParseWhileInstr ()
        {
            if (!TryConsume (TokenType.KeywordWhile))
                return null;

            var result = new WhileInstr ();

            Expect (TokenType.LParen);
            result.Test = ExpectResult (ParseExpression (), "Expression");
            Expect (TokenType.RParen);

            result.Body = ParseInstrBlock ();

            return result;
        }

        InstrNode ParseRepeatInstr ()
        {
            if (!TryConsume (TokenType.KeywordRepeat))
                return null;

            var result = new RepeatInstr ();

            result.Body = ParseInstrBlock ();

            if (!TryConsume (TokenType.KeywordUntil))
                return null;

            result.Test = ExpectResult (ParseExpression (), "Expression");

            return result;
        }

        InstrNode ParseIfInstr ()
        {
            if (!TryConsume (TokenType.KeywordIf))
                return null;

            var result = new IfInstr ();

            Expect (TokenType.LParen);
            result.TestExpr = ParseExpression ();
            Expect (TokenType.RParen);

            result.Body = ParseInstrBlock ();
            if (TryConsume (TokenType.KeywordElse))
                result.ElseBody = ParseInstrBlock ();

            return result;
        }

        Assignment ParseAssignment2 ()
        {
            int tindex = TokenIndex;

            var left = ParseLValue ();
            if (left == null)
            {
                TokenIndex = tindex;
                return null;
            }

            if (TryConsume (TokenType.OpAssign, out Token op)
             || TryConsume (TokenType.OpAssignDiv, out op)
             || TryConsume (TokenType.OpAssignLShift, out op)
             || TryConsume (TokenType.OpAssignMinus, out op)
             || TryConsume (TokenType.OpAssignMod, out op)
             || TryConsume (TokenType.OpAssignMul, out op)
             || TryConsume (TokenType.OpAssignPlus, out op)
             || TryConsume (TokenType.OpAssignRShift, out op)
             || TryConsume (TokenType.OpAssignRShiftSign, out op))
            {
                return new Assignment {
                    Left = left,
                    Op = op,
                    Right = ExpectResult (ParseExpression (), "Assignment expression")
                };
            }

            TokenIndex = tindex;
            return null;
        }

        ExprNode ParseExpr2 ()
        {
            int tindex = TokenIndex;

            // Pre operator
            Token preOp = Token.None;
            var hasPreOp = TryConsume (TokenType.OpInc, out preOp)
                        || TryConsume (TokenType.OpDec, out preOp)
                        || TryConsume (TokenType.OpBitNeg, out preOp)
                        || TryConsume (TokenType.OpPlus, out preOp)
                        || TryConsume (TokenType.OpMinus, out preOp)
                        || TryConsume (TokenType.OpLogNot, out preOp);

            ExprNode result;

            if (hasPreOp)
            {
                result = new UnaryExpr {
                    IsPostOperator = false,
                    Op = preOp,
                    Expr = ExpectResult (ParseExpression (), "Expression")
                };
            }
            else if (TryConsume (TokenType.LParen))
            {
                result = ExpectResult (ParseExpression (), "Expression");
                Expect (TokenType.RParen);
            }
            else
            {
                result = ParseAssignment2 ()
                      ?? ParseFuncCall ()
                      ?? ParseAtom ()
                      ?? ParseVar ()
                      ?? ParseArrayConst ();
            }

            result = ParseBinaryExpr (result);

            // Post operator
            Token postOp;
            if (TryConsume (TokenType.OpInc, out postOp)
             || TryConsume (TokenType.OpDec, out postOp))
                result = new UnaryExpr { IsPostOperator = true, Op = postOp, Expr = result };

            return result;
        }

        ExprNode ParseExpression ()
        {
            bool preInc = TryConsume (TokenType.OpInc, out Token tpre)
                       || TryConsume (TokenType.OpInc, out tpre);

            ExprNode left = ParseSimpleExpression ();
            if (left == null)
                return null;

            if (preInc)
                left = new UnaryExpr { IsPostOperator = false, Op = tpre, Expr = left };

            if (!TryConsumeOperator (out Token top))
                return ExpressionReducer.Reduce (left);

            ExprNode right = ExpectResult (ParseSimpleExpression (), "Term");
            BinExpr result = new BinExpr { Left = left, Right = right, Op = top };

            int opPrecedence = GetOperatorPrecedence (top.Type);
            if (opPrecedence == -1)
                throw new ParserException (CurrentLine, $"Unknown operator '{top.Value}'");

            while (TryConsumeOperator (out top))
            {
                right = ExpectResult (ParseSimpleExpression (), "Term");

                int prevPrecedence = opPrecedence;

                opPrecedence = GetOperatorPrecedence (top.Type);
                if (opPrecedence == -1)
                    throw new ParserException (CurrentLine, $"Unknown operator '{top.Value}'");

                ExprNode newLeft;
                ExprNode newRight;
                if (prevPrecedence < opPrecedence)
                {
                    newLeft = result.Left;
                    newRight = new BinExpr { Left = result.Right, Right = right, Op = result.Op };
                }
                else
                {
                    newLeft = result;
                    newRight = right;
                }

                result = new BinExpr { Left = newLeft, Right = newRight, Op = top };
            }

            if (result.Right == null)
                return ExpressionReducer.Reduce (result.Left);

            return ExpressionReducer.Reduce (result);
        }

        ExprNode ParseSimpleExpression ()
        {
            ExprNode result = null;

            // Unary operator
            while (TryConsume (TokenType.OpPlus, out Token top)
                || TryConsume (TokenType.OpMinus, out top)
                || TryConsume (TokenType.OpLogNot, out top)
                || TryConsume (TokenType.OpBitNeg, out top)
                || TryConsume (TokenType.OpInc, out top)
                || TryConsume (TokenType.OpDec, out top))
            {
                result = result == null ? new UnaryExpr { IsPostOperator = false, Op = top, Expr = ExpectResult (ParseTerm (), "term") }
                                        : new UnaryExpr { IsPostOperator = false, Op = top, Expr = result };
            }

            // Simple term 
            if (result == null)
                result = ParseTerm ();

            if (result == null)
                return null;

            // Unary operator
            while (TryConsume (TokenType.OpInc, out Token top)
                || TryConsume (TokenType.OpDec, out top))
                result = new UnaryExpr { IsPostOperator = true, Op = top, Expr = result };

            return result;
        }

        ExprNode ParseTerm ()
        {
            Atom atom = ParseAtom ();
            if (atom != null)
                return atom;

            if (TryConsume (TokenType.LParen))
            {
                ExprNode expr = ParseExpression ();
                Expect (TokenType.RParen);

                return expr;
            }

            return ParseVar ()
                ?? ParseArrayConst ();
        }

        ExprNode[] ParseArgExprList ()
        {
            List<ExprNode> result = new List<ExprNode> ();

            do
            {
                var expr = ParseExpression ();
                if (expr is IdentifierAccess && TryConsume (TokenType.Colon))
                {
                    expr = new NamedArgExpr {
                        Identifier = ((IdentifierAccess) expr).Identifier,
                        Expr = ExpectResult (ParseExpression (), "Expression")
                    };
                }

                if (expr != null)
                    result.Add (expr);
                else if (result.Count > 0)
                    throw new ParserException (CurrentLine, "Expression expected");
            } while (TryConsume (TokenType.Comma));

            return result.ToArray ();
        }

        ExprNode[] ParseExprList ()
        {
            var expr = ParseExpression ();
            if (expr == null)
                return new ExprNode[0];

            List<ExprNode> result = new List<ExprNode> { expr };

            while (TryConsume (TokenType.Comma))
                result.Add (ExpectResult (ParseExpression (), "Expression expected."));

            return result.ToArray ();
        }

        ExprNode ParseBinaryExpr (ExprNode left)
        {
            if (left == null)
                return null;

            Token op;
            if (TryConsume (TokenType.OpInstr, out op)
             || TryConsume (TokenType.OpNotInstr, out op)
             || TryConsume (TokenType.OpRegexMatch, out op)
             || TryConsume (TokenType.OpNotRegexMatch, out op)
             || TryConsume (TokenType.OpLT, out op)
             || TryConsume (TokenType.OpGT, out op)
             || TryConsume (TokenType.OpEq, out op)
             || TryConsume (TokenType.OpNotEq, out op)
             || TryConsume (TokenType.OpLET, out op)
             || TryConsume (TokenType.OpGET, out op)

             || TryConsume (TokenType.OpBitAnd, out op)
             || TryConsume (TokenType.OpBitXor, out op)
             || TryConsume (TokenType.OpBitOr, out op)
             || TryConsume (TokenType.OpLShift, out op)
             || TryConsume (TokenType.OpRShift, out op)
             || TryConsume (TokenType.OpRShiftSign, out op)

             || TryConsume (TokenType.OpPlus, out op)
             || TryConsume (TokenType.OpMinus, out op)
             || TryConsume (TokenType.OpMul, out op)
             || TryConsume (TokenType.OpDiv, out op)
             || TryConsume (TokenType.OpMod, out op)
             || TryConsume (TokenType.OpPow, out op)

             || TryConsume (TokenType.OpLogAnd, out op)
             || TryConsume (TokenType.OpLogOr, out op)
             )
            {
                var right = ExpectResult (ParseExpression (), "Expression");
                return new BinExpr { Left = left, Op = op, Right = right };
            }
            else
                return left;
        }

        ExprNode ParseVar ()
        {
            return ParseFuncCall ()
                ?? ParseNumArg ()
                ?? ParseArrayElem ()
                ?? (ExprNode) ParseIdentifier ();
        }

        IdentifierAccess ParseIdentifier ()
        {
            if (!TryConsume (TokenType.Identifier, out Token id))
                return null;

            return new IdentifierAccess { Identifier = id.Value };
        }

        ExprNode ParseNumArg  ()
        {
            // TODO: fix
            return null;
        }


        ArrayAccess ParseArrayElem ()
        {
            int tindex = TokenIndex;

            var id = ParseIdentifier ();
            if (id == null)
            {
                TokenIndex = tindex;
                return null;
            }

            if (!TryConsume (TokenType.LSquare))
            {
                TokenIndex = tindex;
                return null;
            }

            var result = new ArrayAccess {
                Left = id,
                IndexExpr = ExpectResult (ParseExpression (), "Array index")
            };

            Expect (TokenType.RSquare);

            return result;
        }

        ArrayConst ParseArrayConst ()
        {
            int tindex = TokenIndex;

            if (!TryConsume (TokenType.LSquare))
                return null;

            List<ExprNode> list = new List<ExprNode> ();

            do
            {
                ExprNode data = ParseArrayData ();
                if (data == null)
                    throw new ParserException (CurrentLine, "Array data expected");

                list.Add (data);
            } while (TryConsume (TokenType.Comma));

            Expect (TokenType.RSquare);

            return new ArrayConst { ExprItems = list.ToArray () };
        }

        ExprNode ParseArrayData ()
        {
            int tindex = TokenIndex;

            var atom = ParseAtom ();
            if (atom != null)
                return atom;

            if (!TryConsume (TokenType.Identifier, out var id))
            {
                TokenIndex = tindex;
                return null;
            }

            Expect (TokenType.OpArrow);

            atom = ParseAtom ();
            if (atom == null)
            {
                TokenIndex = tindex;
                return null;
            }

            return new ArrayData { Key = id.Value, Inner = atom };
        }

        Atom ParseAtom ()
        {
            return ParseString ()
                ?? ParseNumber ()
                ?? ParseIPAddress ();
        }

        Atom ParseNumber ()
        {
            Token tok;
            if (TryConsume (TokenType.Number, out tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.Number };
            else if (TryConsume (TokenType.BinNumber, out tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.Number };
            else if (TryConsume (TokenType.HexNumer, out tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.Number };
            else if (TryConsume (TokenType.OctNumber, out tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.Number };

            return null;
        }

        Atom ParseIPAddress ()
        {
            if (TryConsume (TokenType.IPAddress, out var tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.IPAddress };
            return null;
        }

        Atom ParseString ()
        {
            Token tok;
            if (TryConsume (TokenType.String, out tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.String };
            else if (TryConsume (TokenType.PureString, out tok))
                return new Atom { Raw = tok, Type = Atom.AtomType.PureString };

            return null;
        }

        string[] ParseArgsDecl ()
        {
            if (!TryConsume (TokenType.Identifier, out var id))
                return null;

            var args = new List<string> { id.Value };
            while (TryConsume (TokenType.Comma))
                args.Add (Expect (TokenType.Identifier).Value);

            return args.ToArray ();
        }

        VarDecl ParseVarDecl (TokenType keyword)
        {
            int tindex = TokenIndex;

            if (!TryConsume (keyword))
            {
                TokenIndex = tindex;
                return null;
            }

            var list = ParseArgsDecl ();
            if (list == null)
            {
                TokenIndex = tindex;
                return null;
            }

            return new VarDecl {
                IsGlobal = keyword == TokenType.KeywordGlobalVar,
                Identifiers = list.ToArray ()
            };
        }

        VarDecl ParseLocalVarDecl  () => ParseVarDecl (TokenType.KeywordLocalVar);
        VarDecl ParseGlobalVarDecl () => ParseVarDecl (TokenType.KeywordGlobalVar);

        T ExpectResult<T> (T result, string expected)
            where T:IASTNode
        {
            if (result == null)
                throw new ParserException (CurrentLine, $"{expected} expected.");

            return result;
        }

        Token Expect (TokenType type, string message = "")
        {
            var next = Read ();
            if (next.Type != type)
            {
                var err = string.IsNullOrEmpty (message)
                        ? $"'{type.ToString ()}' expected. Found '{next.Type.ToString ()}'."
                        : string.Format (message, type, next.Type);

                throw new ParserException (next.Line, err);
            }

            return next;
        }

        bool TryConsume (TokenType type, out Token result)
        {
            result = Token.None;
            if (Peek ().Type != type)
                return false;

            result = Read ();
            return true;
        }

        bool TryConsumeOperator (out Token result)
        {
            result = Token.None;
            if (GetOperatorPrecedence (Peek ().Type) == -1)
                return false;

            result = Read ();
            return true;
        }

        bool TryConsume (TokenType type) => TryConsume (type, out _);

        int TokenIndex
        {
            get => _index;
            set => _index = value;
        }

        Token Read ()
        {
            var result = _tokens[_index];
            _index = Math.Min (_tokens.Length, _index + 1);
            return result;
        }

        Token Peek () => _tokens[_index];
        int CurrentLine => Peek ().Line;


        static int GetOperatorPrecedence (TokenType op) => Array.IndexOf (Operators, op);


        int _index = -1;

        readonly Token[] _tokens;

        readonly Module _module;

        static readonly TokenType[] Operators = new TokenType[] {
            TokenType.OpMul,
            TokenType.OpDiv,
            TokenType.OpMod,
            TokenType.OpPlus,
            TokenType.OpMinus,
            TokenType.OpLShift,
            TokenType.OpRShift,
            TokenType.OpRShiftSign,
            TokenType.OpLT,
            TokenType.OpLET,
            TokenType.OpGT,
            TokenType.OpGET,
            TokenType.OpEq,
            TokenType.OpNotEq,
            TokenType.OpInstr,
            TokenType.OpNotInstr,
            TokenType.OpRegexMatch,
            TokenType.OpNotRegexMatch,
            TokenType.OpBitAnd,
            TokenType.OpBitXor,
            TokenType.OpBitOr,
            TokenType.OpLogAnd,
            TokenType.OpLogOr,
            TokenType.OpAssign,
            TokenType.OpAssignMul,
            TokenType.OpAssignDiv,
            TokenType.OpAssignMod,
            TokenType.OpAssignPlus,
            TokenType.OpAssignMinus,
            TokenType.OpAssignLShift,
            TokenType.OpAssignRShift,
            TokenType.OpAssignRShiftSign,
        };
    }
}
