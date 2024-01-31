namespace nodes
{
    public sealed class NodeParenthesized : Node
    {
        public Node Expression{get;}

        public NodeParenthesized(Node expr)
        {
            this.Expression = expr;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeParenthesized;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + "-> NodeParenthesized");
            this.Expression.ShowDebug(indent + "  ");
        }
    }
}