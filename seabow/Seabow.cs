using core;
using nodes;
using utils;

class Seabow
{
    static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        
        List<Command> commands = CommandHandler.Handle(args);
        switch (commands.First().Name)
        {
            case CommandType.Interpreter: {
                Interpreter interp = new();
                interp.Interpret();
            } break;

            case CommandType.Help: {
                Console.WriteLine(Globals.HELPS);
            } break;

            case CommandType.ListLibraries: {
                if (Directory.Exists("/libs"))
                {
                    Console.WriteLine("installed seabow libraries:");
                    foreach (string dir in Directory.GetDirectories("/libs"))
                        Console.WriteLine("\t- " + dir);
                }
                else
                    Console.WriteLine("'libs' directory not find in the seabow folder");
            } break;

            case CommandType.UnknownCommand: {
                Diagnostic diag = new Diagnostic(DiagnosticType.DiagError, null, String.Format("SeabowError: Use of unknown seabow command '{0}'", commands.First().Value));
                diag.Print();
            } break;

            default: {
                string code = File.ReadAllText("test/main.sbw");
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
                    foreach (Token tok in tokens)
                        Console.WriteLine(tok);
                    Console.WriteLine("\n");
                    root.ShowDebug();
                }
            } break;
        }
    }
}