using System.Globalization;
using nodes;
using utils;
using values;

namespace core
{
    public sealed class Parser
    {
        private List<Diagnostic> diagnostics;
        private readonly List<Token> tokens;
        private int index;

        public Parser(ref List<Token> toks, ref List<Diagnostic> diags)
        {
            this.tokens = toks;
            this.diagnostics = diags;
            this.index = 0;
        }

        public NodeCompound Parse()
        {
            List<Node> nodes = new();
            Node node = this.ParseStatement(1);
            while (node.GetNodeType() != NodeType.NodeUndefined)
            {
                nodes.Add(node);
                node = this.ParseStatement(1);
            }

            return new NodeCompound(ref nodes);
        }

        private void SkipNewLines()
        {
            TokenType current = this.Get().Type;
            while (current == TokenType.TokenNewLine)
            {
                this.index++;
                current = this.Get().Type;
            }
        }

        private Token Advance()
        {
            Token old = this.Get();
            this.index++;
            return old;
        }

        private Token Get()
        {
            return this.index >= this.tokens.Count ? this.tokens.Last() : this.tokens[this.index];
        }

        private Token GetAt(int offset)
        {
            int pos = this.index + offset;
            return pos >= this.tokens.Count ? this.tokens.Last() : this.tokens[pos];
        }

