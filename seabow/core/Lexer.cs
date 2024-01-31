using core;
using utils;

namespace core
{
    public sealed class Lexer
    {
        private readonly string code;
        private int index;
        private Position pos;
        private List<Diagnostic> diagnostics;

        public Lexer(ref string code, ref List<Diagnostic> diags)
        {
            this.code = code;
            this.pos = new Position(1, 1);
            this.index = 0;
            this.diagnostics = diags;
        }

        public List<Token> Lex()
        {
            List<Token> tokens = new();

            Token actual = this.LexToken();
            do
            {
                if (actual.Type != TokenType.TokenUndefined)
                    tokens.Add(actual);
                actual = this.LexToken();
            } while (actual.Type != TokenType.TokenEof);

            tokens.Add(actual);
            return tokens;
        }

        private Token LexToken()
        {
            this.SkipSpaces();
            char current = this.Get();
            if (current == '\0')
                return new Token(TokenType.TokenEof, null, new Position(this.pos.Line, this.pos.Column));
        
            if (current == '/')
            {
                char _next = this.GetAt(1);
                if (_next == '/' || _next == '*')
                {
                    Token? tok = this.SkipComments();
                    if (tok != null) return tok;
                }
                else if (_next == '=')
                    return this.AdvanceWith(2, TokenType.TokenSlashEquals);
                else
                    return this.AdvanceWith(1, TokenType.TokenSlash);
            }

            if (Char.IsLetter(current) || current == '_')
                return this.LexWord();
            else if (Char.IsDigit(current))
                return this.LexNumber();
            
            char next = this.GetAt(1);
            switch (current)
            {
                case '\n': {
                    int col = this.pos.Column;
                    this.index++;
                    this.pos.Column = 1;
                    this.pos.Line++;
                    return new Token(TokenType.TokenNewLine, null, new Position(this.pos.Line-1, col));
                }

                case '#': return this.LexMacro();
                case '"': return this.LexString();
                case '\'': return this.LexCharacter();
                case '@': return this.LexFormatString();

                case '(': return this.AdvanceWith(1, TokenType.TokenLeftParenthesis);
                case ')': return this.AdvanceWith(1, TokenType.TokenRightParenthesis);
                case '[': return this.AdvanceWith(1, TokenType.TokenLeftBracket);
                case ']': return this.AdvanceWith(1, TokenType.TokenRightBracket);
                case '{': return this.AdvanceWith(1, TokenType.TokenLeftBrace);
                case '}': return this.AdvanceWith(1, TokenType.TokenRightBrace);

                case ';': return this.AdvanceWith(1, TokenType.TokenSemi);
                case ',': return this.AdvanceWith(1, TokenType.TokenComma);
                case ':': return this.AdvanceWith(1, TokenType.TokenColon);
                case '.': return this.AdvanceWith(1, TokenType.TokenDot);

                case '?': return this.AdvanceWith(1, TokenType.TokenQuestion);
                case '$': return this.AdvanceWith(1, TokenType.TokenDollar);

                case '+': {
                    if (next == '+')
                        return this.AdvanceWith(2, TokenType.TokenPlusPlus);
                    else if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokenPlusEquals);
                    return this.AdvanceWith(1, TokenType.TokenPlus);
                }

                case '-': {
                    if (next == '-')
                        return this.AdvanceWith(2, TokenType.TokenMinusMinus);
                    else if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokenMinusEquals);
                    return this.AdvanceWith(1, TokenType.TokenMinus);
                }

                case '=': {
                    if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokensEqualsEquals);
                    else if (next == '>')
                        return this.AdvanceWith(2, TokenType.TokenArrow);
                    return this.AdvanceWith(1, TokenType.TokenEquals);
                }

                case '*': return next == '=' ? this.AdvanceWith(2, TokenType.TokenStarEquals)
                    : this.AdvanceWith(1, TokenType.TokenStar);
                
                case '%': return next == '=' ? this.AdvanceWith(2, TokenType.TokenModuloEquals)
                    : this.AdvanceWith(1, TokenType.TokenModulo);
                
                case '!': return next == '=' ? this.AdvanceWith(2, TokenType.TokenExclamationEquals)
                    : this.AdvanceWith(1, TokenType.TokenExclamation);
                
