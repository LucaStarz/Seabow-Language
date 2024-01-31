using utils;

namespace values
{
    public enum ValueKind: byte
    {
        ValueUndefined, ValueNull, ValueReference, ValueNotNull, ValueAny,
        
        ValueByte, ValueUbyte, ValueShort, ValueUshort, ValueInt, ValueUint, ValueLong, ValueUlong,
        ValueInt128, ValueUint128, ValueHalf, ValueFloat, ValueDouble, ValueLdouble,
        ValueIsize, ValueUsize, ValueBool,
        ValueChar, ValueString, ValueType, ValueError, ValueArray, ValueList, ValueDict,

        ValueStruct, ValueEnum, ValueClass
    }

    public abstract class Value
    {
        public static string? ValueKindToString(ValueKind kind)
        {
            switch (kind)
            {
                case ValueKind.ValueNull: return "<null>";
                case ValueKind.ValueAny: return "<any>";
                case ValueKind.ValueUlong: return "<ulong>";
                case ValueKind.ValueDouble: return "<double>";
                case ValueKind.ValueBool: return "<bool>";
                case ValueKind.ValueChar: return "<char>";
                case ValueKind.ValueString: return "<string>";
                case ValueKind.ValueType: return "<type>";
                case ValueKind.ValueError: return "<error>";

                default: return null;
            }
        }

        public static ValueError UnaryOpError(ref ValueType t, string op)
        {
            return new("OperatorError", String.Format("Unary operator '{0}' is not defined for type {1}", op, t));
        }

        public static ValueError BinaryOpError(ref ValueType t1, ref ValueType t2, string op)
        {
            return new("OperatorError", String.Format("Binary operator '{0}' is not defined for types {1} and {2}", op, t1, t2));
        }

        public static ValueError OpWithNullError(string op)
        {
            return new("OperatorError", String.Format("Operator '{0}' can not be used with null value", op));
        }

        public static ValueError ConvertionError(ref ValueType from, ref ValueType to)
        {
            return new("ConvertionError", String.Format("Can not convert from type {0} to type {1}", from, to));
        }

        public static ValueError AutoConvertionError(ref ValueType from, ref ValueType to)
        {
            return new("ConvertionError", String.Format("Can not automatically convert from type {0} to type {1}", from, to));
        }

        public static ValueError ZeroDivError()
        {
            return new("ZeroDivisionError", "Can not divide by zero");
        }

        public static Value? GetDefaultValue(string kind)
        {
            switch (kind)
            {
                case "ulong": return new ValueUlong(null);
                case "double": return new ValueDouble(null);
                case "bool": return new ValueBool(null);
                case "char": return new ValueCharacter(null);
                case "string": return new ValueString(null);
                case "error": return new ValueError(null, null);
                case "any": return new ValueAny(null);

                default: return null;
            }
        }

        public abstract ValueKind GetValueKind();
        public abstract bool IsNull();

        public virtual Value Plus(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "+");
        }

        public virtual Value LeftPlus()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "+");
        }

        public virtual Value PlusEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "+=");
        }
        
        public virtual Value LeftIncr()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "left ++");
        }

        public virtual Value RightIncr()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "right ++");
        }

        public virtual Value Minus(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "-");
        }

        public virtual Value LeftMinus()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "-");
        }

        public virtual Value MinusEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "-=");
        }

        public virtual Value LeftDecr()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "left --");
        }

        public virtual Value RightDecr()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "right --");
        }

        public virtual Value Times(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "*");
        }

        public virtual Value TimesEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "*=");
        }

        public virtual Value Div(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "/");
        }

        public virtual Value DivEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "/=");
        }

        public virtual Value Rem(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "%");
        }

        public virtual Value RemEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "%=");
        }

        public virtual Value BitwiseXOr(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "^");
        }

        public virtual Value BitwiseXOrEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "^=");
        }

        public virtual Value BitwiseAnd(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "&");
        }

        public virtual Value BitwiseAndEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "&=");
        }

        public virtual Value BitwiseOr(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "|");
        }

        public virtual Value BitwiseOrEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "|=");
        }

        public virtual Value Assign(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "=");
        }

        public virtual Value Get(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "[]");
        }

        public virtual Value Access(ref string name, byte type)
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, ".");
        }

        public virtual Value LeftShift(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "<<");
        }

        public virtual Value LeftShiftEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "<<=");
        }

        public virtual Value RightShift(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, ">>");
        }

        public virtual Value RightShiftEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, ">>=");
        }

        public virtual Value Not()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "!");
        }

        public virtual Value NotEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "!=");
        }

        public virtual Value Equals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "==");
        }

        public virtual Value Less(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "<");
        }

        public virtual Value LessEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "<=");
        }

        public virtual Value Great(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, ">");
        }

        public virtual Value GreatEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, ">=");
        }

        public virtual Value And(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "&&");
        }

        public virtual Value Or(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "||");
        }

        public virtual Value ReferenceOf()
        {
            ValueType t = (this.Convert(ref Globals.ToType) as ValueType)!;
            return Value.UnaryOpError(ref t, "$");
        }

        public virtual Value In(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "in");
        }

        public virtual Value Is(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType) as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType) as ValueType)!;
            return Value.BinaryOpError(ref t1, ref t2, "is");
        }

        public abstract Value Convert(ref ValueType dest);
        public abstract Value AutoConvert(ref ValueType dest);

        public override string ToString()
        {
            ValueString vs = (this.AutoConvert(ref Globals.ConvToString) as ValueString)!;
            return vs.Value ?? "null";
        }
    }
}