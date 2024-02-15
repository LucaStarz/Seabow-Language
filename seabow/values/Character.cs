using core;
using utils;

namespace values
{
    public sealed class ValueCharacter: Value
    {
        public char? Value{get;}

        public ValueCharacter(char? c)
        {
            this.Value = c;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueChar;
        }

        public override bool IsNull()
        {
            return this.Value == null;
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
                    val = new ValueType(ValueKind.ValueChar, null);
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