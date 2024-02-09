using nodes;
using utils;
using values;

namespace core
{
    public enum ElementModifier: byte {
        ModifierLoopControl, ModifierDiagnostic, ModifierNotPrint,

        ModifierVariable, ModifierConstant, ModifierUnassignedConstant
    }

    public sealed class Element
    {
        public nuint Index{get;}
        public Value Value{get;}
        public ElementModifier[] Modifiers{get; set;}

        public Element(nuint index, Value val, ref ElementModifier[] mods)
        {
            this.Index = index;
            this.Value = val;
            this.Modifiers = mods;
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
            Element? result = this.EvaluateCompound(root);
            return result == null ? null : (
                result.Modifiers.Contains(ElementModifier.ModifierNotPrint) ? null : result.Value
            );
        }

        private Element? EvaluateNode(Node node)
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

        private Element? EvaluateCompound(NodeCompound cmp)
        {
            this.globalIndex++;
            Element? ret = null;
            foreach (Node node in cmp.Nodes)
            {
                ret = this.EvaluateNode(node);
            }

            this.globalIndex--;
            return ret;
        }

        private Element? EvaluateLiteral(NodeLiteral lit)
        {
            return new Element(this.globalIndex, lit.Value, ref Globals.EMPTY_MODIFIERS);
        }

        private Element? GetAssignValue(string? kind, Node? expression)
        {
            Element? elt = null;
            if (kind != null)
            {
                Value? val = Value.GetDefaultValue(kind);
                if (val != null)
                {
                    ElementModifier[] mods = { ElementModifier.ModifierUnassignedConstant };
                    elt = new(0, val, ref mods);
                }
            }

            if (expression != null)
            {
                if (elt != null)
                {
                    Element? result = this.EvaluateNode(expression);
                    if (result == null)
                        return null;
                    
                    Element res = elt.Value.Assign(result.Value);
                    if (res.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                    {
                        ValueError err = (res.Value as ValueError)!;
                        Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("{0}: {1}", err.Name, err.Details));
                        diag.Print();
                        return null;
                    }
                }
                else
                {
                    elt = this.EvaluateNode(expression);
                    if (elt == null)
                        return null;
                }
                elt.Modifiers = Globals.EMPTY_MODIFIERS;
            }

            return elt;
        }

        private Element? EvaluateVariableDeclaration(NodeVariableDeclaration declaration)
        {
            if (this.elements.ContainsKey(declaration.Name))
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("RuntimeError: Name '{0}' is already used, cannot be used twice", declaration.Name));
                diag.Print();
                return null;
            }
            
            Element? val = this.GetAssignValue(declaration.Kind, declaration.Expression);
            if (val == null)
                return null;

            ElementModifier[] mods = { ElementModifier.ModifierVariable };
            this.elements.Add(declaration.Name, new Element(this.globalIndex, val.Value, ref mods));
            return null;
        }

        private Element? EvaluateConstantDeclaration(NodeConstantDeclaration declaration)
        {
            if (this.elements.ContainsKey(declaration.Name))
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("RuntimeError: Name '{0}' is already used, cannot be used twice", declaration.Name));
                diag.Print();
                return null;
            }
            
            Element? val = this.GetAssignValue(declaration.Kind, declaration.Expression);
            if (val == null)
                return null;

            ElementModifier[] mods = { ElementModifier.ModifierConstant };
            if (val.Modifiers.Contains(ElementModifier.ModifierUnassignedConstant))
                mods[0] = ElementModifier.ModifierUnassignedConstant;

            this.elements.Add(declaration.Name, new Element(this.globalIndex, val.Value, ref mods));
            return null;
        }

        private Element? EvaluateVarConstAccess(NodeVarConstAccess access)
        {
            if (this.elements.ContainsKey(access.Name))
                return this.elements[access.Name];
            else
            {
                Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("RuntimeError: Variable or constant '{0}' does not exists", access.Name));
                diag.Print();
                return null;
            }
        }

        private Element? EvaluateBinaryOperation(NodeBinary binary)
        {
            switch (binary.OpType)
            {
                case TokenType.TokenEquals: {
                    Element? left = this.EvaluateNode(binary.Left);
                    if (left == null)
                        return null;

                    if (left.Modifiers.Contains(ElementModifier.ModifierConstant))
                    {
                        Diagnostic diag = new(DiagnosticType.DiagError, null, "AssignError: Can not assign a constant to another value");
                        diag.Print();
                        return null;
                    }

                    if (!left.Modifiers.Contains(ElementModifier.ModifierVariable) && !left.Modifiers.Contains(ElementModifier.ModifierUnassignedConstant))
                    {
                        Diagnostic diag = new(DiagnosticType.DiagError, null, "AssignError: Can not assign a value to a literal value");
                        diag.Print();
                        return null;
                    }

                    Element? right = this.EvaluateNode(binary.Right!);
                    if (right == null)
                        return null;

                    Element elt = left.Value.Assign(right.Value);
                    if (elt.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                    {
                        ValueError err = (elt.Value as ValueError)!;
                        Diagnostic diag = new(DiagnosticType.DiagError, null, String.Format("{0}: {1}", err.Name, err.Details));
                        diag.Print();
                        return null;
                    }

                    if (left.Modifiers.Contains(ElementModifier.ModifierUnassignedConstant))
                        left.Modifiers[Array.IndexOf(left.Modifiers, ElementModifier.ModifierUnassignedConstant)] = ElementModifier.ModifierConstant;

                    ElementModifier[] mods = { ElementModifier.ModifierNotPrint };
                    Element ret = new(0, elt.Value, ref mods);
                    return ret;
                }

                default: {
                    Diagnostic diag = new(DiagnosticType.DiagError, null, "RuntimeError: An undefined binary operation occured");
                    diag.Print();
                    return null;
                }
            }
        }
    }
}