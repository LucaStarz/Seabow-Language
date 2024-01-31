using utils;

namespace values
{
    public sealed class ValueBool : Value
    {
        public bool? Value{get;}

        public ValueBool(bool? val)
        {
            this.Value = val;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueBool;
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
                        : this.Value! == true ? new ValueString("true") : new ValueString("false");
                }

                case ValueKind.ValueType: {
                    return new ValueType(ValueKind.ValueBool, null);
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
                        : this.Value! == true ? new ValueString("true") : new ValueString("false");
                }
            }

            ValueType from = (this.Convert(ref Globals.ToType) as ValueType)!;
            return values.Value.AutoConvertionError(ref from, ref dest);
        }
    }
}