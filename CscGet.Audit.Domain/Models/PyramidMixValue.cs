using System;
using CscGet.Audit.Domain.Models.Core;

namespace CscGet.Audit.Domain.Models
{
    public class PyramidMixValue : ValueObject<PyramidMixValue>
    {
        public PyramidMixValue(int yearNumber, decimal value)
        {
            if (yearNumber < 1 || yearNumber > 51)
                throw new ArgumentOutOfRangeException(nameof(yearNumber), "Year number should be between 1 and 51");

            YearNumber = yearNumber;
            Value = value;
        }

        public int YearNumber { get; }
        public decimal Value { get; }

        protected override bool EqualsCore(PyramidMixValue other)
        {
            return YearNumber == other.YearNumber && Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                var hashCode = YearNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                return hashCode;
            }
        }
    }
}
