namespace SampleProject.Domain.Domains.ValueObject
{
    /// <summary>
    /// Value Object
    /// </summary>
    public abstract class ValueObjectClass
    {
        protected static bool EqualOperator(ValueObjectClass left, ValueObjectClass right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObjectClass left, ValueObjectClass right)
        {
            return !(EqualOperator(left, right));
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObjectClass)obj;

            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
            => GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);

        public ValueObjectClass GetCopy()
        {
            return this.MemberwiseClone() as ValueObjectClass;
        }
    }
}
