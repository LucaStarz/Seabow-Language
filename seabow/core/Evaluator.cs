using nodes;
using utils;
using values;

namespace core
{
    public sealed class Element
    {
        public nuint Index{get;}
        public Value Value{get;}
        public byte Modifier{get;}

        public Element(nuint index, Value val, byte mod)
        {
            this.Index = index;
            this.Value = val;
            this.Modifier = mod;
        }
    }

    public sealed class Evaluator
    {
        private Dictionary<string, Element> elements;
        private nuint globalIndex;

        public Evaluator()
        {
            this.elements = new();
            this.globalIndex = 0;
        }

        public Value? Evaluate(NodeCompound root)
        {
            this.globalIndex = 0;
            return this.EvaluateCompound(root);
        }

        private Value? EvaluateNode(Node node)
        {
            switch (node.GetNodeType())
            {
                case NodeType.NodeCompound: return this.EvaluateCompound((node as NodeCompound)!);
            
                case NodeType.NodeLiteral: return this.EvaluateLiteral((node as NodeLiteral)!);

                case NodeType.NodeBinaryOperation: return this.EvaluateBinaryOperation((node as NodeBinary)!);

                case NodeType.NodeVariableDeclaration: return this.EvaluateVariableDeclaration((node as NodeVariableDeclaration)!);
                case NodeType.NodeConstantDeclaration: return this.EvaluateConstantDeclaration((node as NodeConstantDeclaration)!);
                case NodeType.NodeVarConstAccess: return this.EvaluateVarConstAccess((node as NodeVarConstAccess)!);

                default: {
                    Diagnostic diag = new(
                        DiagnosticType.DiagError,
                        null,
                        "RuntimeError: An invalid statement was found by the evaluator process"
                    );
                    diag.Print();
                } return null;
            }
        }

        private Value? EvaluateCompound(NodeCompound cmp)
        {
            this.globalIndex++;
            Value? ret = null;
            foreach (Node node in cmp.Nodes)
            {
                ret = this.EvaluateNode(node);
            }

            this.globalIndex--;
            return ret;
        }

        private Value? EvaluateLiteral(NodeLiteral lit)
        {
            return lit.Value;
        }

        private Value? GetAssignedValue(string? kind, Node? expression)
        {
            Value? val = null;
            if (kind != null)
                val = Value.GetDefaultValue(kind);
            
            if (expression != null)
            {
                if (val != null)
                {
                    Value? result = this.EvaluateNode(expression);
                    Value res = val.Assign(result == null ? new ValueNull() : result);
                    if (res != val)
                    {
                        Diagnostic diag = new(DiagnosticType.DiagError, null, ((res as ValueError)!.Convert(ref Globals.ConvToString) as ValueString)!.Value!);
                        diag.Print();
                        return null;
                    }
                }
                else
                    val = this.EvaluateNode(expression);
            }

            val ??= new ValueNull();
            return val;
        }

        private Value? EvaluateVariableDeclaration(NodeVariableDeclaration declaration)
        {
            if (this.elements.ContainsKey(declaration.Name))
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("RuntimeError: Name '{0}' is already used, cannot be used twice", declaration.Name));
                diag.Print();
                return null;
            }
            
            Value? val = this.GetAssignedValue(declaration.Kind, declaration.Expression);
            if (val == null)
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, "RuntimeError: Cannot create the variable due to previous error");
                diag.Print();
                return null;
            }

            this.elements.Add(declaration.Name, new Element(this.globalIndex, val, Globals.MODIFIER_NONE));
            return null;
        }

        private Value? EvaluateConstantDeclaration(NodeConstantDeclaration declaration)
        {
            if (this.elements.ContainsKey(declaration.Name))
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("RuntimeError: Name '{0}' is already used, cannot be used twice", declaration.Name));
                diag.Print();
                return null;
            }
            
            Value? val = this.GetAssignedValue(declaration.Kind, declaration.Expression);
            if (val == null)
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, "RuntimeError: Cannot create the variable due to previous error");
                diag.Print();
                return null;
            }

            this.elements.Add(declaration.Name, new Element(this.globalIndex, val, Globals.MODIFIER_CONSTANT));
            return null;
        }

        private Value? EvaluateVarConstAccess(NodeVarConstAccess access)
        {
            if (this.elements.ContainsKey(access.Name))
                return this.elements[access.Name].Value;
            else
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("RuntimeError: Variable or constant '{0}' does not exists", access.Name));
                diag.Print();
                return null;
            }
        }

        private Value? EvaluateBinaryOperation(NodeBinary binary)
        {
            switch (binary.OpType)
            {

                default: {
                    Diagnostic diag = new(DiagnosticType.DiagError, null, "RuntimeError: An undefined binary operation occured");
                    diag.Print();
                    return null;
                }
            }
        }
    }
}