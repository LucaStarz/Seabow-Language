namespace nodes
{
    public sealed class NodeVarConstAccess: Node
    {
        public string Name{get;}

        public NodeVarConstAccess(string name)
        {
            this.Name = name;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeVarConstAccess;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + String.Format("-> NodeVarConstAccess(name: {0})", this.Name));
        }
    }
}