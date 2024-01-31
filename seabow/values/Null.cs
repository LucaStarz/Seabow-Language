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

        public override Value Convert(ref ValueType dest)
        {
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    return new ValueString("null");
                }

                case ValueKind.ValueType: {
                    return new ValueType(ValueKind.ValueNull, null);
                }
            }

            ValueType from = (this.Convert(ref Globals.ToType) as ValueType)!;
            return values.Value.ConvertionError(ref from, ref dest);
        }

        public override Value AutoConvert(ref ValueType dest)
        {
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    return new ValueString("null");
                }
            }

            ValueType from = (this.Convert(ref Globals.ToType) as ValueType)!;
            return values.Value.AutoConvertionError(ref from, ref dest);
        }
    }
}