using core;

namespace nodes
{
    public sealed class NodeUnary: Node
    {
        public TokenType OpType{get;}
        public Node Operand{get;}

        public NodeUnary(TokenType tt, ref Node op)
        {
            this.OpType = tt;
            this.Operand = op;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeUnaryOperation;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + String.Format("-> NodeUnaryOperation(type: {0})", this.OpType));
            this.Operand.ShowDebug(indent + "  ");
        }
    }
}