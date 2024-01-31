using values;

namespace nodes
{
    public sealed class NodeLiteral: Node
    {
        public Value Value{get;}

        public NodeLiteral(Value val)
        {
            this.Value = val;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeLiteral;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + String.Format("-> NodeLiteral(value: {0})", this.Value));
        }
    }
}