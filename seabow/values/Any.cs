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

        public override Value Convert(ref ValueType dest)
        {
            throw new NotImplementedException();
        }

        public override Value AutoConvert(ref ValueType dest)
        {
            throw new NotImplementedException();
        }
    }
}