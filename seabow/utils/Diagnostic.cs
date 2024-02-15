using values;

namespace utils
{
    public enum DiagnosticType: byte
    {
        DiagInfo, DiagWarning, DiagError, DiagSuccess
    }

    public class Diagnostic
    {
        public DiagnosticType Type{get;}
        public Position? Position{get;}
        public string Message{get;}

        public Diagnostic(DiagnosticType dt, Position? pos, string msg)
        {
            this.Type = dt;
            this.Position = pos;
            this.Message = msg;
        }

        public void Print()
        {
            switch (this.Type)
            {
                case DiagnosticType.DiagInfo: break;
                case DiagnosticType.DiagWarning: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case DiagnosticType.DiagError: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case DiagnosticType.DiagSuccess: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
            }

            Console.WriteLine(this.Position != null ? String.Format("[At line {0}, column {1}]: {2}", this.Position.Line, this.Position.Column, this.Message)
                : this.Message);
            Console.ResetColor();
        }

        public static void CreateAndPrint(DiagnosticType dt, string msg)
        {
            Diagnostic diag = new Diagnostic(dt, null, msg);
            diag.Print();
        }

        public static void CreateErrorAndPrint(ValueError err)
        {
            string prt = "";
            if (err.Context != null)
            {
                err.Context.Reverse();
                foreach (string ctxt in err.Context)
                    prt += ctxt + '\n';
            }

            Diagnostic diag = new Diagnostic(DiagnosticType.DiagError, null, prt + String.Format("{0}: {1}", err.Name, err.Details));
            diag.Print();
        }
    }
}