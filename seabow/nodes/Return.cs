using System.Data.Common;

namespace nodes
{
    public sealed class NodeReturn: Node
    {
        public Node? Expression{get;}

        public NodeReturn(ref Node? expr)
        {
            this.Expression = expr;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeReturn;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + "-> NodeReturn");
            this.Expression?.ShowDebug(indent + "  ");
        }
    }
}