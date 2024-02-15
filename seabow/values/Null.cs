using core;
using utils;

namespace values
{
    public sealed class ValueNull: Value
    {
        public ValueNull() {}

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueNull;
        }

        public override bool IsNull()
        {
            return true;
        }

        public override Element Convert(ref ValueType dest)
        {
            Value? val = null;
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    val = new ValueString("null");
                } break;

                case ValueKind.ValueType: {
                    val = new ValueType(ValueKind.ValueNull, null);
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

            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    val = new ValueString("null");
                } break;
            }

            if (val == null) {
                ValueType from = (this.Convert(ref Globals.ToType).Value as ValueType)!;
                return new Element(0, values.Value.AutoConvertionError(ref from, ref dest), ref Globals.DIAG_MODIFIERS);
            } else
                return new Element(0, val, ref Globals.EMPTY_MODIFIERS);
        }
    }
}