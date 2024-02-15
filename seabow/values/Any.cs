using core;
using utils;

namespace values
{
    public sealed class ValueAny: Value
    {
        public Value? Value{get;}

        public ValueAny(Value? val)
        {
            this.Value = val;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueAny;
        }

        public override bool IsNull()
        {
            return this.Value == null;
        }

        public override Element Convert(ref ValueType dest)
        {
            Element? elt = this.Value?.Convert(ref dest);

            if (elt == null) {
                ValueType from = (this.Convert(ref Globals.ToType).Value as ValueType)!;
                return new Element(0, values.Value.ConvertionError(ref from, ref dest), ref Globals.DIAG_MODIFIERS);
            } else
                return elt;
        }

        public override Element AutoConvert(ref ValueType dest)
        {
            Element? elt = this.Value?.Convert(ref dest);

            if (elt == null) {
                ValueType from = (this.Convert(ref Globals.ToType).Value as ValueType)!;
                return new Element(0, values.Value.AutoConvertionError(ref from, ref dest), ref Globals.DIAG_MODIFIERS);
            } else
                return elt;
        }
    }
}