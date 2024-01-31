namespace nodes
{
    public sealed class NodeBreak: Node
    {
        public NodeBreak() {}

        public override NodeType GetNodeType()
        {
            return NodeType.NodeBreak;
        }

        public override void ShowDebug(string indent="")
        {
            Console.WriteLine(indent + "-> NodeBreak");
        }
    }
}