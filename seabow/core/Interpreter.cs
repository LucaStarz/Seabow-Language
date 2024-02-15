using nodes;
using utils;
using values;

namespace core
{
    public sealed class Interpreter
    {

        public void Interpret()
        {
            Console.WriteLine(String.Format("seabow {0}.{1}.{2}", Globals.SEABOW_MAJOR, Globals.SEABOW_MINOR, Globals.SEABOW_PATCH));
            Evaluator evaluator = new();

            string? code = "RUN";
            while (code != null && code.Length > 0)
            {
                Console.Write(">>> ");
                code = Console.ReadLine();

                if (code != null && code.Length > 0)
                {
                    List<Diagnostic> diags = new();
                    Lexer lex = new(ref code, ref diags);
                    List<Token> tokens = lex.Lex();
                    Parser parser = new(ref tokens, ref diags);
                    NodeCompound root = parser.Parse();

                    if (diags.Count > 0)
                    {
                        foreach (Diagnostic diag in diags)
                            diag.Print();
                    }
                    else
                    {
                        Value result = evaluator.Evaluate(root);
                        if (result.GetValueKind() != ValueKind.ValueNull)
                            Console.WriteLine(result);
                    }
                }
            }
        }
    }
}