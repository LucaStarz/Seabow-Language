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

        public override Value Convert(ref ValueType dest)
        {
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    if (this.IsNull())
                        return new ValueString("null");
                    
                    string? err = values.Value.ValueKindToString(this.Kind) + (this.Completion == null ? "" : String.Format("[{0}]", this.Completion));
                    if (err == null)
                    {
                        
                    }

                    return new ValueString(err);
                }

                case ValueKind.ValueType: {
                    return new ValueType(ValueKind.ValueType, null);
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
                    if (this.IsNull())
                        return new ValueString("null");
                    
                    string err = values.Value.ValueKindToString(this.Kind) + (this.Completion == null ? "" : String.Format("[{0}]", this.Completion));
                    return new ValueString(err);
                }
            }

            ValueType from = (this.Convert(ref Globals.ToType) as ValueType)!;
            return values.Value.AutoConvertionError(ref from, ref dest);
        }
    }
}