        private void Match(TokenType expected, string text)
        {
            Token current = this.Advance();
            if (current.Type != expected)
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    current.Position,
                    String.Format("SyntaxError: Expected '{0}' but was '{1}'", text, current.Content)
                ));
            }
        }

        private Node ParseStatement(byte pmod)
        {
            this.SkipNewLines();
            Token current = this.Get();
            Node? node = null;
            switch (current.Type)
            {
                case TokenType.TokenLeftBrace: return this.ParseCompound(pmod);
                case TokenType.TokenVar: return this.ParseVariableConstant(pmod, false);
                case TokenType.TokenConst: return this.ParseVariableConstant(pmod, true);
                case TokenType.TokenFunc: return this.ParseFunction(pmod);
            }

            node ??= this.ParseController(pmod);
            node ??= this.ParseBinaryUnaryExpression();

            if (node == null)
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    current.Position,
                    "SyntaxError: Invalid statement"
                ));
                return new NodeNoOperation();
            }

            if (pmod % 2 == 1 && node.GetNodeType() != NodeType.NodeUndefined && node.GetNodeType() != NodeType.NodeNoOperation)
            {
                Token actual = this.Get();
                if (actual.Type != TokenType.TokenNewLine && actual.Type != TokenType.TokenSemi && actual.Type != TokenType.TokenEof)
                {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        actual.Position,
                        "SyntaxError: Need new line or semi-colon between two statements"
                    ));
                    return new NodeNoOperation();
                }

                this.Advance();
            }

            return node;
        }

        private Node ParseCompound(byte pmod)
        {
            this.Advance();
            List<Node> nodes = new();
            byte new_pmod = (byte)(pmod % 2 == 1 ? pmod : pmod + 1);
            Token current = this.Get();

            while (current.Type != TokenType.TokenRightBrace && current.Type != TokenType.TokenEof)
            {
                Node stat = this.ParseStatement(new_pmod);
                nodes.Add(stat);
                current = this.Get();
            }

            this.Match(TokenType.TokenRightBrace, "}");
            return new NodeCompound(ref nodes);
        }

        private Node ParseVariableConstant(byte pmod, bool isConstant)
        {
            Position? start = this.Advance().Position;
            List<Node> declarations = new();

            while (this.Get().Type == TokenType.TokenIdentifier)
            {
                Token begin = this.Advance();
                string? kind = null;
                Node? expression = null;
                
                if (this.Get().Type == TokenType.TokenColon)
                {
                    this.Advance();
                    Token actual = this.Advance();
                    if (actual.Type != TokenType.TokenIdentifier)
                    {
                        this.diagnostics.Add(new Diagnostic(
                            DiagnosticType.DiagError,
                            actual.Position,
                            "SyntaxError: ':' after variable or constant name during declaration need a value type"
                        ));
                        kind = "";
                    } else
                        kind = actual.Content;
                }

                if (this.Get().Type == TokenType.TokenEquals)
                {
                    this.Advance();
                    expression = this.ParseStatement(0);
                    if (expression.GetNodeType() == NodeType.NodeNoOperation || expression.GetNodeType() == NodeType.NodeUndefined)
                    {
                        this.diagnostics.Add(new Diagnostic(
                            DiagnosticType.DiagError,
                            begin.Position,
                            "SyntaxError: '=' after variable or constant name during declaration need a correct expression"
                        ));
                    }
                }

                if (kind == null && expression == null)
                {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        begin.Position,
                        "SyntaxError: Variable or constant declaration need a value type or an assignated expression"
                    ));
                    declarations.Add(new NodeNoOperation());
                } else
                    declarations.Add(isConstant ? new NodeConstantDeclaration(begin.Content!, kind, expression) : new NodeVariableDeclaration(begin.Content!, kind, expression));
            
                if (this.Get().Type == TokenType.TokenComma)
                {
                    this.Advance();
                    if (this.Get().Type != TokenType.TokenIdentifier)
                    {
                        this.diagnostics.Add(new Diagnostic(
                            DiagnosticType.DiagError,
                            this.Get().Position,
                            "SyntaxError: Need a variable or constant name after ',' on declarations"
                        ));
                    }
                }
                else
                {
                    if (this.Get().Type == TokenType.TokenIdentifier)
                    {
                        this.diagnostics.Add(new Diagnostic(
                            DiagnosticType.DiagError,
                            this.Get().Position,
                            "SyntaxError: Need a separator ',' between two variables or constants declarations"
                        ));
                    } else
                        break;
                }
            }
            
            if (declarations.Count == 0)
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    start,
                    "SyntaxError: 'var' or 'const' keyword need at least one variable or constant declaration"
                ));
                return new NodeNoOperation();
            }

            return declarations.Count > 1 ? new NodeCompound(ref declarations) : declarations.First();
        }

        private Node ParseFunction(byte pmod)
        {
            this.Advance();
            Token name = this.Advance();
            if (name.Type != TokenType.TokenIdentifier)
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    name.Position,
                    "SyntaxError: 'func' keyword need a function name after to declare a function"
                ));
                return new NodeNoOperation();
            }
            return new NodeNoOperation();
        }

        private Node? ParseController(byte pmod)
        {
            Token current = this.Get();
            if (pmod % 10 >= 4 && current.Type == TokenType.TokenReturn)
            {
                Node? expr = null;
                TokenType tt = this.GetAt(1).Type;
                if (tt != TokenType.TokenNewLine && tt != TokenType.TokenSemi)
                    expr = this.ParseStatement(0);
                return new NodeReturn(ref expr);
            } else if (pmod % 10 >= 2)
            {
                if (current.Type == TokenType.TokenBreak)
                    return new NodeBreak();
                else if (current.Type == TokenType.TokenContinue)
                    return new NodeContinue();
            }

            return null;
        }

        private Node ParseBinaryUnaryExpression(byte precedence=0)
        {
            Node? left = null;
            byte unary_precedence = Token.GetUnaryPrecedence(this.Get().Type);
            if (unary_precedence != 0 && unary_precedence >= precedence)
            {
                if (unary_precedence == 2)
                    left = this.ParseQuestionOperation();
                else
                {
                    Token current = this.Advance();
                    Node operand = this.ParseBinaryUnaryExpression(unary_precedence);
                    left = new NodeUnary(current.Type, ref operand);
                }
            } else
                left = this.ParsePrimaryExpression();

            while (true)
            {
                Token current = this.Get();
                byte binary_precedence = Token.GetBinaryPrecedence(current.Type);
                if (binary_precedence == 0 || binary_precedence <= precedence)
                    break;
                
                this.index++;
                if (binary_precedence == 17)
                    return new NodeBinary(current.Type, ref left);
            
                Node right = this.ParseBinaryUnaryExpression(binary_precedence);
                left = new NodeBinary(current.Type, ref left, ref right);
            }

            return left;
        }

        private Node ParseQuestionOperation()
        {
            Token begin = this.Advance();
            Node condition = this.ParseStatement(0);
            this.Match(TokenType.TokenColon, ":");

            Node first = this.ParseStatement(0);
            this.Match(TokenType.TokenColon, ":");

            Node second = this.ParseStatement(0);
            return new NodeQuestion(ref condition, ref first, ref second);
        }

        private Node ParsePrimaryExpression()
        {
            Token current = this.Advance();
            switch (current.Type)
            {
                case TokenType.TokenLeftParenthesis: {
                    Node expression = this.ParseStatement(0);
                    this.Match(TokenType.TokenRightParenthesis, ")");
                    return new NodeParenthesized(expression);
                }

                case TokenType.TokenInteger: {
                    ulong.TryParse(current.Content, out ulong result);
                    return new NodeLiteral(new ValueUlong(result));
                }

                case TokenType.TokenDecimal: {
                    double.TryParse(current.Content, out double result);
                    return new NodeLiteral(new ValueDouble(result));
                }

                case TokenType.TokenFalse: {
                    return new NodeLiteral(new ValueBool(false));
                }

                case TokenType.TokenTrue: {
                    return new NodeLiteral(new ValueBool(true));
                }

                case TokenType.TokenCharacter: {
                    return new NodeLiteral(new ValueCharacter(current.Content!.First()));
                }

                case TokenType.TokenString: {
                    return new NodeLiteral(new ValueString(current.Content));
                }

                case TokenType.TokenFormatString: {

                } break;

                case TokenType.TokenNull: {
                    return new NodeLiteral(new ValueNull());
                }

                case TokenType.TokenBreak: {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        current.Position,
                        "SyntaxError: Could not use break keyword outside of a loop"
                    ));
                    return new NodeNoOperation();
                }

                case TokenType.TokenContinue: {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        current.Position,
                        "SyntaxError: Could not use continue keyword outside of a loop"
                    ));
                    return new NodeNoOperation();
                }

                case TokenType.TokenReturn: {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        current.Position,
                        "SyntaxError: Could not declare a return statement outside a function"
                    ));
                    return new NodeNoOperation();
                }

                case TokenType.TokenEof: return new NodeUndefined();
            
                case TokenType.TokenIdentifier: return this.ParseAccess(current);
            }

            this.diagnostics.Add(new Diagnostic(
                DiagnosticType.DiagError,
                current.Position,
                "SyntaxError: Incorrect statement found"
            ));
            return new NodeNoOperation();
        }

        private Node ParseAccess(Token current)
        {
            Token actual = this.Get();
            if (actual.Type == TokenType.TokenDot)
            {
                this.Advance();
                actual = this.Advance();
                if (actual.Type != TokenType.TokenIdentifier)
                {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        current.Position,
                        "SyntaxError: '.' is used to access to an attribute of a variable and need an attribute name after"
                    ));
                    return new NodeNoOperation();
                }
                return new NodeAttributeAccess(current.Content!, actual.Content!);
            }
            else if (actual.Type == TokenType.TokenLeftParenthesis)
                return this.ParseFunctionCall(current);
            return new NodeVarConstAccess(current.Content!);
        }

        private Node ParseFunctionCall(Token current)
        {
            return new NodeNoOperation();
        }
    }
}