namespace nodes
{
    public sealed class NodeNoOperation: Node
    {
        public NodeNoOperation() {}

        public override NodeType GetNodeType()
        {
            return NodeType.NodeNoOperation;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + "-> NodeNoOperation");
        }
    }
}