namespace nodes
{
    public sealed class NodeContinue: Node
    {
        public NodeContinue() {}

        public override NodeType GetNodeType()
        {
            return NodeType.NodeContinue;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + "-> NodeContinue");
        }
    }
}