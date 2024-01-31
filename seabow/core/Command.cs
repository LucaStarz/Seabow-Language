namespace core
{
    public enum CommandType: byte
    {
        UnknownCommand,

        Interpreter, Compiler, NativeCompiler, BytecodeInterpreter,
        LibraryCompiler,

        Help, ListLibraries,

        InputFile, OutputFile
    }

    public sealed class Command
    {
        public CommandType Name{get;}
        public string? Value{get;}

        public Command(CommandType name, string? val)
        {
            this.Name = name;
            this.Value = val;
        }
    }

    public static class CommandHandler
    {
        public static List<Command> Handle(string[] args)
        {
            List<Command> commands = new();

            if (args.Length < 1)
                commands.Add(new Command(CommandType.Interpreter, null));
            else
            {
                switch (args[0])
                {
                    case "int": commands.Add(new Command(CommandType.Interpreter, null)); break;
                    case "cmp": commands.Add(new Command(CommandType.Compiler, null)); break;
                    case "build": commands.Add(new Command(CommandType.NativeCompiler, null)); break;
                    case "run": commands.Add(new Command(CommandType.BytecodeInterpreter, null)); break;
                    case "lib": commands.Add(new Command(CommandType.LibraryCompiler, null)); break;

                    case "help": case "-h": case "--help": commands.Add(new Command(CommandType.Help, null)); break;
                    case "list": case "-l": commands.Add(new Command(CommandType.ListLibraries, null)); break;
                
                    default: commands.Add(new Command(CommandType.UnknownCommand, args[0])); break;
                }
            }

            for (int i=1; i<args.Length; i++)
            {

            }

            return commands;
        }
    }
}