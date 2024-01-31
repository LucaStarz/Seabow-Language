namespace nodes
{
    public sealed class NodeAttributeAccess: Node
    {
        public string SuperName{get;}
        public string AttributeName{get;}

        public NodeAttributeAccess(string sup, string att)
        {
            this.SuperName = sup;
            this.AttributeName = att;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeAttributeAccess;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + String.Format("-> NodeAttributeAccess(super: {0}, attr: {1})", this.SuperName, this.AttributeName));
        }
    }
}