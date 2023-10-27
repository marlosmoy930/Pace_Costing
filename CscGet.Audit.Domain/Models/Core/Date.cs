using System;

namespace CscGet.Audit.Domain.Models.Core
{
    public sealed class Date : ValueObject<Date>, IComparable<Date>
    {
        public Date(int year, int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));

            if (year < 1950)
                throw new ArgumentOutOfRangeException(nameof(year));

            Year = year;
            Month = month;
        }

        public int Year { get; }
        public int Month { get; }

        protected override bool EqualsCore(Date other)
        {
            return Year == other.Year && Month == other.Month;
        }

        protected override int GetHashCodeCore()
        {
            var hashCode = Year.GetHashCode();
            hashCode = (hashCode * 397) ^ Month;
            return hashCode;
        }

        public int CompareTo(Date other)
        {
            if (other == null || other < this)
                return 1;

            if (this == other)
                return 0;

            return -1;
        }
        
        public static bool operator >=(Date operandA, Date operandB)
        {
            if (operandA == null)
                throw new ArgumentNullException(nameof(operandA));
            if (operandB == null)
                throw new ArgumentNullException(nameof(operandB));

            var dateTimeA = new DateTime(operandA.Year, operandA.Month, 1);
            var dateTimeB = new DateTime(operandB.Year, operandB.Month, 1);

            return dateTimeA >= dateTimeB;
        }

        public static bool operator >(Date operandA, Date operandB)
        {
            if (operandA == null)
                throw new ArgumentNullException(nameof(operandA));
            if (operandB == null)
                throw new ArgumentNullException(nameof(operandB));

            var dateTimeA = new DateTime(operandA.Year, operandA.Month, 1);
            var dateTimeB = new DateTime(operandB.Year, operandB.Month, 1);

            return dateTimeA > dateTimeB;
        }

        public static bool operator <(Date operandA, Date operandB)
        {
            if (operandA == null)
                throw new ArgumentNullException(nameof(operandA));
            if (operandB == null)
                throw new ArgumentNullException(nameof(operandB));

            var dateTimeA = new DateTime(operandA.Year, operandA.Month, 1);
            var dateTimeB = new DateTime(operandB.Year, operandB.Month, 1);

            return dateTimeA < dateTimeB;
        }

        public static bool operator <=(Date operandA, Date operandB)
        {
            var dateTimeA = new DateTime(operandA.Year, operandA.Month, 1);
            var dateTimeB = new DateTime(operandB.Year, operandB.Month, 1);

            return dateTimeA <= dateTimeB;
        }

        public static int operator -(Date operandA, Date operandB)
        {
            if (operandA == null)
                throw new ArgumentNullException(nameof(operandA));
            if (operandB == null)
                throw new ArgumentNullException(nameof(operandB));

            var aLessThanB = operandA < operandB;

            if (aLessThanB)
            {
                var tempA = new Date(operandA.Year, operandA.Month);
                var tempB = new Date(operandB.Year, operandB.Month);
                operandA = tempB;
                operandB = tempA;
            }

            var numberOfMonths = operandA.Month - operandB.Month +
                                 12 * (operandA.Year - operandB.Year) + 1;

            return aLessThanB ? -numberOfMonths : numberOfMonths;
        }

        public static implicit operator Date(DateTime dateTime)
        {
            return new Date(dateTime.Year, dateTime.Month);
        }

        public static implicit operator Date(DateTime? dateTime)
        {
            if (dateTime == null)
                return null;

            return new Date(dateTime.Value.Year, dateTime.Value.Month);
        }

        public static implicit operator DateTime(Date date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }
}
