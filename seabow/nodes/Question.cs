namespace nodes
{
    public sealed class NodeQuestion: Node
    {
        public Node Condition{get;}
        public Node First{get;}
        public Node Second{get;}

        public NodeQuestion(ref Node cnd, ref Node f, ref Node s)
        {
            this.Condition = cnd;
            this.First = f;
            this.Second = s;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.NodeQuestionOperation;
        }

        public override void ShowDebug(string indent = "")
        {
            Console.WriteLine(indent + "-> NodeQuestionOperation");
            this.Condition.ShowDebug(indent + "  ");
            this.First.ShowDebug(indent + "  ");
            this.Second.ShowDebug(indent + "  ");
        }
    }
}