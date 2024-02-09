using utils;

namespace core
{
    public enum TokenType: byte
    {
        TokenUndefined, TokenEof, TokenNewLine,

        TokenVar, TokenConst, TokenFunc, TokenLambda, TokenStruct, TokenEnum,
        TokenClass, TokenConstructor, TokenInterface,

        TokenAbstract, TokenVirtual, TokenSealed, TokenOperator, TokenStatic, TokenOverride,
        TokenPrivate, TokenProtected, TokenPublic, TokenReadonly,
        TokenGlobal, TokenUndel,

        TokenBreak, TokenContinue, TokenReturn,

        TokenIf, TokenElif, TokenElse, TokenSwitch, TokenCase, TokenDefault,
        TokenFor, TokenForeach, TokenWhile, TokenDoWhile,
        TokenTry, TokenCatch, TokenFinally,
        TokenImport, TokenInclude, TokenAs,
        TokenExit, TokenThrow, TokenDelete, TokenThis,

        TokenIdentifier, TokenFalse, TokenTrue, TokenNull, TokenInteger, TokenDecimal,
        TokenBigInteger, TokenBigDecimal, TokenOctal, TokenHexa, TokenBin,
        TokenString, TokenCharacter, TokenMacro, TokenFormatString,

        TokenPlus, TokenPlusEquals, TokenPlusPlus, TokenMinus, TokenMinusEquals, TokenMinusMinus,
        TokenStar, TokenStarEquals, TokenSlash, TokenSlashEquals, TokenModulo, TokenModuloEquals,
        TokenEquals, TokensEqualsEquals, TokenExclamation, TokenExclamationEquals,
        TokenGreat, TokenGreatEquals, TokenLess, TokenLessEquals,
        TokenLeftShift, TokenLeftShiftEquals, TokenRightShift, TokenRightShiftEquals,
        TokenTilde, TokenHat, TokenHatEquals, TokenAmpersand, TokenAmpersandEquals,
        TokenAmpersandAmpersand, TokenPipe, TokenPipeEquals, TokenPipePipe, TokenIn, TokenIs, TokenDollar,
        
        TokenLeftParenthesis, TokenRightParenthesis, TokenLeftBrace, TokenRightBrace,
        TokenLeftBracket, TokenRightBracket, TokenSemi, TokenColon, TokenDot,
        TokenQuestion, TokenComma, TokenArrow
    }

    public sealed class Token
    {
        public TokenType Type{get;}
        public string? Content{get;}
        public Position? Position{get;}

        public Token(TokenType tt, string? cnt, Position? pos)
        {
            this.Type = tt;
            this.Content = cnt;
            this.Position = pos;
        }

        public override string ToString()
        {
            return String.Format("Token(type: {0}, content: {1}, pos: {2})", this.Type, this.Content, this.Position);
        }

        public static byte GetUnaryPrecedence(TokenType tt)
        {
            switch (tt)
            {
                case TokenType.TokenDollar: return 18;
                case TokenType.TokenPlusPlus: case TokenType.TokenMinusMinus: return 16;
                case TokenType.TokenExclamation: case TokenType.TokenTilde: case TokenType.TokenPlus: case TokenType.TokenMinus: return 15;
                case TokenType.TokenQuestion: return 2;

                default: return 0;
            }
        }

        public static byte GetBinaryPrecedence(TokenType tt)
        {
            switch (tt)
            {
                case TokenType.TokenPlusPlus: case TokenType.TokenMinusMinus: return 17;
                case TokenType.TokenStar: case TokenType.TokenSlash: case TokenType.TokenModulo: return 13;
                case TokenType.TokenPlus: case TokenType.TokenMinus: return 12;
                case TokenType.TokenLeftShift: case TokenType.TokenRightShift: return 11;
                case TokenType.TokenIn: case TokenType.TokenIs: return 10;
                case TokenType.TokenLess: case TokenType.TokenLessEquals: case TokenType.TokenGreat: case TokenType.TokenGreatEquals: return 9;
                case TokenType.TokenEquals: case TokenType.TokenExclamationEquals: return 8;
                case TokenType.TokenAmpersand: return 7;
                case TokenType.TokenHat: return 6;
                case TokenType.TokenPipe: return 5;
                case TokenType.TokenAmpersandAmpersand: return 4;
                case TokenType.TokenPipePipe: return 3;

                case TokenType.TokensEqualsEquals: case TokenType.TokenPlusEquals: case TokenType.TokenMinusEquals: case TokenType.TokenStarEquals:
                case TokenType.TokenSlashEquals: case TokenType.TokenModuloEquals: case TokenType.TokenLeftShiftEquals:
                case TokenType.TokenRightShiftEquals: case TokenType.TokenPipeEquals: case TokenType.TokenAmpersandEquals:
                    return 1;

                default: return 0;
            }
        }
    }
}