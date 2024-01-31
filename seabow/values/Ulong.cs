using utils;

namespace values
{
    public sealed class ValueUlong: Value
    {
        public ulong? Value{get;}
        
        public ValueUlong(ulong? val)
        {
            this.Value = val;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueUlong;
        }

        public override bool IsNull()
        {
            return this.Value == null;
        }

        public override Value Convert(ref ValueType dest)
        {
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    return this.IsNull() ? new ValueString("null")
                        : new ValueString(this.Value!.ToString());
                }

                case ValueKind.ValueType: {
                    return new ValueType(ValueKind.ValueUlong, null);
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
                    return this.IsNull() ? new ValueString("null")
                        : new ValueString(this.Value!.ToString());
                }
            }

            ValueType from = (this.Convert(ref Globals.ToType) as ValueType)!;
            return values.Value.AutoConvertionError(ref from, ref dest);
        }
    }
}