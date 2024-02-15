using core;
using utils;

namespace values
{
    public sealed class ValueType: Value
    {
        public ValueKind Kind{get;}
        public string? Completion{get;}

        public ValueType(ValueKind kind, string? completion)
        {
            this.Kind = kind;
            this.Completion = completion;
        }

        public ValueType()
        {
            this.Kind = ValueKind.ValueUndefined;
            this.Completion = null;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueType;
        }

        public override bool IsNull()
        {
            return this.Kind == ValueKind.ValueUndefined;
        }

        public override Element Convert(ref ValueType dest)
        {
            Value? val = null;
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    if (this.IsNull())
                        val = new ValueString("null");
                    else {
                        string? err = values.Value.ValueKindToString(this.Kind) + (this.Completion == null ? "" : String.Format("[{0}]", this.Completion));
                        if (err == null)
                        {
                            
                        }

                        val = new ValueString(err);
                    }
                } break;

                case ValueKind.ValueType: {
                    val = new ValueType(ValueKind.ValueType, null);
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