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

        public override Value Convert(ref ValueType dest)
        {
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    return this.IsNull() ? new ValueString("null")
                        : new ValueString(this.Value!.ToString());
                }

                case ValueKind.ValueType: {
                    return new ValueType(ValueKind.ValueChar, null);
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