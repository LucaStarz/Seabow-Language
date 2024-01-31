namespace nodes
{
    public enum NodeType: byte
    {
        NodeUndefined, NodeNoOperation,

        NodeCompound,

        NodeReturn, NodeBreak, NodeContinue,

        NodeConvert, NodeParenthesized, NodeLiteral, NodeFormattedString,

        NodeUnaryOperation, NodeBinaryOperation, NodeQuestionOperation,

        NodeVariableDeclaration, NodeConstantDeclaration, NodeFunctionDeclaration,

        NodeVarConstAccess, NodeFunctionCall, NodeAttributeAccess,

        NodeModifier
    }

    public abstract class Node
    {
        public abstract NodeType GetNodeType();
        public abstract void ShowDebug(string indent="");
    }
}