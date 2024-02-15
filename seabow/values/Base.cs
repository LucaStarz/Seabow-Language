using core;
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
            return kind switch
            {
                ValueKind.ValueNull => "<null>",
                ValueKind.ValueAny => "<any>",
                ValueKind.ValueByte => "<byte>",
                ValueKind.ValueUbyte => "<ubyte>",
                ValueKind.ValueLong => "<long>",
                ValueKind.ValueUlong => "<ulong>",
                ValueKind.ValueUint128 => "<uint128>",
                ValueKind.ValueDouble => "<double>",
                ValueKind.ValueLdouble => "<ldouble>",
                ValueKind.ValueBool => "<bool>",
                ValueKind.ValueChar => "<char>",
                ValueKind.ValueString => "<string>",
                ValueKind.ValueType => "<type>",
                ValueKind.ValueError => "<error>",
                _ => null,
            };
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
            return kind switch
            {
                "byte" => new ValueByte(null),
                "ubyte" => new ValueUbyte(null),
                "long" => new ValueLong(null),
                "ulong" => new ValueUlong(null),
                "uint128" => new ValueUint128(null),
                "double" => new ValueDouble(null),
                "ldouble" => new ValueLdouble(null),
                "bool" => new ValueBool(null),
                "char" => new ValueCharacter(null),
                "string" => new ValueString(null),
                "error" => new ValueError(null, null),
                "type" => new ValueType(),
                "any" => new ValueAny(null),
                _ => null,
            };
        }

        public abstract ValueKind GetValueKind();
        public abstract bool IsNull();

        public virtual Element Plus(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "+"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element LeftPlus()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "+"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element PlusEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "+="), ref Globals.DIAG_MODIFIERS);
        }
        
        public virtual Element LeftIncr()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "left ++"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element RightIncr()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "right ++"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Minus(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "-"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element LeftMinus()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "-"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element MinusEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "-="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element LeftDecr()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "left --"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element RightDecr()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "right --"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Times(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "*"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element TimesEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "*="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Div(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "/"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element DivEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "/="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Rem(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "%"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element RemEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "%="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element BitwiseXOr(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "^"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element BitwiseXOrEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "^="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element BitwiseAnd(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "&"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element BitwiseAndEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "&="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element BitwiseOr(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "|"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element BitwiseOrEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "|="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Assign(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Get(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "[]"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Access(ref string name, byte type)
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "."), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element LeftShift(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "<<"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element LeftShiftEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "<<="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element RightShift(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, ">>"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element RightShiftEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, ">>="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Not()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "!"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element NotEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "!="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Equals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "=="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Less(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "<"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element LessEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "<="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Great(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, ">"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element GreatEquals(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, ">="), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element And(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "&&"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Or(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "||"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element ReferenceOf()
        {
            ValueType t = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.UnaryOpError(ref t, "$"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element In(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "in"), ref Globals.DIAG_MODIFIERS);
        }

        public virtual Element Is(Value other)
        {
            ValueType t1 = (this.Convert(ref Globals.ToType).Value as ValueType)!;
            ValueType t2 = (other.Convert(ref Globals.ToType).Value as ValueType)!;
            return new Element(0, Value.BinaryOpError(ref t1, ref t2, "is"), ref Globals.DIAG_MODIFIERS);
        }

        public abstract Element Convert(ref ValueType dest);
        public abstract Element AutoConvert(ref ValueType dest);

        public override string ToString()
        {
            ValueString vs = (this.Convert(ref Globals.ConvToString).Value as ValueString)!;
            return vs.Value ?? "null";
        }
    }
}