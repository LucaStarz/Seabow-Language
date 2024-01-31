using values;

namespace nodes
{
    public sealed class NodeVariableDeclaration: Node
    {
        public string Name{get;}
        public string? Kind{get;}
        public Node? Expression{get;}

        public NodeVariableDeclaration(string name, string? kind, Node? expr)
        {
            this.Name = name;
            this.Kind = kind;
            this.Expression = expr;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeVariableDeclaration;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + String.Format("-> NodeVariableDeclaration(type: {0})", this.Kind != null ? this.Kind : "<unknown>"));
            this.Expression?.ShowDebug(indent + "  ");
        }
    }
}