using utils;

namespace values
{
    public sealed class ValueError: Value
    {
        public string? Name{get;}
        public string? Details{get;}

        public ValueError(string? name, string? details)
        {
            this.Name = name;
            this.Details = details;
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueError;
        }

        public override bool IsNull()
        {
            return this.Name == null;
        }

        public override Value Convert(ref ValueType dest)
        {
            switch (dest.Kind)
            {
                case ValueKind.ValueString: {
                    if (this.IsNull())
                        return new ValueString("null");
                    
                    string err = this.Name! + ((this.Details == null) ? "" : ": " + this.Details!);
                    return new ValueString(err);
                }

                case ValueKind.ValueType: {
                    return new ValueType(ValueKind.ValueError, null);
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
                    
                    string err = this.Name! + ((this.Details == null) ? "" : this.Details!);
                    return new ValueString(err);
                }
            }

            ValueType from = (this.Convert(ref Globals.ToType) as ValueType)!;
            return values.Value.AutoConvertionError(ref from, ref dest);
        }
    }
}