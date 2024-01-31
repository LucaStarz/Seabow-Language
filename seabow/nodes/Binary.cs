using core;

namespace nodes
{
    public sealed class NodeBinary: Node
    {
        public TokenType OpType{get;}
        public Node Left{get;}
        public Node? Right{get;}

        public NodeBinary(TokenType tt, ref Node l, ref Node r)
        {
            this.OpType = tt;
            this.Left = l;
            this.Right = r;
        }

        public NodeBinary(TokenType tt, ref Node l)
        {
            this.OpType = tt;
            this.Left = l;
            this.Right = null;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeBinaryOperation;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + String.Format("-> NodeBinaryOperation(type: {0})", this.OpType));
            this.Left.ShowDebug(indent + "  ");
            this.Right?.ShowDebug(indent + "  ");
        }
    }
}