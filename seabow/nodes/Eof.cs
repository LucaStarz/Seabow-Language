namespace nodes
{
    public sealed class NodeUndefined: Node
    {
        public NodeUndefined() {}

        public override NodeType GetNodeType()
        {
            return NodeType.NodeUndefined;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + "-> NodeEndOfFile");
        }
    }
}