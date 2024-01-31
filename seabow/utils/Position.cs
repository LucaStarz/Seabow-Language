namespace utils
{
    public class Position
    {
        public int Line {get; set;}
        public int Column {get; set;}

        public Position(int l, int c)
        {
            this.Line = l;
            this.Column = c;
        }

        public override string ToString()
        {
            return String.Format("Position(line: {0}, column: {1})", this.Line, this.Column);
        }
    }
}