                case '<': {
                    if (next == '<') {
                        return this.GetAt(2) == '=' ? this.AdvanceWith(3, TokenType.TokenLeftShiftEquals)
                            : this.AdvanceWith(2, TokenType.TokenLeftShift);
                    } else if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokenLessEquals);
                    return this.AdvanceWith(1, TokenType.TokenLess);
                }

                case '>': {
                    if (next == '>') {
                        return this.GetAt(2) == '=' ? this.AdvanceWith(3, TokenType.TokenRightShiftEquals)
                            : this.AdvanceWith(2, TokenType.TokenRightShift);
                    } else if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokenGreatEquals);
                    return this.AdvanceWith(1, TokenType.TokenGreat);
                }

                case '~': return this.AdvanceWith(1, TokenType.TokenTilde);
                case '^': return next == '=' ? this.AdvanceWith(2, TokenType.TokenHatEquals)
                    : this.AdvanceWith(1, TokenType.TokenHat);
                
                case '&': {
                    if (next == '&')
                        return this.AdvanceWith(2, TokenType.TokenAmpersandAmpersand);
                    else if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokenAmpersandEquals);
                    return this.AdvanceWith(1, TokenType.TokenAmpersand);
                }

                case '|': {
                    if (next == '|')
                        return this.AdvanceWith(2, TokenType.TokenPipePipe);
                    else if (next == '=')
                        return this.AdvanceWith(2, TokenType.TokenPipeEquals);
                    return this.AdvanceWith(1, TokenType.TokenPipe);
                }
            }

            this.diagnostics.Add(new Diagnostic(
                DiagnosticType.DiagError,
                new Position(this.pos.Line, this.pos.Column),
                String.Format("SyntaxError: Unexpected character '{0}'", current)
            ));
            this.Advance();
            return new Token(TokenType.TokenUndefined, null, null);
        }

        private char Get()
        {
            return this.index < this.code.Length ? this.code.ElementAt(this.index) : '\0';
        }

        private char GetAt(int offset)
        {
            int at = this.index + offset;
            return at < this.code.Length ? this.code[at] : '\0';
        }

        private void Advance()
        {
            this.index++;
            this.pos.Column++;
        }

        private Token AdvanceWith(byte size, TokenType tt)
        {
            int col = this.pos.Column;
            for (byte i=0; i<size; i++)
                this.Advance();
            
            return new Token(tt, null, new Position(this.pos.Line, col));
        }

        private void SkipSpaces()
        {
            char current = this.Get();
            while (current == ' ' || current == '\t' || (current >= '\v' && current <= '\r')) 
            {
                this.Advance();
                current = this.Get();
            }
        }

        private void SpecialSkipSpaces()
        {
            char current = this.Get();
            while ((current >= '\t' && current <= '\r') && current != ' ')
            {
                this.Advance();
                if (current == '\n')
                {
                    this.pos.Line++;
                    this.pos.Column = 1;
                }

                current = this.Get();
            }
        }

        private Token? SkipComments()
        {
            this.Advance();
            char current = this.Get();
            if (current == '*')
            {
                bool new_line = false;
                Position new_pos = new(0, 0);
                this.Advance();
                current = this.Get();

                while (current != '\0')
                {
                    if (current == '\n')
                    {
                        new_pos.Line = this.pos.Line;
                        new_pos.Column = this.pos.Column;
                        this.Advance();

                        this.pos.Column = 1;
                        this.pos.Line++;
                        current = this.Get();
                        new_line = true;
                        continue;
                    }

                    if (current == '*' && this.GetAt(1) == '/')
                    {
                        this.Advance();
                        this.Advance();
                        return new_line ? new Token(TokenType.TokenNewLine, null, new_pos) : null;
                    }

                    this.Advance();
                    current = this.Get();
                } 
            }
            else
            {
                while (current != '\n' && current != '\0')
                {
                    this.Advance();
                    current = this.Get();
                }

                int col = this.pos.Column;
                this.index++;
                this.pos.Column = 1;
                this.pos.Line++;
                return new Token(TokenType.TokenNewLine, null, new Position(this.pos.Line-1, col));
            }

            return null;
        }

        private Token LexWord()
        {
            string word = "";
            int col = this.pos.Column;
            char current = this.Get();
            while (Char.IsLetterOrDigit(current) || current == '_')
            {
                word += current;
                this.Advance();
                current = this.Get();
            }

            TokenType tt;
            if (!Globals.SEABOW_KEYWORDS.TryGetValue(word, out tt))
                tt = TokenType.TokenIdentifier;

            Position pos = new(this.pos.Line, col);
            return tt != TokenType.TokenIdentifier ? new Token(tt, null, pos) : new Token(tt, word, pos);
        }

        private Token LexNumber()
        {
            string nbr = "";
            int col = this.pos.Column;
            bool dots = false, exps = false;
            char mode = 'i';

            char first = this.Get();
            this.Advance();
            char current = this.Get();
            if (first == '0')
            {
                if (current == 'o') mode = 'o';
                else if (current == 'x') mode = 'x';
                else if (current == 'b') mode = 'b';

                if (mode != 'i')
                    this.Advance();
                else
                    nbr += first;
            } else nbr += first;

            current = this.Get();
            while (Char.IsDigit(current) || (current >= 'A' && current <= 'F') || (current >= 'a' && current <= 'f') || current == '_' || current == '.')
            {
                if (mode != 'x' && (current == 'e' || current == 'E'))
                {
                    byte err = 0;
                    if (mode != 'i' && mode != 'f') err = 1;
                    else if (exps) err = 2;

                    mode = 'f';
                    exps = true;
                    nbr += 'e';
                    if (err == 0) this.Advance();

                    current = this.Get();
                    if (err == 0 && (current != '-' && current != '+')) err = 3;

                    switch (err)
                    {
                        case 1: return this.LexIncorrectNumber(col, "SyntaxError: Hexadecimal, octal or binary number can not be a exponential number");
                        case 2: return this.LexIncorrectNumber(col, "SyntaxError: Exponential number can not have more than one 'e'");
                        case 3: return this.LexIncorrectNumber(col, "SyntaxError: Exponential number must have '+' or '-' after 'e'");
                    }

                    nbr += current;
                    this.Advance();
                    current = this.Get();
                    continue;
                }
                else if (current == '.')
                {
                    if (mode != 'i' && mode != 'f')
                        return this.LexIncorrectNumber(col, "SyntaxError: Hexadecimal, octal or binary number can not be a decimal number");
                    else if (dots)
                        return this.LexIncorrectNumber(col, "SyntaxError: Decimal number can not have more than one '.'");
                
                    mode = 'f';
                    dots = true;
                }
                else if (current == '_')
                {
                    this.Advance();
                    current = this.Get();
                    continue;
                } else if (mode == 'b' && (current != '1' && current != '0'))
                    return this.LexIncorrectNumber(col, "SyntaxError: Binary number can only have digits 0 and 1");
                else if (mode == 'o' && !(current >= '0' && current <= '7'))
                    return this.LexIncorrectNumber(col, "SyntaxError: Octal number can only have digits from 0 to 7");
                else if (mode != 'x' && ((current >= 'A' && current <= 'F') || (current >= 'a' && current <= 'f')))
                    return this.LexIncorrectNumber(col, "SyntaxError: Letters A to F can only be used with hexadecimal numbers");
        
                nbr += current;
                this.Advance();
                current = this.Get();
            }

            current = this.Get();
            if (current == 'I')
            {
                this.Advance();
                if (mode != 'i')
                {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        new Position(this.pos.Line, col),
                        "SyntaxError: Hexadecimal, octal, binary or decimal number can not have modifier 'I'"
                    ));
                    return new Token(TokenType.TokenUndefined, null, null);
                } mode = 'I';
            }
            else if (current == 'L')
            {
                this.Advance();
                if (mode != 'f' && mode != 'i')
                {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        new Position(this.pos.Line, col),
                        "SyntaxError: Hexadecimal, octal or binary number can not have modifier 'L'"
                    ));
                    return new Token(TokenType.TokenUndefined, null, null);
                } mode = 'F';
            }

            current = this.GetAt(-1);
            if (current == '-' || current == '+')
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(this.pos.Line, col),
                    "SyntaxError: Exponential number must have at least 1 digit after 'e+' or 'e-'"
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            } 
            else if (current == '.')
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(this.pos.Line, col),
                    "SyntaxError: Decimal number must have at least 1 digit after '.'"
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            }

            Position pos = new(this.pos.Line, col);
            switch (mode)
            {
                case 'o': return new Token(TokenType.TokenOctal, nbr, pos);
                case 'x': return new Token(TokenType.TokenHexa, nbr, pos);
                case 'b': return new Token(TokenType.TokenBin, nbr, pos);
                case 'f': return new Token(TokenType.TokenDecimal, nbr, pos);
                case 'I': return new Token(TokenType.TokenBigInteger, nbr, pos);
                case 'F': return new Token(TokenType.TokenBigDecimal, nbr, pos);
                case 'i': default: return new Token(TokenType.TokenInteger, nbr, pos);
            }
        }

        private Token LexIncorrectNumber(int col, string err)
        {
            char current = this.Get();
            while (Char.IsDigit(current) || (current >= 'A' && current <= 'F') || (current >= 'a' && current <= 'f') || current == '_' || current == '.')
            {
                if (current == 'e' || current == 'E')
                {
                    current = this.GetAt(1);
                    if (current == '+' || current == '-')
                        this.Advance();
                }

                this.Advance();
                current = this.Get();
            }

            this.diagnostics.Add(new Diagnostic(
                DiagnosticType.DiagError,
                new Position(this.pos.Line, col),
                err
            ));
            return new Token(TokenType.TokenUndefined, null, null);
        }

        private List<char> LexSpecialCharacters()
        {
            this.Advance();
            List<char> ret = new();
            switch (this.Get())
            {
                case 'n': ret.Add('\n'); break;
                case 't': ret.Add('\t'); break;
                case 'r': ret.Add('\r'); break;
                case 'f': ret.Add('\f'); break;
                case 'v': ret.Add('\v'); break;
                case 'b': ret.Add('\b'); break;
                case 'a': ret.Add('\a'); break;

                case 'u': case 'x': {
                    string hexa = "";
                    this.Advance();
                    char current = this.Get();
                    for (byte i=0; i<8; i++)
                    {
                        if (!(current >= '0' && current <= '9') && !(current >= 'A' && current <= 'F') && !(current >= 'a' && current <= 'f')) break;

                        hexa += current;
                        this.Advance();
                        current = this.Get();
                    }

                    uint value = Convert.ToUInt32(hexa, 16);
                    ret.Add((char)(value >> 16));
                    ret.Add((char)value);
                } return ret;

                case 'o': {
                    string oct = "";
                    this.Advance();
                    char current = this.Get();
                    byte limit = (byte)(current <= '3' ? 11 : 10);
                    for (byte i=0; i<limit; i++)
                    {
                        if (current < '0' || current > '7') break;

                        oct += current;
                        this.Advance();
                        current = this.Get();
                    }

                    uint value = Convert.ToUInt32(oct, 8);
                    ret.Add((char)(value >> 16));
                    ret.Add((char)value);
                } return ret;

                case 'B': {
                    string bin = "";
                    this.Advance();
                    char current = this.Get();
                    for (byte i=0; i<32; i++)
                    {
                        if (current != '0' && current != '1') break;

                        bin += current;
                        this.Advance();
                        current = this.Get();
                    }

                    uint value = Convert.ToUInt32(bin, 2);
                    ret.Add((char)(value >> 16));
                    ret.Add((char)value);
                } return ret;

                default: ret.Add(this.Get()); break;
            }

            this.Advance();
            return ret;
        }

        private Token LexString()
        {
            int l = this.pos.Line, col = this.pos.Column;
            this.Advance();
            this.SpecialSkipSpaces();
            string s = "";
            char current = this.Get();
            
            while (current != '"' && current != '\0')
            {
                if (current == '\\')
                {
                    foreach (char c in this.LexSpecialCharacters())
                        s += c;
                }
                else
                {
                    this.Advance();
                    s += current;
                }

                this.SpecialSkipSpaces();
                current = this.Get();
            }

            current = this.Get();
            if (current != '"')
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(l, col),
                    "SyntaxError: Unterminated string"
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            }

            this.Advance();
            return new Token(TokenType.TokenString, s, new Position(l, col));
        }

        private Token LexCharacter()
        {
            int l = this.pos.Line, col = this.pos.Column;
            this.Advance();
            string c = "";
            char current = this.Get();
            if (current == '\'')
            {
                this.Advance();
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(l, col),
                    "SyntaxError: Empty character"
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            }

            if (current == '\\')
            {
                List<char> cs = this.LexSpecialCharacters();
                if (cs.Count > 1)
                {
                    this.diagnostics.Add(new Diagnostic(
                        DiagnosticType.DiagError,
                        new Position(l, col),
                        "SyntaxError: Too many special characters for a simple character"
                    ));
                    return new Token(TokenType.TokenUndefined, null, null);
                }

                c += cs[0];
            }
            else
            {
                c += current;
                this.Advance();
            }

            current = this.Get();
            if (current != '\'')
            {
                while (current != '\'' && current != '\0')
                {
                    if (current == '\n')
                    {
                        this.index++;
                        this.pos.Column = 1;
                        this.pos.Line++;
                    } else this.Advance();
                    current = this.Get();
                }

                this.Advance();
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(l, col),
                    "SyntaxError: Unterminated character"
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            }

            this.Advance();
            return new Token(TokenType.TokenCharacter, c, new Position(l, col));
        }

        private Token LexMacro()
        {
            int col = this.pos.Column;
            this.Advance();
            this.SkipSpaces();
            string macr = "";

            char current = this.Get();
            while (Char.IsLetterOrDigit(current) || current == '_')
            {
                macr += current;
                this.Advance();
                current = this.Get();
            }

            if (macr.Length == 0)
            {
                macr = "SyntaxError: '#' is used to declare a macro and need an identifier name";
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(this.pos.Line, col),
                    macr
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            }

            return new Token(TokenType.TokenMacro, macr, new Position(this.pos.Line, col));
        }

        private Token LexFormatString()
        {
            int l = this.pos.Line, col = this.pos.Column;
            this.Advance();
            if (this.Get() != '\"')
            {
                this.diagnostics.Add(new Diagnostic(
                    DiagnosticType.DiagError,
                    new Position(l, col),
                    "SyntaxError: '@' is used to declare formatted string and need a string"
                ));
                return new Token(TokenType.TokenUndefined, null, null);
            }

            Token str = this.LexString();
            if (str.Type != TokenType.TokenString)
                return str;
            
            return new Token(TokenType.TokenFormatString, str.Content, new Position(l, col));
        }
    }
}