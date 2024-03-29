using core;
using utils;

namespace values
{
    public sealed class ValueDouble: Value
    {
        public double? Value{get; private set;}

        public ValueDouble(double? val)
        {
            this.Value = val;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueDouble;
        }

        public override bool IsNull()
        {
            return this.Value == null;
        }

        public override Element LeftMinus()
        {
            return new Element(0, new ValueDouble(-this.Value), ref Globals.EMPTY_MODIFIERS);
        }

        public override Element Convert(ref ValueType dest)
        {
            Value? val = null;

            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    val = this.IsNull() ? new ValueString("null")
                        : new ValueString(this.Value!.ToString());
                } break;

                case ValueKind.ValueType: {
                    val = new ValueType(ValueKind.ValueDouble, null);
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