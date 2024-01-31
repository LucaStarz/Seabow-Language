namespace nodes
{
    public sealed class NodeCompound: Node
    {
        public List<Node> Nodes{get;}

        public override NodeType GetNodeType()
        {
            return NodeType.NodeCompound;
        }

        public NodeCompound(ref List<Node> nodes)
        {
            this.Nodes = nodes;
        }

        public override void ShowDebug(string indent="")
        {
            if (indent.Length > 0)
                Console.WriteLine(indent + "-> NodeCompound");
            else
                Console.WriteLine("ROOT");

            foreach (Node node in this.Nodes)
                node.ShowDebug(indent + "  ");
        }
    }
}