using core;
using utils;

namespace values
{
    public sealed class ValueError: Value
    {
        public string? Name{get;}
        public string? Details{get;}
        public List<string>? Context{get; private set;}

        public ValueError(string? name, string? details)
        {
            this.Name = name;
            this.Details = details;
            this.Context = null;
        }

        public void AddContext(string ctxt)
        {
            if (this.Context == null)
                this.Context = new List<string>();
            
            this.Context.Add(ctxt);
        }

        public override ValueKind GetValueKind()
        {
            return ValueKind.ValueError;
        }

        public override bool IsNull()
        {
            return this.Name == null;
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
                        string err = this.Name! + ((this.Details == null) ? "" : ": " + this.Details!);
                        val = new ValueString(err);
                    }
                } break;

                case ValueKind.ValueType: {
                    val = new ValueType(ValueKind.ValueError, null);
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