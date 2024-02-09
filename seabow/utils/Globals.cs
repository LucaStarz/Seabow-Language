using core;
using values;

namespace utils
{
    public static class Globals
    {
        public static readonly Dictionary<string, TokenType> SEABOW_KEYWORDS = new()
        {
            {"var", TokenType.TokenVar},
            {"const", TokenType.TokenConst},
            {"func", TokenType.TokenFunc},
            {"lambda", TokenType.TokenLambda},
            {"struct", TokenType.TokenStruct},
            {"enum", TokenType.TokenEnum},
            {"class", TokenType.TokenClass},
            {"constructor", TokenType.TokenConstructor},
            {"interface", TokenType.TokenInterface},

            {"abstract", TokenType.TokenAbstract},
            {"virtual", TokenType.TokenVirtual},
            {"sealed", TokenType.TokenSealed},
            {"operator", TokenType.TokenOperator},
            {"static", TokenType.TokenStatic},
            {"override", TokenType.TokenOverride},
            {"private", TokenType.TokenPrivate},
            {"protected", TokenType.TokenProtected},
            {"public", TokenType.TokenPublic},
            {"readonly", TokenType.TokenReadonly},
            {"global", TokenType.TokenGlobal},
            {"undel", TokenType.TokenUndel},

            {"break", TokenType.TokenBreak},
            {"continue", TokenType.TokenContinue},
            {"return", TokenType.TokenReturn},

            {"if", TokenType.TokenIf},
            {"elif", TokenType.TokenElif},
            {"else", TokenType.TokenElse},
            {"switch", TokenType.TokenSwitch},
            {"case", TokenType.TokenCase},
            {"default", TokenType.TokenDefault},
            {"for", TokenType.TokenFor},
            {"foreach", TokenType.TokenForeach},
            {"while", TokenType.TokenWhile},
            {"do", TokenType.TokenDoWhile},
            {"try", TokenType.TokenTry},
            {"catch", TokenType.TokenCatch},
            {"finally", TokenType.TokenFinally},
            {"import", TokenType.TokenImport},
            {"include", TokenType.TokenInclude},
            {"as", TokenType.TokenAs},
            {"exit", TokenType.TokenExit},
            {"throw", TokenType.TokenThrow},
            {"delete", TokenType.TokenDelete},
            {"this", TokenType.TokenThis},

            {"false", TokenType.TokenFalse},
            {"true", TokenType.TokenTrue},
            {"null", TokenType.TokenNull},
            {"in", TokenType.TokenIn},
            {"is", TokenType.TokenIs}
        };

        public static values.ValueType ToType = new(ValueKind.ValueType, null);
        public static values.ValueType ConvToString = new(ValueKind.ValueString, null);
    
        public static uint SEABOW_MAJOR = 0;
        public static uint SEABOW_MINOR = 1;
        public static uint SEABOW_PATCH = 0;

        public static string HELPS = "seabow is a tool for managing seabow files (.sbw, .sbb, .sbl)\n\nUsage:\n\tseabow <command> [arguments]\n\nList of seabow commands:\n\t<none>\tstart seabow interpreter\n\tint\tstart seabow interpreter or interpret seabow source code if provided\n\tcmp\tcompile seabow source code to bytecode file (.sbw -> .sbb)\n\tbuild\tcompile seabow source code to a native executable\n\trun\tinterpret a seabow bytecode file\n\tlib\tcompile seabow source code into seabow bytecode library (.sbw -> .sbl)\n\thelp\tshow helps for seabow or for a specified library if given\n\tlist\tlist all installed seabow libraries";
    
        public static ElementModifier[] EMPTY_MODIFIERS = Array.Empty<ElementModifier>();
        public static ElementModifier[] DIAG_MODIFIERS = { ElementModifier.ModifierDiagnostic };
    }
}