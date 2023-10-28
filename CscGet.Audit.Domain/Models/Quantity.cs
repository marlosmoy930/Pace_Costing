using CscGet.Audit.Domain.Models.Core;

namespace CscGet.Audit.Domain.Models
{
    public sealed class Quantity : ValueObject<Quantity>
    {
        public Quantity(decimal value, int month, int year, string formula)
        {
            Value = value;
            Formula = formula;
            Date = new Date(year, month);
        }

        public decimal Value { get; }
        public Date Date { get; }
        public string Formula { get; }

        protected override bool EqualsCore(Quantity other)
        {
            return Value == other.Value && Date == other.Date && Formula == other.Formula;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                var hashCode = Value.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Formula?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}
