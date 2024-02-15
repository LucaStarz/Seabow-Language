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

        public Value Evaluate(NodeCompound root)
        {
            this.globalIndex = 0;
            Element result = this.EvaluateCompound(root);
            if (result.Modifiers.Contains(ElementModifier.ModifierDiagnostic)) {
                Diagnostic.CreateErrorAndPrint((result.Value as ValueError)!);
                return new ValueNull();
            }

            return result.Value;
        }

        private ref Element AddContextAndReturn(ref Element elt, string ctxt)
        {
            (elt.Value as ValueError)!.AddContext(ctxt);
            return ref elt;
        }

        private Element EvaluateNode(Node node)
        {
            switch (node.GetNodeType())
            {
                case NodeType.NodeCompound: return this.EvaluateCompound((node as NodeCompound)!);
            
                case NodeType.NodeLiteral: return this.EvaluateLiteral((node as NodeLiteral)!);

                case NodeType.NodeUnaryOperation: return this.EvaluateUnaryOperation((node as NodeUnary)!);
                case NodeType.NodeBinaryOperation: return this.EvaluateBinaryOperation((node as NodeBinary)!);

                case NodeType.NodeVariableDeclaration: return this.EvaluateVariableDeclaration((node as NodeVariableDeclaration)!);
                case NodeType.NodeConstantDeclaration: return this.EvaluateConstantDeclaration((node as NodeConstantDeclaration)!);
                case NodeType.NodeVarConstAccess: return this.EvaluateVarConstAccess((node as NodeVarConstAccess)!);

                default: return new Element(0, new ValueError("RuntimeError", "An invalid statement was found by the evaluator process"), ref Globals.DIAG_MODIFIERS);
            }
        }

        private Element EvaluateCompound(NodeCompound cmp)
        {
            this.globalIndex++;
            Element? ret = null;
            foreach (Node node in cmp.Nodes)
                ret = this.EvaluateNode(node);

            this.globalIndex--;
            return ret ?? new Element(0, new ValueNull(), ref Globals.EMPTY_MODIFIERS);
        }

        private Element EvaluateLiteral(NodeLiteral lit)
        {
            return new Element(this.globalIndex, lit.Value, ref Globals.EMPTY_MODIFIERS);
        }

        private Element GetAssignValue(string? kind, Node? expression)
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
                    if (result.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return result;
                    
                    Element res = elt.Value.Assign(result.Value);
                    if (res.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return res;
                }
                else
                {
                    elt = this.EvaluateNode(expression);
                    if (elt.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return elt;
                }
                elt.Modifiers = Globals.EMPTY_MODIFIERS;
            }

            return elt!;
        }

        private Element EvaluateVariableDeclaration(NodeVariableDeclaration declaration)
        {
            if (this.elements.ContainsKey(declaration.Name))
            {
                Element elt = new Element(0, new ValueError("DeclarationError", String.Format("Name '{0}' is already used, cannot be used twice", declaration.Name)), ref Globals.DIAG_MODIFIERS);
                return this.AddContextAndReturn(ref elt, String.Format("In var declaration '{0}':", declaration.Name));
            }
            
            Element val = this.GetAssignValue(declaration.Kind, declaration.Expression);
            if (val.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                return this.AddContextAndReturn(ref val, String.Format("In var declaration '{0}':", declaration.Name));

            ElementModifier[] mods = { ElementModifier.ModifierVariable };
            this.elements.Add(declaration.Name, new Element(this.globalIndex, val.Value, ref mods));
            return new Element(0, new ValueNull(), ref Globals.EMPTY_MODIFIERS);
        }

        private Element EvaluateConstantDeclaration(NodeConstantDeclaration declaration)
        {
            if (this.elements.ContainsKey(declaration.Name))
            {
                Element elt = new Element(0, new ValueError("DeclarationError", String.Format("Name '{0}' is already used, cannot be used twice", declaration.Name)), ref Globals.DIAG_MODIFIERS);
                return this.AddContextAndReturn(ref elt, String.Format("In const declaration '{0}':", declaration.Name));
            }
            
            Element val = this.GetAssignValue(declaration.Kind, declaration.Expression);
            if (val.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                return this.AddContextAndReturn(ref val, String.Format("In const declaration '{0}':", declaration.Name));

            ElementModifier[] mods = { ElementModifier.ModifierConstant };
            if (val.Modifiers.Contains(ElementModifier.ModifierUnassignedConstant))
                mods[0] = ElementModifier.ModifierUnassignedConstant;

            this.elements.Add(declaration.Name, new Element(this.globalIndex, val.Value, ref mods));
            return new Element(0, new ValueNull(), ref Globals.EMPTY_MODIFIERS);
        }

        private Element EvaluateVarConstAccess(NodeVarConstAccess access)
        {
            if (this.elements.ContainsKey(access.Name))
                return this.elements[access.Name];
            else
                return new Element(0, new ValueError("AccessError", String.Format("Variable or constant '{0}' does not exists", access.Name)), ref Globals.DIAG_MODIFIERS);
        }

        private Element EvaluateUnaryOperation(NodeUnary unary)
        {
            switch (unary.OpType)
            {
                case TokenType.TokenMinus: {
                    Element elt = this.EvaluateNode(unary.Operand);
                    if (elt.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return elt;
                    
                    Element result = elt.Value.LeftMinus();
                    if (result.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return result;

                    return result;
                }

                default: return new Element(0, new ValueError("OperationError", "An undefined unary operation occured"), ref Globals.DIAG_MODIFIERS);
            }
        }

        private Element EvaluateBinaryOperation(NodeBinary binary)
        {
            switch (binary.OpType)
            {
                case TokenType.TokenEquals: {
                    Element left = this.EvaluateNode(binary.Left)!;
                    if (left.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return left;

                    if (left.Modifiers.Contains(ElementModifier.ModifierConstant))
                        return new Element(0, new ValueError("OperationError", "Can not assign a constant to another value"), ref Globals.DIAG_MODIFIERS);

                    if (!left.Modifiers.Contains(ElementModifier.ModifierVariable) && !left.Modifiers.Contains(ElementModifier.ModifierUnassignedConstant))
                        return new Element(0, new ValueError("OperationError", "Can not assign a value to a literal value"), ref Globals.DIAG_MODIFIERS);

                    Element right = this.EvaluateNode(binary.Right!);
                    if (right.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return right;

                    Element elt = left.Value.Assign(right.Value);
                    if (elt.Modifiers.Contains(ElementModifier.ModifierDiagnostic))
                        return elt;

                    if (left.Modifiers.Contains(ElementModifier.ModifierUnassignedConstant))
                        left.Modifiers[Array.IndexOf(left.Modifiers, ElementModifier.ModifierUnassignedConstant)] = ElementModifier.ModifierConstant;

                    ElementModifier[] mods = { ElementModifier.ModifierNotPrint };
                    Element ret = new(0, elt.Value, ref mods);
                    return ret;
                }

                default: return new Element(0, new ValueError("OperationError", "An undifined binary operation occured"), ref Globals.DIAG_MODIFIERS);
            }
        }
    }
}