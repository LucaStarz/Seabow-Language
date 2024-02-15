using core;
using utils;

namespace values
{
    public sealed class ValueLong: Value
    {
        public long? Value{get; private set;}

        public ValueLong(long? val)
        {
            this.Value = val;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueLong;
        }

        public override bool IsNull()
        {
            return this.Value == null;
        }

        public override Element LeftMinus()
        {
            if (this.IsNull())
                return new Element(0, values.Value.OpWithNullError("unary -"), ref Globals.DIAG_MODIFIERS);
            return new Element(0, new ValueLong(-this.Value), ref Globals.EMPTY_MODIFIERS);
        }

        public override Element Assign(Value other)
        {
            switch (other.GetValueKind())
            {
                case ValueKind.ValueLong: {
                    this.Value = (other as ValueLong)!.Value;
                    return new Element(0, this, ref Globals.EMPTY_MODIFIERS);
                }

                case ValueKind.ValueUlong: {
                    this.Value = (long?)(other as ValueUlong)!.Value;
                    return new Element(0, this, ref Globals.EMPTY_MODIFIERS);
                }
            }

            return base.Assign(other);
        }

        public override Element Convert(ref ValueType dest)
        {
            Value? val = null;
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    val = this.IsNull() ? new ValueString("null")
                        : new ValueString(this.Value.ToString());
                } break;

                case ValueKind.ValueType: {
                    val = new ValueType(ValueKind.ValueLong, null);
                } break;
            }

            if (val == null) {
                ValueType from = (this.Convert(ref Globals.ToType).Value as ValueType)!;
                return new Element(0, values.Value.ConvertionError(ref from, ref dest), ref Globals.DIAG_MODIFIERS);
            } else
                return new Element(0, val, ref Globals.EMPTY_MODIFIERS);
        }

        public override Element AutoConvert(ref ValueType dest)
        {
            Value? val = null;

            if (val == null) {
                ValueType from = (this.Convert(ref Globals.ToType).Value as ValueType)!;
                return new Element(0, values.Value.AutoConvertionError(ref from, ref dest), ref Globals.DIAG_MODIFIERS);
            } else
                return new Element(0, val, ref Globals.EMPTY_MODIFIERS);
        }
    }
}